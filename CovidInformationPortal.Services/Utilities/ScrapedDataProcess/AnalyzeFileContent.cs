using CovidInformationPortal.Models.ScrapedData;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CovidInformationPortal.Services.Utilities.ScrapedDataProcess
{
    internal static class AnalyzeFileContent
    {
        private const string Nsi = "националната информационна система";
        private const string Nos = "националния оперативен щаб";
        private const string Nos2 = "националният оперативен щаб";

        private const string NsiAntigenni = "антигенни";
        private const string NsiVaccinated = "ваксинирани";
        private const string NsiVaccinated1 = "ваксинационен цикъл";
        private const string NsiNewFound = "новодиагностицираните";
        private const string Counties = "разпределението по области";

        private const string NosNewCases1 = "нови случая";
        private const string NosNewCases2 = "нови случаи";
        private const string NosNewCases3 = "новите случаи";

        private const string NewCasesAre = "новите случаи са";

        private const string DuringTheDay = "през днешния ден са доказани";
        private const string PositiveResult = "положителен резултат";

        public static void GatherInformationByFoundPattern(string content, DayInformationModel entity)
        {
            int searchedIndex;
            searchedIndex = content.IndexOf(Nsi);
            if (searchedIndex != -1)
            {
                var content1 = content.Substring(0, searchedIndex);
                if (content1.IndexOf(NsiNewFound) != -1)
                {
                    var numbersAsString1 = GetNumbersByTemplate(content1);
                    if (numbersAsString1.Count() == 2)
                    {
                        entity.TotalPositive = int.Parse(numbersAsString1[0]);
                        entity.TotalTestsMade = int.Parse(numbersAsString1[1]);
                        entity.PaternId = 2;

                        Console.WriteLine($"Pattern found: {NsiNewFound}", UTF8Encoding.UTF8);
                        return;
                    }
                }

                //Antigenni
                if (content1.IndexOf(NsiAntigenni) != -1)
                {
                    //Vaccinated
                    if (content1.IndexOf(NsiVaccinated) != -1 || content1.IndexOf(NsiVaccinated1) != -1)
                    {
                        var numbersAsString2 = GetNumbersByTemplate(content1, @"\d+[\ \,]*\d*");
                        if (numbersAsString2.Count() == 7)
                        {
                            entity.TotalPositive = int.Parse(numbersAsString2[0]);
                            entity.PositiveWithPCR = int.Parse(numbersAsString2[1]);
                            entity.PositiveWithQuickTest = int.Parse(numbersAsString2[2]);
                            entity.VaccinatedPercentage = decimal.Parse(numbersAsString2[3]);
                            entity.TotalTestsMade = int.Parse(numbersAsString2[4]);
                            entity.TotalTestsPCR = int.Parse(numbersAsString2[5]);
                            entity.TotalTestsQuick = int.Parse(numbersAsString2[6]);
                            entity.PaternId = 3;

                            Console.WriteLine($"Pattern found: {NsiVaccinated}", UTF8Encoding.UTF8);
                            return;
                        }
                        if (numbersAsString2.Count() == 8)
                        {
                            entity.TotalPositive = int.Parse(numbersAsString2[0]);
                            entity.PositiveWithPCR = int.Parse(numbersAsString2[1]);
                            entity.PositiveWithQuickTest = int.Parse(numbersAsString2[2]);
                            entity.VaccinatedPercentage = decimal.Parse(numbersAsString2[4]);
                            entity.TotalTestsMade = int.Parse(numbersAsString2[5]);
                            entity.TotalTestsPCR = int.Parse(numbersAsString2[6]);
                            entity.TotalTestsQuick = int.Parse(numbersAsString2[7]);
                            entity.PaternId = 12;

                            Console.WriteLine($"Pattern found: {NsiVaccinated}", UTF8Encoding.UTF8);
                            return;
                        }
                    }

                    var numbersAsString = GetNumbersByTemplate(content1);
                    if (numbersAsString.Count() == 6)
                    {
                        entity.TotalPositive = int.Parse(numbersAsString[0]);
                        entity.PositiveWithPCR = int.Parse(numbersAsString[1]);
                        entity.PositiveWithQuickTest = int.Parse(numbersAsString[2]);
                        entity.TotalTestsMade = int.Parse(numbersAsString[3]);
                        entity.TotalTestsPCR = int.Parse(numbersAsString[4]);
                        entity.TotalTestsQuick = int.Parse(numbersAsString[5]);
                        entity.PaternId = 4;

                        Console.WriteLine($"Pattern found: {NsiAntigenni}", UTF8Encoding.UTF8);
                        return;
                    }

                    if (numbersAsString.Count() == 5)
                    {
                        entity.TotalPositive = int.Parse(numbersAsString[0]);
                        entity.PositiveWithQuickTest = int.Parse(numbersAsString[1]);
                        entity.TotalTestsMade = int.Parse(numbersAsString[2]);
                        entity.TotalTestsPCR = int.Parse(numbersAsString[3]);
                        entity.TotalTestsQuick = int.Parse(numbersAsString[4]);
                        entity.PaternId = 14;

                        Console.WriteLine($"Pattern found: {NsiAntigenni}", UTF8Encoding.UTF8);
                        return;
                    }

                }

                var countyContentIndex = content1.IndexOf(Counties);
                if (countyContentIndex != -1)
                {
                    var content2 = content1.Substring(0, countyContentIndex);
                    var numbersAsString = GetNumbersByTemplate(content2);
                    if (numbersAsString.Count() == 2)
                    {
                        var number1 = int.Parse(numbersAsString[0]);
                        var number2 = int.Parse(numbersAsString[1]);

                        if (number1 > number2)
                        {
                            entity.TotalPositive = number2;
                            entity.TotalTestsMade = number1;
                        }
                        else
                        {
                            entity.TotalPositive = number1;
                            entity.TotalTestsMade = number2;
                        }
                        entity.PaternId = 5;
                        Console.WriteLine($"Pattern found: {Nsi} -- {Counties}", UTF8Encoding.UTF8);
                        return;
                    }
                }
            }

            searchedIndex = content.IndexOf(Nos);
            if (searchedIndex == -1)
            {
                searchedIndex = content.IndexOf(Nos2);
            }

            if (searchedIndex != -1)
            {
                var content1 = content.Substring(0, searchedIndex);
                if (content1.IndexOf(NosNewCases1) != -1 || content1.IndexOf(NosNewCases2) != -1 || content1.IndexOf(NosNewCases3) != -1)
                {
                    var numbersAsString = GetNumbersByTemplate(content1);
                    if (numbersAsString.Count() == 2)
                    {
                        var number1 = int.Parse(numbersAsString[0]);
                        var number2 = int.Parse(numbersAsString[1]);

                        if (number1 > number2)
                        {
                            entity.TotalPositive = number2;
                            entity.TotalTestsMade = number1;
                        }
                        else
                        {
                            entity.TotalPositive = number1;
                            entity.TotalTestsMade = number2;
                        }

                        entity.PaternId = 6;
                        Console.WriteLine($"Pattern found: {Nos} -- {NosNewCases2} OR {NosNewCases3} with 2 numbers", UTF8Encoding.UTF8);
                        return;
                    }
                    else if (numbersAsString.Count() == 1)
                    {
                        entity.TotalPositive = int.Parse(numbersAsString[0]);
                        entity.PaternId = 7;
                        Console.WriteLine($"Pattern found: {Nos} -- {NosNewCases2} OR {NosNewCases3} with 1 number", UTF8Encoding.UTF8);
                        return;
                    }
                }
                if (content1.IndexOf(PositiveResult) != -1)
                {
                    var numbersAsString = GetNumbersByTemplate(content1);
                    int number1, number2;

                    if (numbersAsString.Count() == 2 || numbersAsString.Count() > 2)
                    {
                        number1 = int.Parse(numbersAsString[0]);
                        number2 = int.Parse(numbersAsString[1]);
                        if (number1 > number2)
                        {
                            entity.TotalPositive = number2;
                            entity.TotalTestsMade = number1;
                        }
                        else
                        {
                            entity.TotalPositive = number1;
                            entity.TotalTestsMade = number2;
                        }
                        if (numbersAsString.Count() == 2)
                        {
                            entity.PaternId = 8;
                        }
                        if (numbersAsString.Count() > 2)
                        {
                            entity.PaternId = 11;
                        }
                        Console.WriteLine($"Pattern found: {Nos} -- {PositiveResult}", UTF8Encoding.UTF8);
                        return;
                    }
                }
            }

            searchedIndex = content.IndexOf(NosNewCases1);
            if (searchedIndex != -1)
            {
                var content1 = content.Substring(0, searchedIndex);
                if (content1.IndexOf(NewCasesAre) != -1)
                {
                    var numbersAsString = GetNumbersByTemplate(content1);
                    var number1 = numbersAsString.Last();
                    entity.TotalPositive = int.Parse(number1);

                    entity.PaternId = 9;
                    Console.WriteLine($"Pattern found: {NewCasesAre} -- {NosNewCases1}", UTF8Encoding.UTF8);
                    return;
                }
            }

            searchedIndex = content.IndexOf(Counties);
            if (searchedIndex != -1)
            {
                var content1 = content.Substring(0, searchedIndex);
                string[] numbersAsString = GetNumbersByTemplate(content1);
                if (numbersAsString.Count() == 2)
                {
                    var number1 = int.Parse(numbersAsString[0]);
                    var number2 = int.Parse(numbersAsString[1]);

                    if (number1 > number2)
                    {
                        entity.TotalPositive = number2;
                        entity.TotalTestsMade = number1;
                    }
                    else
                    {
                        entity.TotalPositive = number1;
                        entity.TotalTestsMade = number2;
                    }

                    entity.PaternId = 10;

                    Console.WriteLine($"Pattern found: {Counties}", UTF8Encoding.UTF8);
                    return;
                }
            }

            searchedIndex = content.IndexOf(DuringTheDay);
            if (searchedIndex != -1)
            {
                var content1 = content.Substring(searchedIndex, DuringTheDay.Length + 10);
                string[] numbersAsString = GetNumbersByTemplate(content1);
                var number = numbersAsString[0];
                entity.TotalPositive = int.Parse(number);
                entity.PaternId = 13;
                return;
            }
        }

        private static string[] GetNumbersByTemplate(string content, string regex = null)
        {
            regex = regex ?? @"\d+[\ ]*\d*";
            MatchCollection matches = Regex.Matches(content, regex);
            string[] numbersAsStrings = matches.Select(m => StringNumberFormating(m.Value)).ToArray();

            return numbersAsStrings;
        }

        private static string StringNumberFormating(string text)
        {
            return text.Trim().Replace(",", ".").Replace(" ", "");
        }
    }
}
