using System.Diagnostics;
using NUnit.Framework;
using WeatherApp;
namespace WeatherApp.nUnitTests
{
    public class Tests
    {

        [SetUp]
        public void Setup()
        {
           
        }

        [TestCase("17701", true)]
        [TestCase("1770", false)]
        [TestCase("177010", false)]
        [TestCase(" 7701", false)]
        [TestCase("1770-", false)]
        [TestCase("1770/", false)]
        [TestCase("1770*", false)]
        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        public void IsValidZipCode_Test(string zip, bool expected)
        {
            bool val = Validation.IsValidZipCode(zip);
            Assert.That(val, Is.EqualTo(expected));
        }

        [TestCase("17701")]
        [TestCase(null)]
        [TestCase(" ")]
        [TestCase("1701")]
        [TestCase("177A1")]
        public void IsValidZipCodeErrors_Test(string zip)
        {
            try
            {
                bool val = Validation.IsValidZipCode(zip);
            }
            catch(Exception e)
            {
                Assert.Fail(e.Message);
            }
            Assert.Pass();
        }

        [TestCase("17701", true)]
        [TestCase("1770", false)]
        [TestCase("177010", false)]
        [TestCase(" 7701", false)]
        [TestCase("1770-", false)]
        [TestCase("1770/", false)]
        [TestCase("1770*", false)]
        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        public async Task GetWeatherData_Test(string zip, bool expected)
        {
            bool result = true;
            List<Weather.weatherData> weatherData = await Weather.GetWeatherData(zip);

            if (weatherData.Count < 3)
            {
                result = false;
            }

            foreach (var day in weatherData)
            {
              result = IsValidWeather(day);
              if (!result)
                  break;
            }
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase("17701")]
        [TestCase(null)]
        [TestCase(" ")]
        [TestCase("1701")]
        [TestCase("177A1")]
        public async Task GetWeatherDataErrors_Test(string zip)
        {
            try
            {
                List<Weather.weatherData> weatherData = await Weather.GetWeatherData(zip);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            Assert.Pass();
        }

        private bool IsValidWeather(Weather.weatherData day)
        {
            if (day.avgTemp < -100 || day.avgTemp > 150)
                return false;

            if (day.maxTemp < -100 || day.maxTemp > 150)
                return false;

            if (day.minTemp < -100 || day.minTemp > 150)
                return false;

            if (day.precipitation < 0 || day.precipitation > 100)
                return false;

            if (day.cloudCover < 0 || day.cloudCover > 100)
                return false;

            if (day.maxSnowIntensity < 0 || day.maxSnowIntensity > 100)
                return false;

            if (day.avgSnowAccumulation < 0 || day.avgSnowAccumulation > 100)
                return false;

            if (day.avgWindSpeed < 0 || day.avgWindSpeed > 100)
                return false;

            return true;
        }

    }
}