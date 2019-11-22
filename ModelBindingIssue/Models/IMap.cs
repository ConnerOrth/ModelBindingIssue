using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModelBindingIssue.Models
{
    public interface IMap<out TDestination>
    {
        TDestination Map();
    }
}
