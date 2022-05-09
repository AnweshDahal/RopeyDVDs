using Microsoft.AspNetCore.Mvc;
using RopeyDVDs.Models;
using System.Diagnostics;
using RopeyDVDs.DBContext;

namespace RopeyDVDs.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDBContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDBContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login", "Authentication");
            }
            var totalDvds = _context.DVDTitle.Count();
            var totalCopies = _context.DVDCopy.Where(x => x.IsDeleted != true).Count();
            var totalMembers = _context.Member.Count();

            ViewData["Info"] = new
            {
                TotalDVDs = totalDvds,
                TotalCopies = totalCopies,
                TotalMembers = totalMembers
            };

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}