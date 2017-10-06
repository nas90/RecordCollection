using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Objects;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using RecordCollection.Web.Data;
using RecordCollection.Web.Models;
using RecordCollection.Web.Models.HomeViewModels;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace RecordCollection.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly LastFM_Credentials _credentials;

        public HomeController(ApplicationDbContext dbContext, IOptions<LastFM_Credentials> settingsOptions)
        {
            DbContext = dbContext;
            _credentials = settingsOptions.Value;
        }

        public ApplicationDbContext DbContext { get; }
        
        public async Task<ViewResult> Index(string searchString)
        {
            HomeViewModel l_model = new HomeViewModel();

            var res = await LoadRecords();

            l_model.Albums = res.ToList();

            if (!String.IsNullOrEmpty(searchString))
            {
                l_model.Albums = l_model.Albums.Where(s => s.LastAlbum.Name.ToLower().Contains(searchString.ToLower())
                                       || s.LastAlbum.ArtistName.ToLower().Contains(searchString.ToLower())).ToList();
            }

            ViewData["Collections"] = DbContext.Collections;
            ViewData["Albums"] = DbContext.Albums;
            
            return View(l_model);
        }
        
        public async Task<IActionResult> Add(string searchArtist, string searchTitle)
        {
            ViewData["Message"] = "Add new record.";

            List<LastArtist> artists = new List<LastArtist>();
            List<LastAlbum> albums = new List<LastAlbum>();

            LastfmClient lfmClient = new LastfmClient(_credentials.LastFM_ApiKey, _credentials.LastFM_SecretKey);

            if (!string.IsNullOrEmpty(searchTitle))
            {
                var resAlbums = await lfmClient.Album.SearchAsync(searchTitle);
                albums = resAlbums.ToList();

            }

            if (!string.IsNullOrEmpty(searchArtist))
            {
                var artistAlbums = await lfmClient.Artist.GetTopAlbumsAsync(searchArtist);
                albums.AddRange(artistAlbums);
            }

            List<Album> searchResults = new List<Album>();

            foreach (var album in albums)
            {
                searchResults.Add(new Album()
                {
                    LastAlbum = album
                });
            }

            return View(searchResults);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(
            string id,
            CancellationToken requestAborted)
        {
            ViewData["Message"] = "Delete record.";
            
            Album album = DbContext.Albums.Where(p => p.ID.ToString() == id).SingleOrDefault();

            DbContext.Albums.Remove(album);
            //DbContext.SaveChanges();

            return Json(album.ID);
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

        public async Task<IQueryable<Album>> LoadRecords()
        {
            var client = new LastfmClient(_credentials.LastFM_ApiKey, _credentials.LastFM_SecretKey);

            var collection = DbContext.Collections.Single(p => p.UserID == DbContext.Users.Single().Id);

            var albums = DbContext.Albums
                            .Where(p => p.CollectionID == collection.ID);

            foreach (var album in albums)
            {
                var response = await client.Album.GetInfoByMbidAsync(album.LastFM_ID);
                LastAlbum lastAlbum = response.Content;

                album.LastAlbum = lastAlbum;
                    //ImgUrl = album.Images.Medium.AbsoluteUri,
                    //LastFM_ID = album.Mbid,
                    //Name = album.Name,
                    //ArtistName = album.ArtistName,
                    //CollectionID = collection.ID,
                    //ID = 
                //});
            }

            return albums;
        } 
    }
}
