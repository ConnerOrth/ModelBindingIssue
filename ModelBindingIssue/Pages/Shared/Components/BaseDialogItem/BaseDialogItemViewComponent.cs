using ModelBindingIssue.Helpers;
using ModelBindingIssue.Factories;
using ModelBindingIssue.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System;
using System.Threading.Tasks;
using ModelBindingIssue.Entities;

namespace ModelBindingIssue.Pages.Components.BaseDialogItems
{
    [ViewComponent]
    public class BaseDialogItemViewComponent : ViewComponent
    {
        private const string DefaultViewName = "Default";

        //TODO: change to factory to handle dependency injection per concrete type.
        //public async Task<IViewComponentResult> InvokeAsync(ItemWrapperViewModel interactionModelSectionItemViewModel)
        public async Task<IViewComponentResult> InvokeAsync(BaseDialogItemViewModel dialogItemViewModel)
        {
            Type type = Type.GetType(dialogItemViewModel.EntityType);

            if (!ViewComponentContext.ViewExists(type.Name))
            {
                //view doesnt exist/can't be found so show default, instead of throwing exceptions.
                return View(DefaultViewName, dialogItemViewModel);
            }

            //if (type == typeof(CharacteristicDialogItem))
            //{
            //    //make sure this only shows characteristics for a given serviceprovider.
            //    ViewBag.Characteristics = (await repository.ListAsync(new CharacteristicFilterSpecification(HttpContext.Session.Get<Guid?>(Constants.SessionKeys.ServiceProviderId))))
            //        .ToSelectList(s => s.TechnicalName);
            //}

            //check if we have a custom viewmodel for this dialogitem
            //this is just so that we dont have to fix the broken ui if we have no mappings
            if (type == typeof(HemaDialogItem))
            {
                ((HemaDialogItemViewModel)dialogItemViewModel).Mappings.Add(new HemaStatusMapViewModel("test", "value"));
            }
            return View(type.Name, dialogItemViewModel);
        }

    }
}
