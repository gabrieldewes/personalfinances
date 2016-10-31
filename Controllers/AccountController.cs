using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using MyApp.Models;
using MyApp.Repositories;

namespace MyApp.Controllers {
    public class AccountController : Controller {
        private IAccountRepository _accountRepo;
        public AccountController(IAccountRepository accountRepo) {
            _accountRepo = accountRepo;
        }

        [Authorize(Roles="admin")]
        public IActionResult Index() {
            return View(_accountRepo.GetAll());
        }
        public IActionResult Register() {
            return View();
        }
        public IActionResult Login() {
            return View();
        }
        public IActionResult Forbidden() {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("username,password")] UserDTO u, string returnUrl) {
            if (ModelState.IsValid) {               
                var e = _accountRepo.GetByUsername(u.username);
                if (e != null) {
                    if (e.password == u.password) {
                        const string Issuer = "http://localhost:5000";
                        var claims = new List<Claim>();
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, e.username, ClaimValueTypes.String, Issuer));
                        claims.Add(new Claim(ClaimTypes.Name, e.fullname, ClaimValueTypes.String, Issuer));
                        claims.Add(new Claim(ClaimTypes.Sid, ""+e.id, ClaimValueTypes.String, Issuer));
                        claims.Add(new Claim(ClaimTypes.Role, e.role, ClaimValueTypes.String, Issuer));
                        var userIdentity = new ClaimsIdentity("SuperSecureLogin");
                        userIdentity.AddClaims(claims);    
                        var userPrincipal = new ClaimsPrincipal(userIdentity);
                        
                        await HttpContext.Authentication.SignInAsync("Cookie", userPrincipal,
                            new AuthenticationProperties {
                                IsPersistent = false,
                                AllowRefresh = false
                            });
                        
                        return RedirectToLocal(returnUrl);
                    }
                }
                ModelState.AddModelError(string.Empty, "Usuário ou senha incorretos.");
            }
            return View(u);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register([Bind("id,fullname,username,password,email")] User u, string returnUrl) {
            if (ModelState.IsValid) {
                u.role = "user";               
                _accountRepo.Add(u);
                return RedirectToLocal(returnUrl);
            }
            return View(u);
        }
        public async Task<IActionResult> Logout() {
            await HttpContext.Authentication.SignOutAsync("Cookie");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Detail(long id) {
            var u = _accountRepo.GetById(id);
            return View(u);
        }

        private IActionResult RedirectToLocal(string returnUrl) {
            if (Url.IsLocalUrl(returnUrl)) {
                return Redirect(returnUrl);
            }
            else {
                return RedirectToAction("Index", "Home");
            }
        }

    }
}
