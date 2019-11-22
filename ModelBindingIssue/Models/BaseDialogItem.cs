using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModelBindingIssue.Models
{
    public abstract class BaseDialogItem : BaseEntity
    {
        public string Name { get; set; }

        //public const int MaxResponses = 4;

        //private static readonly Random random = new Random();

        //public int Ordering { get; set; }
        //public bool EndSession { get; set; }

        //public bool IsNotYetAnswered => string.IsNullOrWhiteSpace(Answer);
        //public string Answer { get; set; }
        //public bool Reset { get; set; }
        //public bool IsCurrent { get; set; }
        //public bool IsHandled { get; set; }


        //public IList<string> SummarizeItems { get; set; } = new List<string>();

        //protected string response;
        //public string Response => response ??= GetRandomResponse(GeneralResponses);

        //// AMI: Setter is necessary because PropertyCopier copies the whole value, not the elements of the collection!
        //public IList<string> GeneralResponses { get; set; } = new List<string>();
        //public IList<string> KnownCustomerResponses { get; set; } = new List<string>();

        //private static int GetRandomResponseIndex(int maxValue)
        //{
        //    // Even though it's extremely simple, the method is left here in case concurrency problems will need to be
        //    // handled in the future.
        //    return random.Next(maxValue);
        //}

        //protected string GetRandomResponse(IList<string> responses)
        //{
        //    if (responses is null || responses.Count == 0) return null;
        //    if (responses.Count == 1) return responses[0];
        //    return responses[GetRandomResponseIndex(responses.Count)];
        //}

        //public void AddGeneralResponse(string response)
        //{
        //    GeneralResponses.Add(response);
        //}

        //public void AddKnownCustomerResponse(string response)
        //{
        //    KnownCustomerResponses.Add(response);
        //}

        ///// <summary>
        ///// Helper method that mimics the settting behavior of the original string field Response.
        ///// Used only in ProfileTagDialogItemHandler - the reason for setting response in handler is unclear.
        ///// </summary>
        ///// <param name="response"></param>
        //public void SetSingleResponse(string response)
        //{
        //    KnownCustomerResponses.Clear();
        //    GeneralResponses.Clear();
        //    AddGeneralResponse(response);
        //    this.response = response;
        //}
    }

    public class ChildDialogItem : BaseDialogItem
    {
        public string ChildName { get; set; }
    }
    public class AnotherDialogItem : BaseDialogItem
    {
        public string AnotherName { get; set; }
    }
}
