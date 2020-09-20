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

        public AirportName Name { get; set; }

        const string BASE_URL = "https://avwx.rest/api/";
       

        public WeatherController(IHttpClientFactory clientFactory, AppConfiguration config)
        {
            _clientFactory = clientFactory;
            _config = config;
           
        }

        [HttpGet()]
        public IActionResult Index()
        {

            return View();
        }

        [HttpPost()]
        public async Task<IActionResult> Index(AirportCode model)
        {

            var airportCode = model.code;

            var message = new HttpRequestMessage();             // instantiate HttpRequestMessage object. Will eventually hold all parts of our GET request
            var message2 = new HttpRequestMessage();

            message.Method = HttpMethod.Get;
            message2.Method = HttpMethod.Get;

            message.RequestUri = new Uri($"{BASE_URL}metar/{airportCode}?token={_config.API_KEY}");
            message2.RequestUri = new Uri($"{BASE_URL}station/{airportCode}?token={_config.API_KEY}");

            var client = _clientFactory.CreateClient();         // Using injected clientFactory, create new local object
            var client2 = _clientFactory.CreateClient();

            var response = await client.SendAsync(message);     // Async sending of GET request with response stored in local object
            var response2 = await client2.SendAsync(message2);

            if (response.IsSuccessStatusCode)                   // If 200-Range Response
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();         // 'using' statement to preserve/garbage-collect resources. Read stream of JSON response into local object
                METARs = await JsonSerializer.DeserializeAsync<METAR>(responseStream);         // Deserialize response stream into our METARs property of type METAR. 
            }                                                                                  // ..If this were more than 1 property on the response, use IEnumerable & for-loop through on the View
            else
            {
                METARs = null;                                                                  // Set Property to null if failed call
            }

            if (response2.IsSuccessStatusCode)
            {
                using var responseStream2 = await response2.Content.ReadAsStreamAsync();
                Name = await JsonSerializer.DeserializeAsync<AirportName>(responseStream2);
            }
            else
            {
                Name = null;
            }

            ViewBag.metarRes = METARs.flight_rules;
            ViewBag.airportName = Name.name;

            Console.WriteLine($"Response from API after POST: {METARs.flight_rules}");
            return View();
           //return RedirectToAction(nameof(Display));                                                               // Return the view passing in the field parsed from response
            
        }

        //public IActionResult Display()
        //{
        //    ViewBag.Message = "Dispaly View Rendered";
        //    Console.WriteLine($"METARs at Display View render: {METARs.flight_rules}");
        //    return View(METARs);
        //}

        //public async Task<IActionResult> Display()
        //{
        //    var message = new HttpRequestMessage();             // instantiate HttpRequestMessage object. Will eventually hold all parts of our GET request
        //    message.Method = HttpMethod.Get;
        //    message.RequestUri = new Uri($"{BASE_URL}/{_airportCode}?token={_config.API_KEY}");

        //    var client = _clientFactory.CreateClient();         // Using injected clientFactory, create new local object

        //    var response = await client.SendAsync(message);     // Async sending of GET request with response stored in local object

        //    if (response.IsSuccessStatusCode)                   // If 200-Range Response
        //    {
        //        using var responseStream = await response.Content.ReadAsStreamAsync();         // 'using' statement to preserve/garbage-collect resources. Read stream of JSON response into local object
        //        METARs = await JsonSerializer.DeserializeAsync<METAR>(responseStream);         // Deserialize response stream into our METARs property of type METAR. 
        //    }                                                                                  // ..If this were more than 1 property on the response, use IEnumerable & for-loop through on the View
        //    else
        //    {
        //        METARs = null;                                                                  // Set Property to null if failed call
        //    }
        //    Console.WriteLine($"Response from API: {response.Content}");
        //    return View(METARs);                                                                // Return the view passing in the field parsed from response                   
        //}

        
    }
}
