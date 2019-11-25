using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModelBindingIssue.Entities;
using ModelBindingIssue.Factories;
using ModelBindingIssue.Helpers;
using ModelBindingIssue.Models;
using Newtonsoft.Json;

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
            var hemaDialogItem = Database.HemaDialogItem;

            HemaDialogItemViewModel model = (HemaDialogItemViewModel)DialogItemViewModelFactory.GetViewModel(hemaDialogItem);
            return View(model);
        }

        public IActionResult Index2()
        {
            var baseDialogItem = new BaseFlowDialogItem() { Name = "HemaItem" };

            BaseDialogItemViewModel model = DialogItemViewModelFactory.GetViewModel(baseDialogItem);
            return View(model);
        }

        [HttpPost]
        //public IActionResult Post([Bind("Id,Name,Number,Mappings,ResponseNoPackages")] BaseDialogItemViewModel model)
        public IActionResult Post(AbstractBaseViewModel model)
        {
            string form = null;
            foreach (var item in Request.Form)
            {
                form += $"{{\"{item.Key}\",\"{item.Value}\"}}," + Environment.NewLine;
            }
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create()
        {
            return View(new ItemWrapperViewModel(null) { InteractionModelSectionId = Database.Section.Id });
        }

        [HttpPost]
        //public async Task<IActionResult> Create([Bind("Enabled,InteractionModelSectionId,SelectedDialogItemType,DialogItem")] ItemWrapperViewModel interactionModelSectionItemViewModel)
        public async Task<IActionResult> Create(AbstractBaseViewModel model)
        {
            string form = null;
            foreach (var item in Request.Form)
            {
                form += $"{{\"{item.Key}\",\"{item.Value}\"}}," + Environment.NewLine;
            }
            if (ModelState.IsValid)
            {
                //InteractionModelSection section = await interactionModelService.GetInteractionModelSectionById(interactionModelSectionItemViewModel.InteractionModelSectionId);
                //if (section == null)
                //{
                //    return NotFound();
                //}

                //InteractionModelSectionItem sectionItem = new InteractionModelSectionItem
                //{
                //    //Note: This is because the ordering of the SectionItem  is non-changeable at this create view.
                //    Ordering = interactionModelSectionItemViewModel.DialogItem.Ordering,
                //    Enabled = interactionModelSectionItemViewModel.Enabled,
                //    InteractionModelSectionId = interactionModelSectionItemViewModel.InteractionModelSectionId,
                //    DialogItem = interactionModelSectionItemViewModel.DialogItem.ToDialogItem(interactionModelSectionItemViewModel.SelectedDialogItemType)
                //};

                //await sectionItemRepository.AddAsync(sectionItem);
                //return RedirectToAction(nameof(DetailsSection), new { ids });
            }
            _logger.LogDebug(JsonConvert.SerializeObject(model, Formatting.Indented));
            return View(new ItemWrapperViewModel(null) { InteractionModelSectionId = Database.Section.Id });
        }

        public IActionResult Edit()
        {
            var hemaDialogItem = Database.HemaDialogItem;

            ItemWrapperViewModel itemWrapper = new ItemWrapperViewModel(hemaDialogItem);
            TempData.Set<BaseDialogItem>(itemWrapper.DialogItem.GetType().Name, hemaDialogItem);

            return View(itemWrapper);
        }
        //[HttpPost]
        //public IActionResult Post([Bind("Name,Number")] ViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        return View(nameof(Index));
        //    }
        //    DBModel dbModel = model.Map();
        //    return View(nameof(Index), new ViewModel() { Name = model.Name, Number = model.Number });
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class ViewModel : IMapper<DBModel>
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
