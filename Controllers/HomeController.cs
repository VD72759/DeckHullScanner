using DeckHullScanner.Interface;
using DeckHullScanner.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Text.Json;

namespace DeckHullScanner.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IHttpContextAccessor _iHttpContextAccessor { get; set; }
        private IUtility _iUtility { get; set; }
        private string _releaseStage { get; set; }
        private string _releaseName { get; set; }
        private string _releaseVersion { get; set; }
        private string _releaseDate { get; set; }

        public HomeController(ILogger<HomeController> logger, IUtility iUtility, IHttpContextAccessor iHttpContextAccessor)
        {
            _logger = logger;
            _iHttpContextAccessor = iHttpContextAccessor;
            _iUtility = iUtility;

            _releaseStage = _iUtility.GetReleaseStage();
            _releaseName = _iUtility.GetReleaseName();
            _releaseVersion = _iUtility.GetReleaseVersion();
            _releaseDate = _iUtility.GetReleaseDate();
        }

        public IActionResult Index()
        {
            if (_releaseStage.ToUpper() == "RELEASE")
            {
                ViewBag.AppVersion = _releaseName + " " + _releaseVersion + " " + _releaseDate;
            }
            else
            {
                ViewBag.AppVersion = "(" + _releaseStage + ")" + " " + _releaseName + " " + _releaseVersion + " " + _releaseDate;
            }



            return View();
        }
        
        public IActionResult MainPage()
        {
            var EmployeeNumber = _iHttpContextAccessor.HttpContext.Session.GetString("EmployeeNumber");
            string homepage = Url.Action("Index", "Home");
            if (string.IsNullOrEmpty(EmployeeNumber))
                return Redirect(homepage);

            ViewBag.EmpId = EmployeeNumber;





            return View();
        }

    }
}