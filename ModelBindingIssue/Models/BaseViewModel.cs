using ModelBindingIssue.Entities;
using ModelBindingIssue.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ModelBindingIssue.Models
{
    public abstract class BaseViewModel : BaseEntity
    {
        public BaseViewModel()
        {
            //required by MVC Views, viewmodels need to have a public parameterless constructor.
        }

        protected BaseViewModel(BaseEntity baseEntity)
        {
            if (baseEntity == null) return;

            Id = baseEntity.Id;
        }
    }

    [DebuggerStepThrough]
    public abstract class AbstractBaseViewModel : BaseViewModel
    {
        public string ActualType => GetType().AssemblyQualifiedName;

        public AbstractBaseViewModel()
        {
        }

        protected AbstractBaseViewModel(BaseEntity baseEntity) : base(baseEntity)
        {
        }
    }

    public class BaseDialogItemViewModel : AbstractBaseViewModel
    {
        public string DialogItemType { get; set; }

        public Guid InteractionModelSectionId { get; set; }
        public string Name { get; set; }
        public int Ordering { get; set; }
        public int MinimumAmountOfStars { get; set; }
        public int MaximumAmountOfStars { get; set; }


        public BaseDialogItemViewModel() { }
        public BaseDialogItemViewModel(BaseDialogItem dialogItem) : base(dialogItem)
        {
            DialogItemType = dialogItem.GetType().AssemblyQualifiedName;
            FromDialogItem(dialogItem);
        }

        public BaseDialogItem ToDialogItem()
        {
            if (TryGetMapperMethod(out MethodInfo? mapperMethod))
            {
                return (BaseDialogItem)mapperMethod.Invoke(this, null);
            }

            Type type = Type.GetType(DialogItemType);

            MethodInfo method = GetType().GetMethod(nameof(BaseDialogItemViewModel.To), BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo generic = method.MakeGenericMethod(type);
            return (BaseDialogItem)generic.Invoke(this, null);
        }

        private TBaseDialogItem To<TBaseDialogItem>() where TBaseDialogItem : BaseDialogItem
        {
            TBaseDialogItem @base = (TBaseDialogItem)Activator.CreateInstance<TBaseDialogItem>();
            PropertyCopy.Copy(this, @base);
            return @base;
        }

        private void FromDialogItem(BaseDialogItem dialogItem)
        {
            if (TryGetMapperMethod(out _))
            {
                return;
            }

            Type type = Type.GetType(DialogItemType);

            MethodInfo method = GetType().GetMethod(nameof(BaseDialogItemViewModel.From), BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo generic = method.MakeGenericMethod(type);
            generic.Invoke(this, new[] { dialogItem });
        }

        private void From<TBaseDialogItem>(TBaseDialogItem dialogItem) where TBaseDialogItem : BaseDialogItem
        {
            PropertyCopy.Copy(dialogItem, this);
        }

        private bool TryGetMapperMethod(out MethodInfo? mapperMethod)
        {
            var mapper = Type.GetType(ActualType)?.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapper<>));
            mapperMethod = mapper?.GetMethod(nameof(IMapper<object>.Map));
            return mapperMethod is object;
        }
    }
}
