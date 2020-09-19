using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using isIFR.Models;
using Microsoft.AspNetCore.Mvc;

using System.Text.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace isIFR.Controllers
{
    public class WeatherController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        private readonly AppConfiguration _config;

        public METAR METARs { get; set; }


        const string BASE_URL = "https://avwx.rest/api/metar/";
       

        public WeatherController(IHttpClientFactory clientFactory, AppConfiguration config)
        {
            _clientFactory = clientFactory;
            _config = config;
           
        }

        public async Task<IActionResult> Display()
        {
            var message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            message.RequestUri = new Uri($"{BASE_URL}/KMCN?token={_config.API_KEY}");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(message);

            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                METARs = await JsonSerializer.DeserializeAsync<METAR>(responseStream);
            }
            else
            {
                METARs = null;
            }
            Console.WriteLine($"Response from API: {response.Content}");
            return View(METARs);
        }

        public IActionResult Index()
        {
            
            return View();
        }
    }
}
