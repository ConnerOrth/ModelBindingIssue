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

        //"[Namespace].[ClassName], [AssemblyName]" <- required typename due to different assemblies.
        private static readonly string @namespace = typeof(BaseDialogItem).Namespace;
        private static readonly string assemblyName = typeof(BaseDialogItem).Assembly.FullName;

        //TODO: change to factory to handle dependency injection per concrete type.
        public async Task<IViewComponentResult> InvokeAsync(ItemWrapperViewModel interactionModelSectionItemViewModel)
        {
            string className = interactionModelSectionItemViewModel.SelectedDialogItemType;
            Type type = Type.GetType($"{@namespace}.{className}, {assemblyName}");

            if (!ViewComponentContext.ViewExists(className))
            {
                //view doesnt exist/can't be found so show default, instead of throwing exceptions.
                return View(DefaultViewName, DialogItemViewModelFactory.GetViewModel((BaseDialogItem)Activator.CreateInstance(type)));
            }

            //if (type == typeof(CharacteristicDialogItem))
            //{
            //    //make sure this only shows characteristics for a given serviceprovider.
            //    ViewBag.Characteristics = (await repository.ListAsync(new CharacteristicFilterSpecification(HttpContext.Session.Get<Guid?>(Constants.SessionKeys.ServiceProviderId))))
            //        .ToSelectList(s => s.TechnicalName);
            //}

            //check if we have a custom viewmodel for this dialogitem
            //interactionModelSectionItemViewModel.DialogItem = DialogItemViewModelFactory.GetViewModel((BaseDialogItem)Activator.CreateInstance(type));
            var item = DialogItemViewModelFactory.GetViewModel((BaseDialogItem)Activator.CreateInstance(type));
            if (type == typeof(HemaDialogItem))
            {
                ((HemaDialogItemViewModel)item).Mappings.Add(new HemaStatusMapViewModel("test", "value"));
            }
            return View(className, item);
        }

    }
}
