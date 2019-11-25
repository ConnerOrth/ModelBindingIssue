using Microsoft.AspNetCore.Mvc.Rendering;
using ModelBindingIssue.Entities;
using ModelBindingIssue.Factories;
using ModelBindingIssue.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ModelBindingIssue.Models
{
    public class ItemWrapperViewModel : BaseViewModel
    {
        public Guid InteractionModelSectionId { get; set; }
        public Guid? DialogItemId { get; set; }
        public BaseDialogItemViewModel DialogItem { get; set; }
        public string SelectedDialogItemType { get; set; }
        public ICollection<SelectListItem> DialogItemTypes { get; } = new List<SelectListItem>();

        public ItemWrapperViewModel()
        {
            //Required by DialogItem Page
        }

        //public InteractionModelSectionItemViewModel(IEnumerable<BaseDialogItem> dialogItems) : this(null, dialogItems) { }
        //public InteractionModelSectionItemViewModel(InteractionModelSectionItem item) : this(item, null) { }
        public ItemWrapperViewModel(BaseDialogItem item) : base(item)
        {
            if (item != null)
            {
                InteractionModelSectionId = item.InteractionModelSectionId;
                DialogItemId = item.Id;
                DialogItem = DialogItemViewModelFactory.GetViewModel(item);
                //Ordering = item.Ordering;
                //Enabled = item.Enabled;
            }

            foreach (var dialogItem in Database.DialogItemTypes.Select(s => s.Key))
            {
                DialogItemTypes.Add(new SelectListItem()
                {
                    Value = dialogItem.Name,
                    Text = dialogItem.Name.Replace("DialogItem", string.Empty),
                    Selected = dialogItem.Name == item?.GetType().Name
                });
            }
            SelectedDialogItemType = DialogItemTypes.FirstOrDefault(d => d.Selected)?.Value;
        }
    }

    public class HemaDialogItemViewModel : BaseDialogItemViewModel, IMapper<HemaDialogItem>
    {
        public string ResponseNoPackages { get; set; }
        public IList<HemaStatusMapViewModel> Mappings { get; set; } = new List<HemaStatusMapViewModel>();

        public HemaDialogItemViewModel() { }
        public HemaDialogItemViewModel(HemaDialogItem entity) : base(entity)
        {
            ResponseNoPackages = entity.ResponseNoPackages;
            Name = entity.Name;
            Id = entity.Id;
            Mappings = entity.HemaStatuses.ToViewModel<HemaStatusMapViewModel>().ToList();
        }

        public HemaDialogItem Map()
        {
            return new HemaDialogItem()
            {
                ResponseNoPackages = ResponseNoPackages,
                Name = Name,
                Id = Id,
                HemaStatuses = Mappings.Select(m => m.Map()).ToList()
            };
        }
    }

    public class HemaStatusMapViewModel : BaseViewModel, IMapper<HemaStatusMap>
    {
        public string ResponseStatus { get; set; }
        public string InputStatuses { get; set; }

        public HemaStatusMapViewModel()
        {

        }

        public HemaStatusMapViewModel(string responseStatus, string inputStatuses)
        {
            ResponseStatus = responseStatus;
            InputStatuses = inputStatuses;
        }

        public HemaStatusMapViewModel(HemaStatusMap hemaStatusMap) : base(hemaStatusMap)
        {
            ResponseStatus = hemaStatusMap.ResponseStatus;
            InputStatuses = string.Join(';', hemaStatusMap.InputStatuses);
        }

        public HemaStatusMap Map()
        {
            return new HemaStatusMap(responseStatus: ResponseStatus, inputStatuses: InputStatuses.Split(','))
            {
                Id = Id
            };
        }
    }

    public class ParentVM : BaseDialogItemViewModel, IMapper<Parent>
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
            };
        }
    }

    //childvm no longer has a name or age, but is able,
    //to use the private parentvm to create a complete child object
    public class ChildVM : BaseDialogItemViewModel, IMapper<Child>
    {
        private readonly ParentVM parentVM;
        public string Hobby { get; set; }
        public IList<Mapping> Mappings { get; set; } = new List<Mapping>();

        public ChildVM(Child child) : base(child)
        {
            parentVM = child.ToViewModel<ParentVM>();
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

    public class BackendClassContainingMappingVM : BaseDialogItemViewModel, IMapper<WithMapping>
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
            childVM = backendClass.ToViewModel<ChildVM>();
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
}
