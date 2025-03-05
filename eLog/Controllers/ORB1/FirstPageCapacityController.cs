using Microsoft.AspNetCore.Mvc;

namespace eLog.Controllers.ORB1
{
    public class FirstPageCapacityController : Controller
    {
        public IActionResult FirstPageCapacity()
        {
            return View("~/Views/ORB1/FirstPageCapacity.cshtml");
        }
    }
}
