using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kursovaya
{
    class ApiConnection
    {
        private HttpClient client = new HttpClient();
        private readonly string apiKey = "7235fb8523a840e9979bd25faff57198";

        private readonly string url = "https://api.football-data.org/v4/matches";
        
        public dynamic GetMatches(string parameters)
        {
            client.DefaultRequestHeaders.Add("X-Auth-Token", apiKey);
            
            HttpResponseMessage response = client.GetAsync(url + parameters).Result;
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }

            dynamic result = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);

            return result;
        }
    }
}
