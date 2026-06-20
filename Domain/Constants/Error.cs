namespace Domain.Constants
{
    public static class Error
    {
        public const string EmailExists = "Email already exists";
        public const string InvalidCredentials = "Email or password is incorrect";
        public const string UserNotFound = "Get user info failed";
        public const string ReferenceRequired = "User have to add at least two references";
        public const string LoanNotFound = "Loan not found";

        // System / Exception errors
        public const string LoanTermInvalid = "Loan term must be >= 1 month";
        public const string EntityNotFoundTemplate = "Entity with id {0} not found";
        public const string TransactionExistsTemplate = "Transaction with ID {0} already exists.";
        public const string TransactionNotFoundTemplate = "Transaction with ID {0} not found.";
        public const string GetOneSelectorRequired = "GetOne<TType> requires a selector which is not provided in the interface";

        // Controller API Error Fallbacks
        public const string UserRegisterFailed = "User registered failed";
        public const string LoginFailed = "Login failed";
        public const string GetLoansFailed = "Failed to retrieve loans";
        public const string GetUserLoansFailed = "Failed to retrieve user loans";
        public const string LoanInfoNotFound = "Loan info not found";
        public const string LoanRepaymentDatesNotFound = "Loan repayment dates not found";
        public const string LoanCreateFailed = "Loan created failed";
        public const string LoanReviewFailed = "Loan reviewed failed";
        public const string NotificationsFetchFailed = "Notifications fetched failed";
        public const string UserReferencesCreateFailed = "User references created failed";
        public const string GetUserReferencesFailed = "Failed to retrieve user references";
    }
}
