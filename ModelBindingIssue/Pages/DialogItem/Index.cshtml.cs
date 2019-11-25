using ModelBindingIssue.Helpers;
using ModelBindingIssue.Factories;
using ModelBindingIssue.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModelBindingIssue.Entities;

namespace ModelBindingIssue.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public BaseDialogItemViewModel DialogItem { get; set; }

        [ViewData]
        public IEnumerable<SelectListItem> JumpTargets { get; set; } = new List<SelectListItem>();

        [ViewData]
        public int Ordering { get; set; }


        public async Task OnPost([FromForm]ItemWrapperViewModel interactionModelSectionItemViewModel)
        {
            string className = interactionModelSectionItemViewModel.SelectedDialogItemType;
            Type type = Type.GetType($"{@namespace}.{className}, {assemblyName}");

            interactionModelSectionItemViewModel.DialogItem = DialogItemViewModelFactory.GetViewModel((BaseDialogItem)Activator.CreateInstance(type));

            DialogItem = interactionModelSectionItemViewModel.DialogItem;
        }

        //"[Namespace].[ClassName], [AssemblyName]" <- required typename due to different assemblies.
        private static readonly string @namespace = typeof(BaseDialogItem).Namespace;
        private static readonly string assemblyName = typeof(BaseDialogItem).Assembly.FullName;

        public async Task OnPostUpdate([FromForm]ItemWrapperViewModel interactionModelSectionItemViewModel)
        {
            string className = interactionModelSectionItemViewModel.SelectedDialogItemType;
            Type type = Type.GetType($"{@namespace}.{className}, {assemblyName}");
            if (interactionModelSectionItemViewModel.DialogItem == null)
            {
                interactionModelSectionItemViewModel.DialogItem = DialogItemViewModelFactory.GetViewModel((BaseDialogItem)TempData.Get(interactionModelSectionItemViewModel.SelectedDialogItemType, type));
            }

            DialogItem = interactionModelSectionItemViewModel.DialogItem ?? DialogItemViewModelFactory.GetViewModel(Database.HemaDialogItem);
        }
    }
}
