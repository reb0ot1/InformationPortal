using CovidInformationPortal.Data;
using CovidInformationPortal.Models.ScrapedData;
using CovidInformationPortal.Services.Utilities;
using CovidInformationPortal.Services.Utilities.ScrapedDataProcess;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CovidInformationPortal.Services
{
    public class DataGatheringService : IDataGatheringService
    {
        private const string Domain = "https://coronavirus.bg";
        private static string fileContainerPathContent = @"C:\\Projects\Covid\CovidScraper\CovidScraper\Content\";

        private readonly ILogger<DataGatheringService> logger;
        private readonly IRepository<DayInformation> dayInformationRepository;
        private readonly IRepository<LostBattle> lostBattleRepository;

        public DataGatheringService(
                ILogger<DataGatheringService> logger,
                IRepository<DayInformation> dayInfoRepository,
                IRepository<LostBattle> lostBattleRepository
            )
        {
            this.logger = logger;
            this.dayInformationRepository = dayInfoRepository;
            this.lostBattleRepository = lostBattleRepository;
        }

        public async Task StartGatheringData(DateTime searchedDate)
        {
            this.logger.LogInformation("Try get day information.");
            var exists = await this.DateExists(searchedDate);
            if (exists)
            {
                this.logger.LogInformation($"Date {searchedDate} already exists.");
                return;
            }

            try
            {
                var contentResult = this.GetContent(searchedDate);
                var dataAsByteArray = contentResult.Content;
                if (!dataAsByteArray.Any())
                {
                    this.logger.LogInformation($"No content found for date {searchedDate}.");
                    return;
                }

                var content = ProccessContent(dataAsByteArray);
                if (content == null)
                {
                    return;
                }
                var entity = new DayInformationModel();

                entity.DayDate = searchedDate;
                entity.FileName = contentResult.ContentPageNumber;
                AnalyzeFileContent.GatherInformationByFoundPattern(content, entity);

                var dbEntity = entity.MapFrom();

                await this.dayInformationRepository.AddAsync(dbEntity);

                this.logger.LogInformation("Daily information gathered.");
                
                await this.LostBattleInformation(content, dbEntity.Id);
                
                this.logger.LogInformation("Ready.");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Something went wrong when gathering data for date {searchedDate}.");
            }
        }

        private async Task LostBattleInformation(string content, int dbEntityId)
        {
            this.logger.LogInformation("Start gathering lost battle information");
            Expression<Func<DayInformation, bool>> func = x => x.Id == dbEntityId;

            var entitiesToCollect = new List<LostBattle>();
           
            var reg1 = Regex.Matches(content, @"(мъж (на\s)*[0-9]+|жена (на\s)*[0-9]+)");
            var reg2 = Regex.Matches(content, @"[0-9]+\-(годишен мъж|годишна жена)");

            MatchCollection allMatches = reg1.Any() ? reg1 : reg2.Any() ? reg2 : null;
            if (allMatches == null)
            {
                entitiesToCollect.Add(new LostBattle { DayInformationId = dbEntityId });
            }
            else
            {
                var entities = new List<LostBattleEntityModel>();
                var count = allMatches.Count();

                foreach (var item1 in allMatches.Select(e => e))
                {
                    LostBattleEntityModel entity = new LostBattleEntityModel();
                    if (item1.Value.IndexOf("жена") != -1)
                    {
                        entity.Gender = Gender.Female;
                    }
                    else if (item1.Value.IndexOf("мъж") != -1)
                    {
                        entity.Gender = Gender.Male;
                    }

                    var numSearch = Regex.Match(item1.Value, @"[0-9]+");
                    entity.Age = int.Parse(numSearch.Value);
                    entities.Add(entity);
                }

                var averageAge = entities.Select(e => e.Age).Average();
                var males = entities.Where(e => e.Gender == Gender.Male).Count();
                var females = entities.Where(e => e.Gender == Gender.Female).Count();
                entitiesToCollect.Add(new LostBattle
                {
                    DayInformationId = dbEntityId,
                    AverageAge = averageAge,
                    Count = entities.Count(),
                    Females = females,
                    Males = males
                });
            }

            if (entitiesToCollect.Any())
            {
                await this.lostBattleRepository.AddManyAsync(entitiesToCollect);
            }

            this.logger.LogInformation("Lost battle information gathered.");
        }

        private string ProccessContent(byte[] content)
        {
            try
            {
                var result = string.Empty;
                MemoryStream memoryStream = new MemoryStream(content);
                StringBuilder allContent = new StringBuilder();
                using (StreamReader file = new StreamReader(memoryStream))
                {
                    string ln;
                    while ((ln = file.ReadLine()) != null)
                    {
                        if (ln.Trim().Length > 0)
                        {
                            allContent.Append(ln.ToLower());
                        }
                    }

                    result = allContent.ToString()
                        .Replace("covid-19", "covid-DD")
                        .Replace("24 часа", "DD часа")
                        .Replace("24 ч", "DD ч")
                        .ToLower();
                }

                return result;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Something went wrong while trying to process content.");
                return null;
            }
        }

        private async Task<bool> DateExists(DateTime searchedDate)
        {
            var dateFromDb = await this.dayInformationRepository.GetAsync(e => e.Date == searchedDate);
            if (dateFromDb != null)
            {
                return true;
            }

            return false;
        }

        private (string ContentPageNumber, byte[] Content) GetContent(DateTime searchedData)
        {
            byte[] dateFound = new byte[] { };
            string contentPageNumber = string.Empty;
            try
            {
                var pageNumber = 1;
                var maxPageNumber = 2;
                string infoUrl = string.Empty;
                
                while (!dateFound.Any() || pageNumber < maxPageNumber)
                {
                    IList<string> urlContainer = new List<string>();
                    HtmlWeb htmlWeb = new HtmlWeb();
                    var pageUrl = $"{Domain}/bg/news?p={pageNumber}";
                    var resultFromRequest = htmlWeb.Load(pageUrl);

                    HtmlNodeCollection nodeCollection = resultFromRequest.DocumentNode.SelectNodes("//*[@class=\"col-md-4 listing-news-wrapper\"]");
                    for (int i = 0; i < nodeCollection.Count; i++)
                    {
                        var node = nodeCollection[i];
                        var searchedContainer = node.SelectSingleNode("a/h4");
                        var text = searchedContainer?.InnerText;
                        if (text != null && (text.Contains("са потвърдените случаи на") || text.Contains("нови случая на")))
                        {
                            var page = node.SelectSingleNode("a");
                            var hrefValue = page.Attributes.FirstOrDefault(e => e.Name == "href");
                            var fullUrlPath = $"{Domain}{hrefValue.Value}";
                            var infoUrlPage = fullUrlPath.Split("/").Last();
                            dateFound = this.GatherPageContent2(fullUrlPath, infoUrlPage, searchedData);
                            if (dateFound != null && dateFound.Any())
                            {
                                contentPageNumber = infoUrlPage;
                                infoUrl = fullUrlPath;
                                break;
                            }
                        }
                    }

                    pageNumber++;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return (contentPageNumber,dateFound);
        }

        private byte[] GatherPageContent2(string url, string pageNumber, DateTime searchedDate)
        {
            Console.WriteLine("Start gathering page content ...");
            byte[] contAsByteArray = new byte[] { };
            try
            {
                HtmlWeb htmlWeb = new HtmlWeb();
                var result = htmlWeb.Load(url);
                var newsDateString = result.DocumentNode.SelectSingleNode("//*[@class=\"single-news-date\"]");
                var newsContentString = result.DocumentNode.SelectSingleNode("//*[@class=\"single-news-content\"]");
                var textForRemove = result.DocumentNode.SelectNodes("//*[@class=\"single-news-content\"]//p[1]//strong");
                var lengthToRemove = 0;
                if (textForRemove != null)
                {
                    foreach (var text in textForRemove)
                    {
                        lengthToRemove += text.InnerLength;
                    }
                }

                //Check if file exists
                var newContentTextToGet = string.Empty;
                var dateString = newsDateString?.InnerText;
                if (string.IsNullOrEmpty(dateString))
                {
                    Console.WriteLine($"Date for page {pageNumber} is missing.");
                    return contAsByteArray;
                }

                var date = DateUtility.GetDateByString(newsDateString?.InnerText);
                if (date != searchedDate)
                {
                    Console.WriteLine($"Date {date} is different than the searched one {searchedDate}.");
                    return contAsByteArray;
                }
                if (newsContentString?.InnerText != null)
                {
                    newContentTextToGet = newsContentString?.InnerText.Substring(lengthToRemove);
                }

                var cont = newContentTextToGet;
                contAsByteArray = Encoding.UTF8.GetBytes(cont);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong when gathering content from url {url}");
                return null;
            }

            Console.WriteLine($"Gathering page content end.");
            return contAsByteArray;
        }
    }
}
