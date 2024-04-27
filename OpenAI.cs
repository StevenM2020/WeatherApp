using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using weatherData = WeatherApp.Weather.weatherData;

namespace WeatherApp
{
    internal class OpenAI
    {
        public static async Task<String> SendOpenAI(List<weatherData> weatherDataDays)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
                var openAIAPIKey = await SecureStorage.GetAsync("OpenAIApiKey");
                request.Headers.Add("Authorization", $"Bearer {openAIAPIKey}");

                // create the message to send to the AI
                string model = "gpt-3.5-turbo";
                string systemContent =
                    $"You give weather recommendations but the response must be between 250 and 300 characters. Dont recommend rain gear unless you think its going to rain or its high enough to be cautious. Only Recommend things that are useful and be funny.";
                string userContent = $"The weather for today shows an average temperature of  {weatherDataDays[0].avgTemp}°F with a maximum of {weatherDataDays[0].maxTemp}°F and a minimum of {weatherDataDays[0].minTemp}°F. Cloud cover is at {weatherDataDays[0].cloudCover}%, with precipitation at {weatherDataDays[0].precipitation}% and snow intensity will be {weatherDataDays[0].maxSnowIntensity}%. Wind speeds average around {weatherDataDays[0].avgWindSpeed} mph.";
                var message = new
                {
                    model = model,
                    max_tokens = 150,
                    messages = new[]
                    {
                    new { role = "system", content = systemContent },
                    new { role = "user", content = userContent}
                }
                };
                string contentString = JsonConvert.SerializeObject(message, Formatting.Indented);
                var content = new StringContent(contentString, Encoding.UTF8, "application/json");
                request.Content = content;

                // send
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                // get
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseContent);

                // parse and display
                var responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);
                var responseText = responseObject.choices[0].message.content.ToString();
                return responseText;

                // https://platform.openai.com/docs/guides/text-generation/reproducible-outputs
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.Message);
                return "AI Weather Recommendations Failed";
            }
        }
    }
}
