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
        public Type Type { get; set; }

        public AbstractBaseViewModel()
        {
        }

        protected AbstractBaseViewModel(BaseEntity baseEntity) : base(baseEntity)
        {
        }
    }

    public class BaseDialogItemViewModel : AbstractBaseViewModel
    {
        public Guid InteractionModelSectionId { get; set; }
        public string Name { get; set; }
        public int Ordering { get; set; }
        public int MinimumAmountOfStars { get; set; }
        public int MaximumAmountOfStars { get; set; }


        public BaseDialogItemViewModel() { }
        public BaseDialogItemViewModel(BaseDialogItem dialogItem) : base(dialogItem)
        {
            viewModelType = Type.GetType($"{typeof(BaseDialogItemViewModel).Namespace}.{dialogItem.GetType().Name}ViewModel, {typeof(BaseDialogItemViewModel).Assembly.FullName}");
            FromDialogItem(dialogItem);
        }

        private readonly Type viewModelType = Type.GetType($"{typeof(BaseDialogItemViewModel).Namespace}.{nameof(BaseDialogItemViewModel)}, {typeof(BaseDialogItemViewModel).Assembly.FullName}");

        private static readonly string @namespace = typeof(BaseDialogItem).Namespace;
        private static readonly string assemblyName = typeof(BaseDialogItem).Assembly.FullName;

        public BaseDialogItem ToDialogItem(string className)
        {
            Type type = Type = Type.GetType($"{@namespace}.{className}, {assemblyName}");

            var mapper = viewModelType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapper<>));
            var mapperMethod = mapper?.GetMethod(nameof(IMapper<object>.Map));
            if (mapperMethod is object)
            {
                return (BaseDialogItem)mapperMethod.Invoke(this, null);
            }

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
            Type type = Type.GetType($"{@namespace}.{dialogItem.GetType().Name}, {assemblyName}");

            var mapper = viewModelType?.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapper<>));
            var mapperMethod = mapper?.GetMethod(nameof(IMapper<object>.Map));
            if (mapperMethod is object)
            {
                return;
            }

            MethodInfo method = GetType().GetMethod(nameof(BaseDialogItemViewModel.From), BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo generic = method.MakeGenericMethod(type);
            generic.Invoke(this, new[] { dialogItem });
        }

        private void From<TBaseDialogItem>(TBaseDialogItem dialogItem) where TBaseDialogItem : BaseDialogItem
        {
            PropertyCopy.Copy(dialogItem, this);
        }
    }
}
