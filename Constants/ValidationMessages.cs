namespace ClassCheck.Constants
{
    public static class ValidationMessages
    {
        public const string RequiredFields = "All fields are required. Please ensure you have filled out every section.";
        public const string InvalidEmail = "Please enter a valid email address.";
        public const string PasswordTooShort = "Your password must be at least 6 characters long for security purposes.";
        public const string EmailExists = "An account with this email already exists. Please try logging in or use a different email.";
        public const string InvalidCredentials = "Invalid email or password. Please check your credentials and try again.";
        public const string EmptyCredentials = "Email and Password cannot be empty. Please enter your credentials.";

		public const string StudentIDExists = "A student with this ID card number already exists. Please check the ID card number and try again.";
    }
}
