namespace AuthenticationProject.Shared.Interfaces.Repositories
{
    using System;
    using JetBrains.Annotations;
    using Results;

    public interface IUserRepository
    {
        UserResult CreateUser([NotNull] string userName, [NotNull] string displayName, [NotNull] string emailAddress, [NotNull] string password, string twitterUserName);

        UserResult UpdatePassword(Guid userId, [NotNull] string existingPassword, [NotNull] string newPassword);

        UserResult ValidatePassword([NotNull] string username, string password);
    }
}