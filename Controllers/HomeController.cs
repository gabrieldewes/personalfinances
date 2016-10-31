using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Repositories;

namespace MyApp.Controllers {
    public class HomeController : Controller {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private IFinancesRepository _financesRepo;
        private long userId;
        public HomeController(
            IHttpContextAccessor httpContextAccessor,
            IFinancesRepository financesRepo) {
            _httpContextAccessor = httpContextAccessor;
            _financesRepo = financesRepo;
            if (httpContextAccessor.HttpContext.User.Identities.Any(u => u.IsAuthenticated)) {
                userId = Convert.ToInt64(httpContextAccessor.HttpContext.User.Identities.First(
                    u => u.IsAuthenticated && u.HasClaim(
                        c => c.Type == ClaimTypes.Sid)).FindFirst(ClaimTypes.Sid).Value);
            }
            else userId=0;
        }
        [AllowAnonymous]
        public IActionResult Index() { 
            if (userId == 0)
                return View();
            Dictionary<string, object> sortable = new Dictionary<string, object>();
            sortable.Add("sortColumn", "id_finance");
            sortable.Add("sortType", "DESC");
            sortable.Add("offset", 0);
            sortable.Add("max", 7);
            return View(_financesRepo.GetAllFinancesByUserId(userId, sortable, null));
        }
        public IActionResult About() {
            ViewData["Message"] = ".NET Core 1.0";
            return View();
        }

        public IActionResult Contact() {
            ViewData["Message"] = "Hit us up";
            return View();
        }

        public IActionResult Error() {
            return View();
        }
    }
}
