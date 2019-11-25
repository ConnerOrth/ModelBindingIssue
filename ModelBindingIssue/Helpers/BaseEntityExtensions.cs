using ModelBindingIssue.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelBindingIssue.Helpers
{
    public static class BaseEntityExtensions
    {
        public static TViewModel ToViewModel<TViewModel>(this BaseEntity entity)
            where TViewModel : BaseViewModel
        {
            return (TViewModel)Activator.CreateInstance(typeof(TViewModel), entity);
        }

        public static IEnumerable<TViewModel> ToViewModel<TViewModel>(this IEnumerable<BaseEntity> entities)
            where TViewModel : BaseViewModel
        {
            return entities.Select(ToViewModel<TViewModel>);
        }
    }
}
