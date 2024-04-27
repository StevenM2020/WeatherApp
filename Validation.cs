using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using logger = WeatherApp.Logger;

namespace WeatherApp
{
    internal class Validation
    {
        private static readonly string ZipPattern = @"^\d{5}$";

        public static bool IsValidZipCode(string zipCode)
        {
            return Regex.IsMatch(zipCode, ZipPattern);
        }

        // checks the list range and the values of the weather data
        public static bool IsValidWeather(List<Weather.weatherData> weatherDataDays)
        {
            if (weatherDataDays.Count <= 0 || weatherDataDays.Count > 3)
                return false;

            foreach (var weatherData in weatherDataDays)
            {
                if (weatherData.avgTemp < -100 || weatherData.avgTemp > 150)
                    return false;
                if (weatherData.maxTemp < -100 || weatherData.maxTemp > 150)
                    return false;
                if (weatherData.minTemp < -100 || weatherData.minTemp > 150)
                    return false;
                if (weatherData.precipitation < 0 || weatherData.precipitation > 100)
                    return false;
                if (weatherData.cloudCover < 0 || weatherData.cloudCover > 100)
                    return false;
                if (weatherData.maxSnowIntensity < 0 || weatherData.maxSnowIntensity > 100)
                    return false;
                if (weatherData.avgSnowAccumulation < 0 || weatherData.avgSnowAccumulation > 100)
                    return false;
                if (weatherData.avgWindSpeed < 0 || weatherData.avgWindSpeed > 100)
                    return false;
            }
            logger.Log("Weather data is valid");
            return true;
        }

        public static bool IsValidAIResponse(string response)
        {
            bool aiResponse = response.Length >= 200 && response.Length <= 350;
            logger.Log(aiResponse ? "AI Response is valid" : "AI Response is invalid");
            return aiResponse;

        }
    }
}
