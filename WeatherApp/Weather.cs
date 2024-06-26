﻿using Newtonsoft.Json.Linq;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("WeatherApp.nUnitTests")]

namespace WeatherApp
{
    public static class Weather
    {
        public struct weatherData
        {
            public float minTemp;
            public float maxTemp;
            public float avgTemp;
            public float precipitation;
            public float cloudCover;
            public int day;
            public float maxSnowIntensity;
            public float avgSnowAccumulation;
            public float avgWindSpeed;

            public readonly override string ToString()
            {
                return "Min Temp: " + minTemp + " Max Temp: " + maxTemp + " Avg Temp: " + avgTemp + " Precipitation: " +
                       precipitation + " Cloud Cover: " + cloudCover + " Day: " + day + " Max Snow Intensity: " +
                       maxSnowIntensity + " Avg Snow Accumulation: " + avgSnowAccumulation + " Avg Wind Speed: " +
                       avgWindSpeed;
            }
        }

        public static string GetWeatherEmoji(weatherData weather)
        {
            switch (weather)
            {
                case weatherData weatherData
                    when weatherData.maxSnowIntensity > 0 || weatherData.avgSnowAccumulation > 0:
                    return "🌨️";
                    break;
                case weatherData weatherData when weatherData.precipitation > 50:
                    return weather.cloudCover > 50 ? "🌧️" : "🌦️";
                    break;
                case weatherData weatherData when weatherData.cloudCover > 70:
                    return "☁️";
                case weatherData weatherData when weatherData.avgTemp > 80:
                    return "☀️";
                    break;
                case weatherData weatherData when weatherData.avgTemp < 40:
                    return "❄️";
                    break;
                default:
                    return "🌤️";
                    break;
            }
        }

        public static readonly HttpClient httpClient = new();

        public static async Task<List<weatherData>> GetWeatherData(string zipCode)
         {
             List<weatherData> weatherDataDays = new List<weatherData>();

             if (!Validation.IsValidZipCode(zipCode))
             {
                 return weatherDataDays;
             }

             var WeatherAPIKey = await SecureStorage.GetAsync("WeatherApiKey");
            var url = "https://api.tomorrow.io/v4/weather/forecast?location=" + zipCode +
                      "&timesteps=1d&units=imperial&apikey=" + WeatherAPIKey;

            try
            {
                // make the call to the API
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                var weather = JObject.Parse(responseBody);

                if (weather["timelines"]["daily"] == null)
                {
                    return weatherDataDays;
                }

                // this line checks if the first day is the current day or not
                var intStartDay = DateTime.Today.ToString("d").Split("/")[1] ==
                                  weather["timelines"]?["daily"]?[0]?["time"]?.ToString().Split("/")[1] ? 0 : 1;

                for (var i = intStartDay; i < 3 + intStartDay; i++)
                {
                    var weatherDataDay = new weatherData();
                    JToken? weatherData = weather["timelines"]?["daily"]?[i]?["values"];
                    try
                    {
                        weatherDataDay.minTemp = float.Parse(weatherData?["temperatureMin"]?.ToString());
                        weatherDataDay.maxTemp = float.Parse(weatherData?["temperatureMax"]?.ToString());
                        weatherDataDay.avgTemp = float.Parse(weatherData?["temperatureAvg"]?.ToString());
                        weatherDataDay.precipitation =
                            float.Parse(weatherData?["precipitationProbabilityAvg"]?.ToString());
                        weatherDataDay.cloudCover = float.Parse(weatherData?["cloudCoverAvg"]?.ToString());
                        weatherDataDay.day = int.Parse(weather["timelines"]["daily"][i]["time"].ToString().Split("/")[1]);
                        weatherDataDay.maxSnowIntensity =
                            weatherDataDay.maxSnowIntensity = float.Parse(weatherData?["snowIntensityMax"]?.ToString());
                        weatherDataDay.avgSnowAccumulation =
                            float.Parse(weatherData?["snowAccumulationAvg"]?.ToString());
                        weatherDataDay.avgWindSpeed = float.Parse(weatherData?["windSpeedAvg"]?.ToString());
                    }
                    catch(Exception ex)
                    {
                        Logger.Log("Weather API Error: " + ex);
                    }

                    Logger.Log("Day "+ (i - intStartDay) +" weather data: " + weatherDataDay.ToString());
                    weatherDataDays.Add(weatherDataDay);

                    // https://docs.tomorrow.io/recipes
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Weather API Error: " + ex);
            }

            return weatherDataDays;
        }
    }
}
