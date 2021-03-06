using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IF.Lastfm.Core.Objects;
using IF.Lastfm.Core.Api;
using RecordCollection.Web.Models;
using RecordCollection.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System;
using RecordCollection.Web.Services;

namespace RecordCollection.Web.Controllers
{
    [Authorize]
    public class BrowseController : Controller
    {
        private readonly DataHelper _dataHelper;

        public BrowseController(ApplicationDbContext dbContext, IOptions<LastFM_Credentials> settingsOptions)
        {
            _dataHelper = new DataHelper(dbContext, settingsOptions);
        }


        public async Task<IActionResult> Index(string searchArtist, string searchTitle, string currentArtist, string currentTitle, int? page)
        {
            if (searchArtist != null || searchTitle != null)
            {
                page = 1;
            }
            else
            {
                searchArtist = currentArtist;
                searchTitle = currentTitle;
            }

            ViewData["Message"] = "Add new record.";
            ViewBag.CurrentArtist = searchArtist;
            ViewBag.CurrentTitle = searchTitle;


            string userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var searchResults = await _dataHelper.SearchAlbums(userID, searchArtist, searchTitle, page ?? 1);

            var pager = new Pager(searchResults.Count(), page);

            BrowseViewModel l_model = new BrowseViewModel() {
                Pager = pager,
                Albums = searchResults.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList()
            };

            return View(l_model);
        }

        public IActionResult Add(string id)
        {
            string userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ActionResponse response = new ActionResponse();

            if (_dataHelper.AddAlbum(userID, id))
            {
                response.data = id;
                response.success = true;
            }
            else
            {
                response.success = false;
                response.data = "Error adding the record.";
            }
            
            return Json(response);
        }
    }
}