namespace ModelBindingIssue.Models
{
    //we dont use a type constraint on TDestination, or a new() constraint
    //because entities might have a private constructor and/or a constructor
    //which requires parameters
    public interface IMapper<out TDestination>
    {
        TDestination Map();
    }
}
