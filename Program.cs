using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http.Json;

namespace Test
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            EnterAPI enter = await auth("https://belorusneft.by/identity/connect/token");

            Test_1 test = new Test_1
            {
                startDate = "2023-01-01",
                endDate = "2023-01-31",
                cardNumber = 0,
                subDivisnNumber = -1,
                flChoice = 1
            };

            API api = await getAPI("https://ssl.beloil.by/rcp/i/api/v2/Contract/operational", enter.access_token, test);

            Console.ReadKey();
        }

        static async Task<EnterAPI> auth(string url)
        {
            IEnumerable<KeyValuePair<string, string>> queries = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("client_id", "rcp.web"),
                new KeyValuePair<string, string>("client_secret", "secret"),
                new KeyValuePair<string, string>("scope", "rcp.api"),
                new KeyValuePair<string, string>("username", "650.6409982"),
                new KeyValuePair<string, string>("password", "aksa")
            };

            HttpContent q = new FormUrlEncodedContent(queries);
            
            using(HttpClient client = new HttpClient())
            {
                using(HttpResponseMessage response = await client.PostAsync(url, q))
                {
                    using(HttpContent content = response.Content)
                    {
                        string mycontent = await content.ReadAsStringAsync();
                        Console.WriteLine(mycontent);

                        EnterAPI enter = new EnterAPI();
                        enter = JsonSerializer.Deserialize<EnterAPI>(mycontent);

                        return enter;
                    }
                }
            }
        }

        static async Task<API> getAPI(string url, string token, Test_1 test_1)
        {

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                JsonContent content = JsonContent.Create(test_1);

                var response = await client.PostAsync(url, content);
                
                API check = await response.Content.ReadFromJsonAsync<API>();

                return check;
            }
        }
    }
}
