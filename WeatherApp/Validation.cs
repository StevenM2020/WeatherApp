using System.Text.RegularExpressions;
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("WeatherApp.nUnitTests")]
namespace WeatherApp
{
    public class Validation
    {
        private static readonly string ZipPattern = @"^\d{5}$";

        public static bool IsValidZipCode(string zipCode)
        {
            bool isValid = Regex.IsMatch(zipCode, ZipPattern);
            Logger.Log(isValid ? "Zip code is valid" : "Zip code is invalid");
            return isValid;
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
            Logger.Log("Weather data is valid");
            return true;
        }

        public static bool IsValidAIResponse(string response)
        {
            bool aiResponse = response.Length >= 200 && response.Length <= 350;
            Logger.Log(aiResponse ? "AI Response is valid" : "AI Response is invalid");
            return aiResponse;

        }
    }
}
