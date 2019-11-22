using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModelBindingIssue.Models;

namespace ModelBindingIssue.Controllers
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

        [HttpPost]
        public IActionResult Post([Bind("Name,Number")] ViewModel model)
        {
            if (ModelState.IsValid)
            {
                return View(nameof(Index));
            }
            DBModel dbModel = model.Map();
            return View(nameof(Index), new ViewModel() { Name = model.Name, Number = model.Number });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class ViewModel : IMap<DBModel>
    {
        [Required(ErrorMessage = "The name property is a required property.")]
        public string Name { get; set; }
        [Range(10, 100, ErrorMessage = "Range should be more than 9 and less than 101.")]
        public int Number { get; set; }

        public DBModel Map() => new DBModel() { Name = Name, Number = Number };
    }

    public class DBModel
    {
        public string Name { get; set; }
        public int Number { get; set; }
    }
}
