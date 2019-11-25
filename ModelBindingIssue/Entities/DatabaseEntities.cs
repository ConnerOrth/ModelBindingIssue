using ModelBindingIssue.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ModelBindingIssue.Entities
{
    public class ChildDialogItem : BaseDialogItem
    {
        public string ChildName { get; set; }
    }

    public class AnotherDialogItem : BaseDialogItem
    {
        public string AnotherName { get; set; }
    }
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


    public class BaseFlowDialogItem : BaseDialogItem { }
    public class HemaDialogItem : BaseDialogItem
    {
        public string ResponseNoPackages { get; set; }
        public IList<HemaStatusMap> HemaStatuses { get; set; } = new List<HemaStatusMap>();
    }

    public class HemaStatusMap : BaseEntity
    {
        public string ResponseStatus { get; set; }

        public IList<string> InputStatuses { get; set; } = new List<string>();

        private HemaStatusMap()
        {
            //Required by ef
        }

        public HemaStatusMap(string responseStatus, string inputStatuses)
        {
            ResponseStatus = responseStatus;
            InputStatuses.Add(inputStatuses);
        }

        public HemaStatusMap(string responseStatus, IEnumerable<string> inputStatuses)
        {
            ResponseStatus = responseStatus;
            InputStatuses = inputStatuses.ToList();
        }
    }

    [DebuggerDisplay("{Name,nq}")]
    public class Section : BaseEntity
    {
        public Guid InteractionModelId { get; set; }
        public string Name { get; set; }
        public int Ordering { get; set; }
        public bool Enabled { get; set; }
        public IList<BaseDialogItem> Items { get; set; } = new List<BaseDialogItem>();
    }
}
