using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using ModelBindingIssue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModelBindingIssue.ModelBinders
{
    public class DerivedTypeModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType != typeof(AbstractBaseViewModel))
            {
                return null;
            }

            var assembly = typeof(AbstractBaseViewModel).Assembly;
            var subclasses = assembly.GetExportedTypes()
                .Where(t => !t.IsInterface && typeof(AbstractBaseViewModel).IsAssignableFrom(t) && !t.IsAbstract)
                .ToList();

            var binders = new Dictionary<Type, (ModelMetadata, IModelBinder)>();
            foreach (var type in subclasses)
            {
                var modelMetadata = context.MetadataProvider.GetMetadataForType(type);
                binders[type] = (modelMetadata, context.CreateBinder(modelMetadata));
            }

            return new DerivedTypeModelBinder(binders);
        }
    }

    public class DerivedTypeModelBinder : IModelBinder
    {
        private Dictionary<Type, (ModelMetadata, IModelBinder)> binders;

        public DerivedTypeModelBinder(Dictionary<Type, (ModelMetadata, IModelBinder)> binders)
        {
            this.binders = binders;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var modelKindName = ModelNames.CreatePropertyModelName(bindingContext.ModelName, nameof(AbstractBaseViewModel.ActualType));
            var modelTypeValue = bindingContext.ValueProvider.GetValue(modelKindName).FirstValue;

            IModelBinder modelBinder;
            ModelMetadata modelMetadata;
            if (binders.TryGetValue(Type.GetType(modelTypeValue), out (ModelMetadata, IModelBinder) metaDataBinder))
            {
                (modelMetadata, modelBinder) = metaDataBinder;
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

            var newBindingContext = DefaultModelBindingContext.CreateBindingContext(
                bindingContext.ActionContext,
                bindingContext.ValueProvider,
                modelMetadata,
                bindingInfo: null,
                bindingContext.ModelName);

            await modelBinder.BindModelAsync(newBindingContext);
            bindingContext.Result = newBindingContext.Result;

            if (newBindingContext.Result.IsModelSet)
            {
                // Setting the ValidationState ensures properties on derived types are correctly 
                bindingContext.ValidationState[newBindingContext.Result] = new ValidationStateEntry
                {
                    Metadata = modelMetadata,
                };
            }
        }
    }
}
