using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RecordCollection.Web.Data;
using RecordCollection.Web.Models;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using RecordCollection.Web.Services;

namespace RecordCollection.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly DataHelper DataHelperController;

        public HomeController(ApplicationDbContext dbContext, IOptions<LastFM_Credentials> settingsOptions)
        {
            DataHelperController = new DataHelper(dbContext, settingsOptions);
        }
        
        public async Task<IActionResult> Index(string searchString, string currentFilter, int? page)
        {
            string userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await DataHelperController.LoadUserRecords(userID);

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                res = res.Where(s => s.LastAlbum.Name.ToLower().Contains(searchString.ToLower())
                                       || s.LastAlbum.ArtistName.ToLower().Contains(searchString.ToLower()));
            }

            var pager = new Pager(res.Count(), page);

            HomeViewModel l_model = new HomeViewModel() {
                Pager = pager,
                Albums = res.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList()
            };
            
            return View(l_model);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult Delete(int id)
        {
            ActionResponse response = new ActionResponse();

            if (DataHelperController.DeleteAlbum(id))
            {
                response.success = true;
                response.data = id.ToString();
            }
            else
            {
                response.success = false;
                response.data = "Error deleting the record.";
            }

            return Json(response);
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
    }
}
