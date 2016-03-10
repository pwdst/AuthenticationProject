namespace AuthenticationProject.Shared.Results
{
    using System.Collections.Generic;

    public class UserResult
    {
        public UserResult()
        {
            ValidationMessages = new Dictionary<string, string>();
        }

        public UserResult(bool actionSucceeded)
        {
            ActionSucceeded = actionSucceeded;

            ValidationMessages = new Dictionary<string, string>();
        }

        public bool ActionSucceeded { get; private set; }

        public void AddValidationMessage(string key, string validationMessage)
        {
            ActionSucceeded = false;

            ValidationMessages.Add(key, validationMessage);
        }

        public Dictionary<string, string> ValidationMessages { get; }
    }
}
