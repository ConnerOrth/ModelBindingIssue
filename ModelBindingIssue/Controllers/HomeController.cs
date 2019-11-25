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
            return View();
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
            var dialogItem = ((BaseDialogItemViewModel)model).ToDialogItem();
            _logger.LogDebug(JsonConvert.SerializeObject(dialogItem, Formatting.Indented));

            return View(new ItemWrapperViewModel(null) { InteractionModelSectionId = Database.Section.Id });
        }

        public IActionResult Edit()
        {
            var hemaDialogItem = Database.HemaDialogItem;

            ItemWrapperViewModel itemWrapper = new ItemWrapperViewModel(hemaDialogItem);
            TempData.Set<BaseDialogItem>(itemWrapper.SelectedDialogItemType, hemaDialogItem);
            TempData.Set<BaseDialogItem>(itemWrapper.DialogItem.GetType().Name, hemaDialogItem);

            return View(itemWrapper);
        }

        [HttpPost]
        public IActionResult Edit(AbstractBaseViewModel model)
        {
            var hemaDialogItem = Database.HemaDialogItem;

            ItemWrapperViewModel itemWrapper = new ItemWrapperViewModel(hemaDialogItem);
            TempData.Set<BaseDialogItem>(itemWrapper.SelectedDialogItemType, hemaDialogItem);
            TempData.Set<BaseDialogItem>(itemWrapper.DialogItem.GetType().Name, hemaDialogItem);

            _logger.LogDebug(JsonConvert.SerializeObject(model, Formatting.Indented));
            var dialogItem = ((BaseDialogItemViewModel)model).ToDialogItem();
            _logger.LogDebug(JsonConvert.SerializeObject(dialogItem, Formatting.Indented));

            return View(itemWrapper);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
