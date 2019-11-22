using ModelBindingIssue.Models;
using System;
using Xunit;
using ModelBindingIssue.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void DialogVM_To_BaseEntity()
        {
            var child = new ChildDialogItem()
            {
                Name = "__child",
                ChildName = nameof(ChildDialogItem) + DateTime.Now.ToString()
            };
            var another = new AnotherDialogItem()
            {
                Name = "__another",
                AnotherName = nameof(AnotherDialogItem) + DateTime.Now.ToString()
            };

            var withMapping = new WithMapping(new List<Mapping>() { Mapping.Create("k1", "v1") });

            var x = new BaseDialogItemViewModel(child);
            var y = new BaseDialogItemViewModel(another);
            var z = new BaseDialogItemViewModel(withMapping);

            var xx = x.ToDialogItem(child.GetType().Name);
            var yy = y.ToDialogItem(another.GetType().Name);
            var zz = z.ToDialogItem(withMapping.GetType().Name);

            Assert.True(((WithMapping)zz).Mappings.Count == 1);
            Assert.True(((WithMapping)zz).Mappings[0].Key == withMapping.Mappings[0].Key);
            Assert.True(((WithMapping)zz).Mappings[0].Value == withMapping.Mappings[0].Value);
        }

        [Fact]
        public void ChildVM_To_BaseEntity()
        {
            var childvm = new ChildVM(new Child()
            {
                Name = "__child",
                Hobby = "__hobby",
                Age = 8,
            });

            var withMappingVM = new BackendClassContainingMappingVM(new WithMapping(new List<Mapping>() { Mapping.Create("key123", "value123") })
            {
                Name = "__backend",
                Hobby = "__mapping",
                Age = 8,
            });

            Child child = childvm.Map();
            Parent parent = childvm.Map();

            WithMapping backend = withMappingVM.Map();
            Child backendChild = withMappingVM.Map();
            Assert.True(backend.Mappings.Count == 1);
            Assert.True(backend.Mappings[0].Key == "key123");
        }
    }

    //*******************************************************//
    //below are the classes and interfaces used by the tests.//
    //*******************************************************//
    //we dont use a type constraint on TDestination, or a new() constraint
    //because entities might have a private constructor and/or a constructor
    //which requires parameters
    public interface IMapper<out TDestination>
    {
        TDestination Map();
    }

    //frontend VMs/DTOs are build with composition
    public abstract class BaseVM : BaseEntity
    {
        public BaseVM()
        {
            //required by MVC Views, viewmodels needs to have a public parameterless constructor.
        }

        protected BaseVM(BaseEntity baseEntity)
        {
            if (baseEntity == null) return;

            Id = baseEntity.Id;
            RowVersion = baseEntity.RowVersion;
        }
    }

    public class BaseDialogVM : BaseVM
    {
        public string Name { get; set; }

        public BaseDialogVM(BaseEntity baseEntity) : base(baseEntity)
        {
        }
    }

    public class ParentVM : BaseDialogVM, IMapper<Parent>
    {
        public int Age { get; set; }

        public ParentVM(Parent parent) : base(parent)
        {
            Name = parent.Name;
            Age = parent.Age;
        }

        public Parent Map()
        {
            return new Parent()
            {
                Name = Name,
                Age = Age,
                Id = Id,
                RowVersion = RowVersion
            };
        }
    }

    //childvm no longer has a name or age, but is able,
    //to use the private parentvm to create a complete child object
    public class ChildVM : BaseDialogVM, IMapper<Child>
    {
        private readonly ParentVM parentVM;
        public string Hobby { get; set; }
        public IList<Mapping> Mappings { get; set; } = new List<Mapping>();

        public ChildVM(Child child) : base(child)
        {
            parentVM = child.ToVM<ParentVM>();
        }

        public Child Map()
        {
            return new Child()
            {
                Name = parentVM.Name,
                Age = parentVM.Age,
                Hobby = Hobby
            };
        }
    }
    public class BackendClassContainingMappingVM : BaseDialogVM, IMapper<WithMapping>
    {
        private readonly ChildVM childVM;
        public string Hobby { get; set; }
        public IList<Mapping> Mappings { get; } = new List<Mapping>()
        {
            Mapping.Create("321yek","321eulav"),
            Mapping.Create("654yek","654eulav"),
        };

        public BackendClassContainingMappingVM(WithMapping backendClass) : base(backendClass)
        {
            Hobby = backendClass.Hobby;
            Mappings = backendClass.Mappings;
            Name = backendClass.Name;
            Id = backendClass.Id;
            RowVersion = backendClass.RowVersion;
            childVM = backendClass.ToVM<ChildVM>();
        }

        public WithMapping Map()
        {
            return new WithMapping(Mappings)
            {
                Name = childVM.Name,
                Hobby = Hobby
            };
        }
    }

    //backend entities, still use inheritance
    public class Parent : BaseDialogItem
    {
        public int Age { get; set; }
    }

    public class Child : Parent
    {
        public string Hobby { get; set; }
    }

    public class WithMapping : Child
    {
        public IList<Mapping> Mappings { get; } = new List<Mapping>();
        public WithMapping(IList<Mapping> mappings)
        {
            Mappings = mappings;
        }
    }

    public sealed class Mapping
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public static Mapping Create(string key, string value) => new Mapping(key, value);
        private Mapping(string key, string value) => (Key, Value) = (key, value);
    }

    //helper class 
    public static class BaseVMExtensions
    {
        public static TViewModel ToVM<TViewModel>(this BaseEntity entity)
            where TViewModel : BaseVM
        {
            return (TViewModel)Activator.CreateInstance(typeof(TViewModel), entity);
        }
    }
}
