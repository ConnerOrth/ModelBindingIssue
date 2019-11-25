using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModelBindingIssue.Entities
{
    public abstract class BaseDialogItem : BaseEntity
    {
        public Guid InteractionModelSectionId { get; set; }
        public string Name { get; set; }
    }
}
