using System;
using System.Collections.Generic;
using System.Text;

namespace SmartNameSearch.Model
{
    public class Person
    {
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public static Person FromCsv(string csvLine)
        {
            var values = csvLine.Split(',');
            var person = new Person
            {
                PersonId = Convert.ToInt32(values[0]),
                FirstName = values[1],
                MiddleName = values[2],
                LastName = values[3]
            };
            return person;
        }
    }
}
