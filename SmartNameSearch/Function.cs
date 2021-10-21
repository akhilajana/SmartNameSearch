using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using SmartNameSearch.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SmartNameSearch
{
    public class Function
    {
        private string filepath = "C:\\Users\\Akhila Jana\\projects\\personal-projects\\AWSLambda\\people.csv";
        private List<Person> _matchedPeople;

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public List<Person> FunctionHandler(List<Person> peopleToSearch, ILambdaContext context)
        {
            _matchedPeople = new List<Person>();
            try
            {
                if (peopleToSearch.Count == 0)
                {
                    LambdaLogger.Log($"Please input user details to search");
                    return _matchedPeople;
                }

                var peopleInCsvList = File.ReadAllLines(@filepath)
                    .Skip(1)
                    .Select(Person.FromCsv)
                    .OrderBy(p => p.FirstName)
                    .ToList();

                return SearchPeopleInCsv(peopleToSearch, peopleInCsvList);
            }
            catch (Exception e)
            {
                LambdaLogger.Log($"Exception occurred {e.Message}");
                return _matchedPeople;

            }
        }


        private List<Person> SearchPeopleInCsv(List<Person> peopleToSearch, List<Person> peopleInCsvList)
        {
            var matchedFirstNames = peopleInCsvList.Where(person =>
                    peopleToSearch.Any(p =>
                        (!string.IsNullOrEmpty(p.FirstName) && p.FirstName == person.FirstName)))
                .ToList();
            if (matchedFirstNames.Count == 0)
            {
                LambdaLogger.Log($"Record with input first names not found! ");
                return _matchedPeople;
            }

            var matchedLastNames = matchedFirstNames.Where(person =>
                    peopleToSearch.Any(p =>
                        (!string.IsNullOrEmpty(p.LastName) && p.LastName == person.LastName)))
                .ToList();
            if (matchedLastNames.Count == 0)
            {
                LambdaLogger.Log($"Last names did not match! ");
                return matchedFirstNames;
            }

            var matchedMiddleNames = matchedFirstNames.Where(person =>
                    peopleToSearch.Any(p =>
                        (p.MiddleName == person.MiddleName)))
                .ToList();
            if (matchedMiddleNames.Count != 0) return matchedMiddleNames;
            LambdaLogger.Log($"Middle names did not match! ");
            return matchedLastNames;
        }
    }
}
