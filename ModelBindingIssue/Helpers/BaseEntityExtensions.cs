using ModelBindingIssue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModelBindingIssue.Helpers
{
    public static class BaseEntityExtensions
    {
        public static TViewModel ToViewModel<TViewModel>(this BaseEntity entity)
            where TViewModel : BaseViewModel
        {
            return (TViewModel)Activator.CreateInstance(typeof(TViewModel), entity);
        }
    }
}
