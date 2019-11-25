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
        [BindProperty]
        public Type Type { get; set; }

        [ViewData]
        public IEnumerable<SelectListItem> JumpTargets { get; set; } = new List<SelectListItem>();

        [ViewData]
        public int Ordering { get; set; }

        public ItemWrapperViewModel InteractionModelSectionItemViewModel { get; set; }

        public async Task OnPost([FromForm]ItemWrapperViewModel interactionModelSectionItemViewModel)
        {
            //InteractionModelSection section = (await sectionRepository.ListAsync(new InteractionModelSectionWithNIdFilterSpecification(interactionModelSectionItemViewModel.InteractionModelSectionId, true, false, false, false))).FirstOrDefault();
            //JumpTargets = await GetJumpTargets(section.InteractionModelId);
            string className = interactionModelSectionItemViewModel.SelectedDialogItemType;
            Type type = Type.GetType($"{@namespace}.{className}, {assemblyName}");

            interactionModelSectionItemViewModel.DialogItem = DialogItemViewModelFactory.GetViewModel((BaseDialogItem)Activator.CreateInstance(type));
            Type = interactionModelSectionItemViewModel.DialogItem.GetType();
            //interactionModelSectionItemViewModel.DialogItem = DialogItemViewModelFactory.GetViewModel(Database.DialogItemTypes[type]);
            DialogItem = interactionModelSectionItemViewModel.DialogItem;
            InteractionModelSectionItemViewModel = interactionModelSectionItemViewModel;
            //Ordering = section.Items.Count > 0 ? section.Items.Max(i => i.Ordering) + 1 : 1;
        }

        //"[Namespace].[ClassName], [AssemblyName]" <- required typename due to different assemblies.
        private static readonly string @namespace = typeof(BaseDialogItem).Namespace;
        private static readonly string assemblyName = typeof(BaseDialogItem).Assembly.FullName;

        public async Task OnPostUpdate([FromForm]ItemWrapperViewModel interactionModelSectionItemViewModel)
        {
            if (interactionModelSectionItemViewModel.DialogItem == null)
            {
                string className = interactionModelSectionItemViewModel.SelectedDialogItemType;
                Type type = Type.GetType($"{@namespace}.{className}, {assemblyName}");
                //interactionModelSectionItemViewModel.DialogItem = new BaseDialogItemViewModel((BaseDialogItem)TempData.Get(interactionModelSectionItemViewModel.SelectedDialogItemType, type));
                //interactionModelSectionItemViewModel.DialogItem = BaseDialogItemViewModel.CreateViewModelForDialogItem((BaseDialogItem)TempData.Get(interactionModelSectionItemViewModel.SelectedDialogItemType, type));
                interactionModelSectionItemViewModel.DialogItem = DialogItemViewModelFactory.GetViewModel((BaseDialogItem)TempData.Get(interactionModelSectionItemViewModel.SelectedDialogItemType, type));
            }

            //InteractionModelSection section = Database.Section(await sectionRepository.ListAsync(new InteractionModelSectionWithNIdFilterSpecification(interactionModelSectionItemViewModel.InteractionModelSectionId, true, false, false, false))).FirstOrDefault();
            //JumpTargets = await GetJumpTargets(section.InteractionModelId);

            //DialogItem = interactionModelSectionItemViewModel.DialogItem ?? new BaseDialogItemViewModel((await sectionItemRepository.ListAsync(new InteractionModelSectionItemWithNIdFilterSpecification(interactionModelSectionItemViewModel.Id, true, false, true))).FirstOrDefault()?.DialogItem);
            //DialogItem = interactionModelSectionItemViewModel.DialogItem ?? BaseDialogItemViewModel.CreateViewModelForDialogItem((await sectionItemRepository.ListAsync(new InteractionModelSectionItemWithNIdFilterSpecification(interactionModelSectionItemViewModel.Id, true, false, true))).FirstOrDefault()?.DialogItem);
            DialogItem = interactionModelSectionItemViewModel.DialogItem ?? DialogItemViewModelFactory.GetViewModel(Database.HemaDialogItem);
            Type = DialogItem.GetType();
            InteractionModelSectionItemViewModel = interactionModelSectionItemViewModel;
            //Ordering = interactionModelSectionItemViewModel.DialogItem.Ordering;
        }

        private async Task<IEnumerable<SelectListItem>> GetJumpTargets(Guid interactionModelId)
        {
            return new List<SelectListItem>();
        }
    }
}
