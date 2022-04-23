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

            List<string> urls = new List<string>();
            try
            {
                var pageNumber = 1;
                var maxPageNumber = 3;
                bool dateFound = false;
                string infoUrl = string.Empty;
                string contentPageNumber = string.Empty;
                while (!dateFound || pageNumber > maxPageNumber)
                {
                    IList<string> urlContainer = new List<string>();
                    HtmlWeb htmlWeb = new HtmlWeb();
                    var pageUrl = $"{Domain}/bg/news?p={pageNumber}";
                    var resultFromRequest = await htmlWeb.LoadFromWebAsync(pageUrl);

                    HtmlNodeCollection nodeCollection = resultFromRequest
                        .DocumentNode
                        .SelectNodes("//*[@class=\"col-md-4 listing-news-wrapper\"]");

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
                            dateFound = GatherPageContent(fullUrlPath, infoUrlPage, searchedDate);
                            if (dateFound)
                            {
                                contentPageNumber = infoUrlPage;
                                infoUrl = fullUrlPath;
                                break;
                            }
                        }
                    }

                    pageNumber++;
                }

                if (string.IsNullOrEmpty(contentPageNumber))
                {
                    this.logger.LogInformation($"No content found for date {searchedDate}.");
                    return;
                }
                var entity = CreateEntity(contentPageNumber);
                var stringBuilder = new StringBuilder();
                var result = new List<DayInformation>();

                if (entity.TotalPositive == null && entity.TotalTestsMade == null)
                {
                    stringBuilder.AppendLine($"Pattern not found for file {entity.FileName}.");
                    return;
                }

                result.Add(entity.MapFrom());

                await this.dayInformationRepository.AddManyAsync(result);

                this.logger.LogInformation("Daily information gathered.");
                
                await this.LostBattleInformation(contentPageNumber);
                
                this.logger.LogInformation("Ready.");
            }
            catch (Exception ex)
            {
                this.logger.LogInformation(ex, $"Something went wrong when gathering data for date {searchedDate}.");
            }
        }

        private async Task LostBattleInformation(string contentId)
        {
            this.logger.LogInformation("Start gathering lost battle information");
            var fileName = "Content" + contentId + ".txt";
            Expression<Func<DayInformation, bool>> func = x => x.LostBattle == null && x.FileName == fileName;
            
                var information = await this.dayInformationRepository.GetAsync(func);
                var filePath = fileContainerPathContent + information.FileName;

                var entitiesToCollect = new List<LostBattle>();
                StringBuilder allContent = new StringBuilder();
                using (StreamReader file = new StreamReader(filePath))
                {
                    int counter = 0;
                    string ln;
                    while ((ln = file.ReadLine()) != null)
                    {
                        counter++;
                        if (counter > 1 && ln.Trim().Length > 0)
                        {
                            allContent.Append(ln.ToLower());
                        }
                    }

                    file.Close();
                }
                var content = allContent.ToString().ToLower();
                var reg1 = Regex.Matches(content, @"(мъж (на\s)*[0-9]+|жена (на\s)*[0-9]+)");
                var reg2 = Regex.Matches(content, @"[0-9]+\-(годишен мъж|годишна жена)");

                MatchCollection allMatches = reg1.Any() ? reg1 : reg2.Any() ? reg2 : null;
                if (allMatches == null)
                {
                    entitiesToCollect.Add(new LostBattle { DayInformationId = information.Id });
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
                        DayInformationId = information.Id,
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

        private bool GatherPageContent(string url, string pageNumber, DateTime searchedDate)
        {
            this.logger.LogInformation("Start gathering page content ...");
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
                    this.logger.LogWarning($"Date for page {pageNumber} is missing.");
                    return false;
                }

                var date = DateUtility.GetDateByString(newsDateString?.InnerText);
                if (date != searchedDate)
                {
                    this.logger.LogWarning($"Date {date} is different than the searched one {searchedDate}.");
                    return false;
                }
                if (newsContentString?.InnerText != null)
                {
                    newContentTextToGet = newsContentString?.InnerText.Substring(lengthToRemove);
                }
                string fileFullPath = $@"{fileContainerPathContent}Content{pageNumber}.txt";
                if (File.Exists(fileFullPath))
                {
                    this.logger.LogWarning($"File for page {pageNumber} already exists.");
                    return false;
                }

                File.WriteAllText(
                    $@"{fileContainerPathContent}Content{pageNumber}.txt",
                    dateString + "\r\n" + newContentTextToGet,
                    Encoding.UTF8
                    );
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Something went wrong when gathering content from url {url}");
                return false;
            }

            this.logger.LogInformation($"Gathering page content end.");
            return true;
        }

        private DayInformationModel CreateEntity(string pageNumber)
        {
            var entity = new DayInformationModel();
            var fileFullPath = $@"{fileContainerPathContent}Content{pageNumber}.txt";
            var fileName = Path.GetFileName(fileFullPath);
            try
            {
                
                StringBuilder allContent = new StringBuilder();
                this.logger.LogInformation($"Start working on file: {fileName}.");

                using (StreamReader file = new StreamReader(fileFullPath))
                {
                    int counter = 0;
                    string ln;
                    entity.FileName = fileName;
                    while ((ln = file.ReadLine()) != null)
                    {
                        counter++;
                        if (counter == 1)
                        {
                            entity.DayDate = DateUtility.GetDateByString(ln);
                        }
                        else if (ln.Trim().Length > 0)
                        {
                            allContent.Append(ln.ToLower());
                        }
                    }

                    file.Close();
                    var contentString = allContent.ToString()
                        .Replace("covid-19", "covid-DD")
                        .Replace("24 часа", "DD часа")
                        .Replace("24 ч", "DD ч");

                    AnalyzeFileContent.GatherInformationByFoundPattern(contentString, entity);

                    this.logger.LogInformation($"Completed: file: {fileName}.");
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Something went wrong while creating entity from gathered text. filename:[{fileName}]");
            }

            return entity;
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
    }
}
