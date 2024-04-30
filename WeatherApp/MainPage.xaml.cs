//Program:  WeatherApp
//Author:   Steven Motz
//Date:     2/1/2024
//Purpose:  This program is a weather app that uses the Tomorrow.io API to get the weather for the next 3 days


using weatherData = WeatherApp.Weather.weatherData;

namespace WeatherApp;

public partial class MainPage : ContentPage
{

    bool blnSendingRequest = false;

    List<weatherData> weatherDataDays = new List<weatherData>();
    public MainPage()
    {
        InitializeComponent();
        btnGetWeather.IsEnabled = false;
        Logger.Log("Main page loaded");
    }

    private async void BtnGetWeather_OnClickedAsync(object sender, EventArgs e)
    {
        Logger.Log("Get Weather Button Clicked, zipcode: " + txtZipCode.Text);
        blnSendingRequest = true;
        btnGetWeather.IsEnabled = false;
        weatherDataDays.Clear();

        // TODO: move to validation later
        if (await SecureStorage.GetAsync("WeatherApiKey") == null)
        {
            Logger.Log("Weather API Key Failed to load");
            await DisplayAlert("Error", "Weather API Key Failed to load", "OK");
            blnSendingRequest = false;
            return;
        }

        try
        {
            weatherDataDays = await Weather.GetWeatherData(txtZipCode.Text);
            if (!Validation.IsValidWeather(weatherDataDays))
            {
                Logger.Log("Invalid Weather Data");
                await DisplayAlert("Error", "Invalid Weather Data", "OK");
                blnSendingRequest = false;
                return;
            }

            FillGrid();
            lblAI.Text = "AI Weather Recommendations Loading...";

            if (await SecureStorage.GetAsync("OpenAIApiKey") == null)
            {
                await DisplayAlert("Error", "OpenAI API Key Failed to load", "OK");
                blnSendingRequest = false;
                lblAI.Text = "AI Weather Recommendations Failed";
                return;
            }

            string strAI = await OpenAI.SendOpenAI(weatherDataDays);
            lblAI.Text = Validation.IsValidAIResponse(strAI) ? strAI : "AI Weather Recommendations Failed";
            Logger.Log("AI Response shown");
        }
        catch (Exception ex)
        {
            Logger.Log("Error :" + ex);
            await DisplayAlert("Error", "An error occured, please try again later.", "OK");
            blnSendingRequest = false;
        }
        blnSendingRequest = false;
    }


    // This method checks the zip code and enables the button.
    private void TxtZipCode_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        btnGetWeather.IsEnabled = false;
        if(blnSendingRequest) return;
        if(!Validation.IsValidZipCode(txtZipCode.Text)) return;
        btnGetWeather.IsEnabled = txtZipCode.Text.Length == 5;
    }

    // if the user presses enter, it will click the button
    private void TxtZipCode_OnCompleted(object sender, EventArgs e)
    {
        if (btnGetWeather.IsEnabled)
            BtnGetWeather_OnClickedAsync(sender, e);
    }

    private void FillGrid()
    {
        //clear the grid
        weatherGrid.Children.Clear();
        weatherGrid.RowDefinitions.Clear();
        weatherGrid.ColumnDefinitions.Clear();

        // add rows to the grid
        for (var j = 0; j < 7; j++)
            weatherGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        for (var k = 0; k < 3; k++)
            weatherGrid.ColumnDefinitions.Add(new ColumnDefinition
                { Width = new GridLength(1, GridUnitType.Star) });

        for (var i = 0; i < 3; i++)
            AddWeatherToGrid(weatherDataDays[i], i);
        Logger.Log("Weather Grid Filled");
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
            Text = "Avg Temp: " + weatherDataDay.avgTemp + "°F",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        Grid.SetColumn(avgTempLabel, column);
        Grid.SetRow(avgTempLabel, 2);
        weatherGrid.Children.Add(avgTempLabel);

        var minTempLabel = new Label
        {
            Text = "Min Temp: " + weatherDataDay.minTemp + "°F",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        Grid.SetColumn(minTempLabel, column);
        Grid.SetRow(minTempLabel, 3);
        weatherGrid.Children.Add(minTempLabel);

        var maxTempLabel = new Label
        {
            Text = "Max Temp: " + weatherDataDay.maxTemp + "°F",
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

        var windSpeedLabel = new Label
        {
            Text = "WS: " + weatherDataDay.avgWindSpeed + "mph",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        Grid.SetColumn(windSpeedLabel, column);
        Grid.SetRow(windSpeedLabel, 6);
        weatherGrid.Children.Add(windSpeedLabel);

        var emojiLabel = new Label
        {
            Text = Weather.GetWeatherEmoji(weatherDataDay),
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            FontSize = 50
        };
        Grid.SetColumn(emojiLabel, column);
        Grid.SetRow(emojiLabel, 1);
        weatherGrid.Children.Add(emojiLabel);
    }
}