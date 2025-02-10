using Microsoft.AspNetCore.Mvc;

namespace Ekip2.Presentation.Areas.Manager.Controllers
{
    public class HomeController : ManagerBaseController
    {
        public IActionResult Index()
        {
            return View();
        }
        
    }
}
