using APA_Library;
using APA_Library.Models;
using System;
using Xunit;

namespace APA_Library.Tests
{
    public class UnitTest1
    {
        [Theory]
        [InlineData("Kraków")]
        [InlineData("Sydney")]
        public void LoadCitiesTest(string country)
        {
            if (country is null)
                throw new ArgumentNullException();
        }

        [Theory]
        [InlineData("Poland", "Warsaw")]
        [InlineData("Australia", "Vancouver")]
        public void LoadMapLocationTest(string country, string city)
        {
            if (country is null && city is null)
            {
                throw new ArgumentNullException();
            }
        }
    }



}