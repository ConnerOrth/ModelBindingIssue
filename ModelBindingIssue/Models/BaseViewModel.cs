using ModelBindingIssue.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ModelBindingIssue.Models
{
    public abstract class BaseViewModel
    {
        public Guid Id { get; set; }
        public byte[] RowVersion { get; set; }

        public BaseViewModel()
        {
        }

        protected BaseViewModel(BaseEntity baseEntity)
        {
            if (baseEntity == null) return;

            Id = baseEntity.Id;
            RowVersion = baseEntity.RowVersion;
        }
    }

    public class BaseDialogItemViewModel : BaseViewModel
    {
        private static readonly string @namespace = typeof(BaseDialogItem).Namespace;
        private static readonly string assemblyName = typeof(BaseDialogItem).Assembly.FullName;

        public BaseDialogItemViewModel() { }

        public BaseDialogItemViewModel(BaseDialogItem dialogItem) : base(dialogItem)
        {
            FromDialogItem(dialogItem);
        }

        public BaseDialogItem ToDialogItem(string className)
        {
            Type type = Type.GetType($"{@namespace}.{className}, {assemblyName}");

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

        private BaseDialogItemViewModel FromDialogItem(BaseDialogItem dialogItem)
        {
            Type type = Type.GetType($"{@namespace}.{dialogItem.GetType().Name}, {assemblyName}");

            MethodInfo method = GetType().GetMethod(nameof(BaseDialogItemViewModel.From), BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo generic = method.MakeGenericMethod(type);
            return (BaseDialogItemViewModel)generic.Invoke(this, new[] { dialogItem });
        }

        private void From<TBaseDialogItem>(TBaseDialogItem dialogItem) where TBaseDialogItem : BaseDialogItem
        {
            PropertyCopy.Copy(dialogItem, this);
        }


        public string Name { get; set; }
        public string ChildName { get; set; }
        public string AnotherName { get; set; }

        public IList<Mapping> Mappings { get; set; } = new List<Mapping>()
        {
            new Mapping(){Key="key1",Value="value1"},
            new Mapping(){Key="key2",Value="value2"}
        };
    }

    public class Mapping
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class BackendClassContainingMapping : BaseDialogItem
    {
        public IList<Mapping> Mappings { get; set; } = new List<Mapping>();
    }
    //public class CustomerViewModel : BaseDialogItemViewModel<Customer>
    //{
    //    public string Name { get; set; }
    //    public int Age { get; set; }

    //    public override Customer Map()
    //    {
    //        var customer = new Customer(Name);
    //        customer.SetAge(Age);
    //        return customer;
    //    }
    //}

    //public class Customer : BaseEntity
    //{
    //    public string Name { get; set; }
    //    public int Age { get; private set; }

    //    private Customer()
    //    {
    //    }

    //    public Customer(string name)
    //    {
    //        Name = name;
    //    }
    //    public void SetAge(int age) => Age = age;
    //}
}
