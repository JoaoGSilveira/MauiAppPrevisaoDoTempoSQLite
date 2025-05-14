using MauiAppTempoAgora.Models;
using Newtonsoft.Json.Linq;

namespace MauiAppTempoAgora.Services
{
    public class DataService
    {
        public static async Task<Tempo?> GetPrevisao(string cidade)
        {
            Tempo? t = null;

            string chave = "6135072afe7f6cec1537d5cb08a5a1a2";

            string url = $"https://api.openweathermap.org/data/2.5/weather?" +
                $"q={cidade}&units=metric&appid={chave}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage resp = await client.GetAsync(url);

                if(resp.IsSuccessStatusCode)
                {
                    string json = await resp.Content.ReadAsStringAsync();

                    var rascunho = JObject.Parse(json);

                    DateTime time = new();
                    DateTime sunrise = time.AddSeconds((double)rascunho["sys"]["sunrise"]).ToLocalTime();
                    DateTime sunset = time.AddSeconds((double)rascunho["sys"]["sunset"]).ToLocalTime();

                    t = new()
                    {
                        lat = Convert.ToDouble(rascunho["coord"]["lat"]),
                        lon = Convert.ToDouble(rascunho["coord"]["lon"]),
                        description = rascunho["weather"][0]["description"].ToString(),
                        main = rascunho["weather"][0]["main"].ToString(),
                        temp_min = Convert.ToDouble(rascunho["main"]["temp_min"]),
                        temp_max = Convert.ToDouble(rascunho["main"]["temp_max"]),
                        speed = Convert.ToDouble(rascunho["wind"]["speed"]),
                        visibility = (int)rascunho["visibility"],
                        sunrise = sunrise.ToString(),
                        sunset = sunset.ToString(),
                    };
                }
            }

            return t;
        }
    }
}
