using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Models;
using MyApp.Repositories;

namespace MyApp.Controllers {
    public class FinancesController : Controller {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private IAccountRepository _accountRepo;
        private IFinancesRepository _financesRepo;
        private long userId;

        public FinancesController(
            IHttpContextAccessor httpContextAccessor, 
            IAccountRepository accountRepo, 
            IFinancesRepository financesRepo) {
            _httpContextAccessor = httpContextAccessor;
            _accountRepo = accountRepo;
            _financesRepo = financesRepo;

            userId = Convert.ToInt64(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Sid).Value);
        }

        [Authorize(Roles="user,admin")]
        public IActionResult Index() {
            Dictionary<string, object> sortable = new Dictionary<string, object>();
            sortable.Add("sortColumn", "date");
            sortable.Add("sortType", "DESC");
            sortable.Add("offset", 0);
            sortable.Add("max", 10);
            var finances = _financesRepo.GetAllFinancesByUserId(userId, sortable, null);
            TempData["sum"] = _financesRepo.GetFinancesSumByUserId(userId, null);
            return View(finances);
        }

        [Route("api/finances/search")]
        [Authorize(Roles="user,admin")]
        public IActionResult Index([FromBody] Dictionary<string, object> searchable) {
            Dictionary<string, object> sortable = new Dictionary<string, object>();
            sortable.Add("sortColumn", "date");
            sortable.Add("sortType", "DESC");
            sortable.Add("offset", 0);
            sortable.Add("max", 10);
            var finances    = _financesRepo.GetAllFinancesByUserId(userId, sortable, searchable);
            TempData["sum"] = _financesRepo.GetFinancesSumByUserId(userId, searchable);
            return View(finances);
        }

        [Authorize(Roles="user,admin")]
        public IActionResult Metadatas() {
            FinanceMetadata model = new FinanceMetadata();
            model.userId = userId;
            model.financeTypes = _financesRepo.GetAllFinanceTypesByUserId(userId);
            model.places = _financesRepo.GetAllPlacesByUserId(userId);
            return View(model);
        }

        [Authorize(Roles="admin,user")]
        public IActionResult Detail(long id) {
            var finance = _financesRepo.GetFinanceById(id);
            return View(finance);
        }
        
        [HttpGet]
        public IActionResult Create() {
            return View(userId);
        }

        [Route("api/finances/financeTypes")]
        [Authorize(Roles="user,admin")]
        public IActionResult GetFinanceTypes() {
            return Json(_financesRepo.GetAllFinanceTypesByUserId(userId));
        }

        [Route("api/finances/places")]
        [Authorize(Roles="user,admin")]
        public IActionResult GetPlaces() {
            return Json(_financesRepo.GetAllPlacesByUserId(userId));
        }

        [HttpPost]
        [Authorize(Roles="user,admin")]
        public IActionResult UpdateStatus([FromBody] Finance finance) {
            if (finance == null) {
                return Json(new Message{code=1, type="error", message="Dados inválidos na requisição."});
            }
            var affected_rows = _financesRepo.UpdateStatus(finance.id, finance.status);
            if (affected_rows == 0) {
                return Json(new Message{code=3, type="error", message="Erro de banco de dados."});
            }
            return Json(new Message{code=0, type="success", message="Status Atualizado!"}); 
        }

        [HttpPost]
        public IActionResult Create([FromBody] Finance finance) {
            if (finance == null || !ModelState.IsValid) {
                return Json(new Message{code=1, type="error", message="Finança possui campos inválidos."});
            }
            var id = _financesRepo.CreateFinance(finance);
            if (id == 0) {
                return Json(new Message{code=3, type="error", message="Erro de banco de dados."});
            }
            finance.id = id;
            return Json(new Message{code=0, type="success", message="Finança Salva!"}); 
        }

        [HttpPost]
        public IActionResult CreateFinanceType([FromBody] FinanceType financeType) {
            if (financeType == null) {
                return Json(new Message{code=1, type="error", message="Dados inválidos na requisição."});
            }
            if (_accountRepo.UserExists(financeType.id)) {
                return Json(new Message{code=2, type="error", message="UserID não encontrado."});
            }   
            var id = _financesRepo.CreateFinanceType(financeType);
            if (id == 0) {
                return Json(new Message{code=3, type="error", message="Erro de banco de dados."});
            }
            financeType.id = id;
            return Json(new Message{code=financeType.id, type="FinanceType", message=financeType.name});
            
        }

        [HttpPost]
        public IActionResult CreatePlace([FromBody] Place place) {
            if (place == null) {
                return Json(new Message{code=1, type="error", message="Dados inválidos na requisição."});
            }
            if (_accountRepo.UserExists(place.id)) {
                return Json(new Message{code=place.id, type="error", message="UserID não encontrado."});
            }
            var id = _financesRepo.CreatePlace(place);
            if (id == 0) {
                return Json(new Message{code=3, type="error", message="Erro de banco de dados."});
            }
            place.id = id;
            return Json(new Message{code=place.id, type="Place", message=place.name});
        }

        [HttpPost]
        [Authorize(Roles="admin,user")]
        public IActionResult DeletePlace([FromBody] long id) {
            if (id == 0) {
                return Json(new Message{code=1, type="error", message="Dados inválidos na requisição."});
            }
            if (!_financesRepo.CheckForeign(id, "finances", "place_id")) {
                return Json(new Message{code=2, type="warning", message="Este lugar está sendo usado em uma finança."});
            }
            var affected_rows = _financesRepo.DeletePlace(id);
            if (affected_rows == 0) {
                return Json(new Message{code=3, type="error", message="Erro de banco de dados."});
            }
            return Json(new Message{code=0, type="success", message="Local apagado."});
        }

        [HttpPost]
        [Authorize(Roles="admin,user")]
        public IActionResult DeleteFinanceType([FromBody] long id) {
            if (id == 0) {
                return Json(new Message{code=1, type="error", message="Dados inválidos na requisição."});
            }
            if (!_financesRepo.CheckForeign(id, "finances", "place_id")) {
                return Json(new Message{code=2, type="warning", message="Este tipo de finança está sendo usado em uma finança."});
            }  
            var affected_rows = _financesRepo.DeleteFinanceType(id);
            if (affected_rows == 0) {
                return Json(new Message{code=3, message="Erro de banco de dados."});
            }
            return Json(new Message{code=0, type="success", message="Tipo de Finança apagado."}); 
                
        }

        [HttpPost]
        [Authorize(Roles="admin,user")]
        public IActionResult DeleteFinance([FromBody] long id) {
            if (id == 0) {
                return Json(new Message{code=1, type="error", message="Dados inválidos na requisição."});
            }
            var affected_rows = _financesRepo.DeleteFinance(id);
            if (affected_rows == 0) {
                return Json(new Message{code=3, type="error", message="Erro de banco de dados."}); 
            }
            return Json(new Message{code=0, type="success", message="Finança apagada. Esperamos que tenha pago."}); 
        }

    }
}