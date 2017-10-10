using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IF.Lastfm.Core.Objects;
using IF.Lastfm.Core.Api;
using RecordCollection.Web.Models.HomeViewModels;
using RecordCollection.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using RecordCollection.Web.Models;
using System.Security.Claims;
using System;

namespace RecordCollection.Web.Controllers
{
    [Authorize]
    public class BrowseController : Controller
    {
        private readonly LastFM_Credentials _credentials;

        public BrowseController(ApplicationDbContext dbContext, IOptions<LastFM_Credentials> settingsOptions)
        {
            DbContext = dbContext;
            _credentials = settingsOptions.Value;
        }

        public ApplicationDbContext DbContext { get; }

        public async Task<IActionResult> Index(string searchArtist, string searchTitle)
        {
            ViewData["Message"] = "Add new record.";

            List<LastArtist> artists = new List<LastArtist>();
            List<LastAlbum> albums = new List<LastAlbum>();

            LastfmClient lfmClient = new LastfmClient(_credentials.LastFM_ApiKey, _credentials.LastFM_SecretKey);

            if (!string.IsNullOrEmpty(searchTitle))
            {
                var resAlbums = await lfmClient.Album.SearchAsync(searchTitle);
                albums.AddRange(resAlbums);

            }

            if (!string.IsNullOrEmpty(searchArtist))
            {
                var artistAlbums = await lfmClient.Artist.GetTopAlbumsAsync(searchArtist);
                albums.AddRange(artistAlbums);
            }

            List<Album> searchResults = new List<Album>();

            string userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            foreach (var album in albums)
            {
                if (!string.IsNullOrEmpty(album.Mbid))
                {
                    Collection l_collection = DbContext.Collections.FirstOrDefault(p => p.UserID == userID);

                    Album albumToAdd = new Album();
                    albumToAdd.LastAlbum = album;

                    foreach (var test in DbContext.Albums.Where(p => p.CollectionID == l_collection.ID))
                    {
                        if (test.LastFM_ID == album.Mbid)
                        {
                            albumToAdd.AlbumInCollection = true;
                            break;
                        }
                    }
                    searchResults.Add(albumToAdd);
                }
            }
            return View(searchResults);
        }

        public JsonResult Add(string id)
        {
            string userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Album l_album = new Album()
            {
                LastFM_ID = id,
                CollectionID = DbContext.Collections.FirstOrDefault(p => p.UserID == userID).ID
            };

            DbContext.Albums.Add(l_album);
            DbContext.SaveChanges();

            return Json(l_album.LastFM_ID);
        }
    }
}