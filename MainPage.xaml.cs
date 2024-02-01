//Program:  WeatherApp
//Author:   Steven Motz
//Date:     1/29/2024
//Purpose:  This program is a weather app that uses the Tomorrow.io API to get the weather for the next 3 days

using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace WeatherApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        btnGetWeather.IsEnabled = false;
    }

    public static readonly HttpClient httpClient = new();

    private void BtnGetWeather_OnClicked(object sender, EventArgs e)
    {
        BtnGetWeather_OnClickedAsync(sender, e);
    }

    private async void BtnGetWeather_OnClickedAsync(object sender, EventArgs e)
    {
        // get the API key from the secure storage and check if it is null
        var APIKey = await SecureStorage.GetAsync("ApiKey");
        if (APIKey == null)
        {
            await DisplayAlert("Error", "API Key Failed to load", "OK");
            return;
        }


        var zipCode = txtZipCode.Text; // get zip from textbox

        // create the url
        var url = "https://api.tomorrow.io/v4/weather/forecast?location=" + zipCode +
                  "&timesteps=1d&units=imperial&apikey=" + APIKey;


        try
        {
            // make the call to the API
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            // gets the weather data from the response
            var json = JObject.Parse(responseBody);
            var weather = json["timelines"]["daily"][0];
            var weatherString = weather.ToString();


            // this line checks if the first day is the current day or not
            var intStartDay = DateTime.Today.ToString("d").Split("/")[1] ==
                              json["timelines"]["daily"][0]["time"].ToString().Split("/")[1] ? 0 : 1;


            //clear the grid
            weatherGrid.Children.Clear();
            weatherGrid.RowDefinitions.Clear();
            weatherGrid.ColumnDefinitions.Clear();

            // add rows to the grid
            for (var j = 0; j < 6; j++)
                weatherGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            for (var k = 0; k < 3; k++)
                weatherGrid.ColumnDefinitions.Add(new ColumnDefinition
                    { Width = new GridLength(1, GridUnitType.Star) });


            // loop through the next 3 days
            for (var i = intStartDay; i < 3 + intStartDay; i++)
            {
                var weatherDataDay = new weatherData();

                // get data from json
                weatherDataDay.minTemp =
                    float.Parse(json["timelines"]["daily"][i]["values"]["temperatureMin"].ToString());
                weatherDataDay.maxTemp =
                    float.Parse(json["timelines"]["daily"][i]["values"]["temperatureMax"].ToString());
                weatherDataDay.avgTemp =
                    float.Parse(json["timelines"]["daily"][i]["values"]["temperatureAvg"].ToString());
                weatherDataDay.precipitation =
                    float.Parse(json["timelines"]["daily"][i]["values"]["precipitationProbabilityAvg"]
                        .ToString());
                weatherDataDay.cloudCover =
                    float.Parse(json["timelines"]["daily"][i]["values"]["cloudCoverAvg"].ToString());
                weatherDataDay.day = int.Parse(json["timelines"]["daily"][i]["time"].ToString().Split("/")[1]);
                weatherDataDay.maxSnowIntensity =
                    float.Parse(json["timelines"]["daily"][i]["values"]["snowIntensityMax"].ToString());
                weatherDataDay.avgSnowAccumulation =
                    float.Parse(json["timelines"]["daily"][i]["values"]["snowAccumulationAvg"].ToString());


                // send data to the labels
                AddWeatherToGrid(weatherDataDay, i - intStartDay);
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(ex.Message);
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    // This method checks the zip code and enables the button.
    private void TxtZipCode_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        btnGetWeather.IsEnabled = false;

        // check if the text is a number
        foreach (var chrTxt in txtZipCode.Text)
            if (!char.IsDigit(chrTxt))
                return;

        // check if the text is 5 characters long
        btnGetWeather.IsEnabled = txtZipCode.Text.Length == 5;
    }

    // if the user presses enter, it will click the button
    private void TxtZipCode_OnCompleted(object? sender, EventArgs e)
    {
        if (btnGetWeather.IsEnabled)
            BtnGetWeather_OnClickedAsync(sender, e);
    }

    private struct weatherData
    {
        public float minTemp;
        public float maxTemp;
        public float avgTemp;
        public float precipitation;
        public float cloudCover;
        public int day;
        public float maxSnowIntensity;
        public float avgSnowAccumulation;
    }

    // This method adds the weather data to the grid using the weatherData struct and the column number.
    private void AddWeatherToGrid(weatherData weatherDataDay, int column)
    {
        var DayLabel = new Label
        {
            Text = "Day: " + weatherDataDay.day,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        Grid.SetColumn(DayLabel, column);
        Grid.SetRow(DayLabel, 0);
        weatherGrid.Children.Add(DayLabel);

        var avgTempLabel = new Label
        {
            Text = "Avg Temp: " + weatherDataDay.avgTemp + "F",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        Grid.SetColumn(avgTempLabel, column);
        Grid.SetRow(avgTempLabel, 2);
        weatherGrid.Children.Add(avgTempLabel);

        var minTempLabel = new Label
        {
            Text = "Min Temp: " + weatherDataDay.minTemp + "F",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        Grid.SetColumn(minTempLabel, column);
        Grid.SetRow(minTempLabel, 3);
        weatherGrid.Children.Add(minTempLabel);

        var maxTempLabel = new Label
        {
            Text = "Max Temp: " + weatherDataDay.maxTemp + "F",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        Grid.SetColumn(maxTempLabel, column);
        Grid.SetRow(maxTempLabel, 4);
        weatherGrid.Children.Add(maxTempLabel);

        var precipitationLabel = new Label
        {
            Text = "Precipitation: " + weatherDataDay.precipitation + "%",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        Grid.SetColumn(precipitationLabel, column);
        Grid.SetRow(precipitationLabel, 5);
        weatherGrid.Children.Add(precipitationLabel);

        var emojiLabel = new Label
        {
            Text = GetWeatherEmoji(weatherDataDay),
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            FontSize = 50
        };
        Grid.SetColumn(emojiLabel, column);
        Grid.SetRow(emojiLabel, 1);
        weatherGrid.Children.Add(emojiLabel);
    }

    // This method returns the emoji for the weather based on the weather data.
    private string GetWeatherEmoji(weatherData weather)
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
}