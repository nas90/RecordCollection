using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Objects;
using System.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using RecordCollection.Web.Data;
using RecordCollection.Web.Models;
using RecordCollection.Web.Models.HomeViewModels;

namespace RecordCollection.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly LastFM_Model _credentials;

        public HomeController(ApplicationDbContext dbContext, IOptions<LastFM_Model> settingsOptions)
        {
            DbContext = dbContext;
            _credentials = settingsOptions.Value;
        }

        public ApplicationDbContext DbContext { get; }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            HomeViewModel l_model = new HomeViewModel();

            var res = await LoadRecords();

            l_model.Albums = res;

            ViewData["Collections"] = DbContext.Collections;
            ViewData["Albums"] = DbContext.Albums;
            
            return View(l_model);
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

        public async Task<List<Album>> LoadRecords()
        {
            var client = new LastfmClient(_credentials.ApiKey, _credentials.SecretKey);

            var collection = DbContext.Collections.Single(p => p.UserID == DbContext.Users.Single().Id);
            var res = await client.Album.GetInfoAsync("Grimes", "Visions");
            var list = DbContext.Albums
                                    .Where(p => p.CollectionID == collection.ID)
                                    .Select(p => p.LastFM_ID);

            var albums = new List<Album>();

            foreach (var l_id in list)
            {
                var response = await client.Album.GetInfoByMbidAsync(res.Content.Mbid);
                LastAlbum l_album = response.Content;

                albums.Add(new Album() {
                    ImgUrl = l_album.Images.Medium.AbsoluteUri,
                    LastFM_ID = l_album.Id,
                    Name = l_album.Name,
                    ArtistName = l_album.ArtistName
                });
            }

            return albums;
        } 
    }
}
