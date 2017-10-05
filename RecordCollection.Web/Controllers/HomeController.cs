using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Objects;
using System.Configuration;
using RecordCollection.Web.Models.HomeViewModels;
using Microsoft.Extensions.Options;

namespace RecordCollection.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly LastFM_Model _credentials;

        public HomeController(IOptions<LastFM_Model> settingsOptions)
        {
            _credentials = settingsOptions.Value;
        }

        public IActionResult Index()
        {
            LoadRecords();
            
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public async void LoadRecords()
        {
            var client = new LastfmClient(_credentials.ApiKey, _credentials.SecretKey);

            var response = await client.Album.GetInfoAsync("Grimes", "Visions");

            LastAlbum visions = response.Content;

            //ViewData["Records"] = visions.ArtistName + " " + visions.Name + " <img src='" + visions.Images.Medium.AbsoluteUri + "' />";
        } 
    }
}
