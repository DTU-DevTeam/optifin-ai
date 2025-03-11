using Microsoft.AspNetCore.Mvc;

namespace AIFinanceAdvisor.UI.Controllers
{
    public class OptifinController : Controller
    {
        [Route("/")]
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Loading_screen() 
        { 
            return View();
        }

       
    }
}
