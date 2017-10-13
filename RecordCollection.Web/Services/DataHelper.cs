using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Objects;
using Microsoft.Extensions.Options;
using RecordCollection.Web.Data;
using RecordCollection.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecordCollection.Web.Services
{
    public class DataHelper
    {
        private readonly LastFM_Credentials _credentials;
        private readonly ApplicationDbContext DbContext;

        public DataHelper(ApplicationDbContext dbContext, IOptions<LastFM_Credentials> settingsOptions)
        {
            DbContext = dbContext;
            _credentials = settingsOptions.Value;
        }

        internal IQueryable<Album> LoadUserRecords(string userID)
        {
           var collection = DbContext.Collections.FirstOrDefault(p => p.UserID == userID);

            var albums = DbContext.Albums
                            .Where(p => p.CollectionID == collection.ID);

            return albums;
        }

        internal async Task<IQueryable<Album>> FetchAlbumData(IQueryable<Album> albums) {

            var client = new LastfmClient(_credentials.LastFM_ApiKey, _credentials.LastFM_SecretKey);

            foreach (var album in albums)
            {
                var response = await client.Album.GetInfoByMbidAsync(album.LastFM_ID);
                LastAlbum lastAlbum = response.Content;

                album.LastAlbum = lastAlbum;
            }
            return albums;
        }

        internal bool DeleteAlbum(int recordID)
        {
            try
            {
                Album album = DbContext.Albums.Where(p => p.ID == recordID).SingleOrDefault();

                if (album != null)
                {
                    DbContext.Albums.Remove(album);
                    DbContext.SaveChanges();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        internal async Task<List<Album>> SearchAlbums(string userID, string searchArtist, string searchTitle, int page)
        {
            List<LastArtist> artists = new List<LastArtist>();
            List<LastAlbum> albums = new List<LastAlbum>();

            LastfmClient lfmClient = new LastfmClient(_credentials.LastFM_ApiKey, _credentials.LastFM_SecretKey);

            if (!string.IsNullOrEmpty(searchTitle))
            {
                var resAlbums = await lfmClient.Album.SearchAsync(searchTitle, page, 10);
                albums.AddRange(resAlbums);

            }

            if (!string.IsNullOrEmpty(searchArtist))
            {
                var artistAlbums = await lfmClient.Artist.GetTopAlbumsAsync(searchArtist, false, page, 10);
                albums.AddRange(artistAlbums);
            }

            List<Album> searchResults = new List<Album>();

            foreach (var album in albums.OrderBy(p => p.PlayCount))
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

            return searchResults;
        }

        internal bool AddAlbum(string userID, string recordID)
        {
            try
            {
                Album l_album = new Album()
                {
                    LastFM_ID = recordID,
                    CollectionID = DbContext.Collections.FirstOrDefault(p => p.UserID == userID).ID
                };
                DbContext.Albums.Add(l_album);
                DbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
