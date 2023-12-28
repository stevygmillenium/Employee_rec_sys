using Employee_rec_sys.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Employee_rec_sys.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Apply() 
        {
            return View();
        }
        [HttpPost]
        public IActionResult Confirmation([Bind(include:"Name,email,phone,job_pos,des_sal,Files")]Applicant appl) 
        {
            appl.dateTime=DateTime.Now;
            return View(appl);
        }
        public IActionResult Rec_admin() 
        {
            Applicant applicant=new Applicant();
            List<appl_files>appl_Files=new List<appl_files>();
            applicant.appl_files=appl_Files;
            return View(applicant);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
