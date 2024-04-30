# WeatherApp

The Weather app provides a weather forecast for the next three days based on the user's zip code. 

#### Project Rationale

This project explores implementing security in a C# application and demonstrates secure coding practices.

#### How to Use

This app runs on Windows and allows you to check the weather for the next 3 days.

Windows [Download](https://onedrive.live.com/download?resid=8EB2B87057C6812D%2111642&authkey=!APFt4ysG85emMHU)

Android [Download](https://onedrive.live.com/download?resid=8EB2B87057C6812D%2111638&authkey=!AI1IEfC9zc1Vudc)

Thank you for downloading the app.

**How to install Windows:** Run WeatherApp_1.0.0.1_x64.msix
If you need to trust the certificate then run WeatherApp_1.0.0.1_x64.cer and add it to your "Trusted Root Certification Authorities" store.

**How to install Android:** Run the APK
You may need to select "more details" and click "install anyway" as the app is not from an app store.

**How to use the app:** Enter your zip code and press the button. There are a limited number of weather requests you can make before you need to wait until it resets.

#### Static Analysis Security Testing (SAST)

I used Microsoft Code Analysis tools in Visual Studio to ensure my code met quality and security standards.

When running the SAST I found I had to fix several variables that could be null as well as remove a using statement and optimize some code.

#### Software Bill of Materials (SBOM)

| Package                               | Version | License | Source                                                                              |
| ------------------------------------- | ------- | ------- | ----------------------------------------------------------------------------------- |
| Microsoft.Maui.Controls               | 8.0.6   | MIT     | [NuGet](https://www.nuget.org/packages/Microsoft.Maui.Controls/8.0.6)               |
| Microsoft.Maui.Controls.Compatibility | 8.0.6   | MIT     | [NuGet](https://www.nuget.org/packages/Microsoft.Maui.Controls.Compatibility/8.0.6) |
| Microsoft.Extensions.Logging.Debug    | 8.0.0   | MIT     | [NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Debug/8.0.0)    |
| Newtonsoft.Json                       | 13.0.3  | MIT     | [NuGet](https://www.nuget.org/packages/Newtonsoft.Json/13.0.3)                      |

#### APIs

| API                                                 | Used For                                                |
| --------------------------------------------------- | ------------------------------------------------------- |
| [tomorrow.io](https://www.tomorrow.io/)             | Gets the weather forecast data                          |
| [openai](https://platform.openai.com/docs/overview) | Text generation to explain today's weather to the user. |

#### Secrets

If you download and want to run the project in your visual studio, you will need to create and fill out a secrets class.

```C#
   static class Secrets
    {

        private static string OpenAIKey = "Your Key";
        private static string WeatherApiKey = "Your Key";
        private static string LogFileDebugPath = "Path you want to store log file";

        public static void InitializeKeys()
        {
            SecureStorage.SetAsync("OpenAIApiKey", OpenAIKey);
            SecureStorage.SetAsync("WeatherApiKey", WeatherApiKey);
            SecureStorage.SetAsync("LogFileDebugPath", LogFileDebugPath);

            Logger.Log("Keys Initialized");
        }


    }
```

Thank you for using my weather app!
