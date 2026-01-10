namespace Domain.Constants.AppEnum
{
    public enum Role
    {
        Admin = 0,
        Manager = 1,
        User = 2,
    }

    public enum ResponseResult
    {
        SUCCESS = 0,
        ERROR = 1,
        VALIDATION_ERROR = 2,
        NOT_FOUND = 3,
        UNAUTHORIZED = 4,
        FORBIDDEN = 5
    }

    public enum Gender
    {
        Male = 0,
        Female = 1,
        Other = 2,
    }

    public enum Status
    {
        Active = 0,
        Inactive = 1,
        Pending = 2,
    }

    public enum ReferenceRelationship
    {
        Parent = 0,
        Child = 1,
        Sibling = 2,
        Spouse = 3,
        Other = 4,
    }

    public enum LoanStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        Paid = 3,
        Cancelled = 4,
    }

    public enum UserLoanStatus
    {
        NotYet = 0,
        Due = 1,
        Overdue = 2,
        Paid = 3
    }
}