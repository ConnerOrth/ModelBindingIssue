using ModelBindingIssue.Entities;
using ModelBindingIssue.Helpers;
using ModelBindingIssue.Models;

namespace ModelBindingIssue.Factories
{
    public static class DialogItemViewModelFactory
    {
        public static BaseDialogItemViewModel GetViewModel(BaseDialogItem dialogItem)
        {
            BaseDialogItemViewModel viewModel = dialogItem switch
            {
                HemaDialogItem c => c.ToViewModel<HemaDialogItemViewModel>(),
                _ => dialogItem.ToViewModel<BaseDialogItemViewModel>(),
            };

            //viewModel.ActualType = viewModel.GetType().AssemblyQualifiedName;
            return viewModel;
        }
    }
}
