namespace AuthenticationProject.Shared.Projections
{
    using System;

    public interface IUser
    {
        Guid Id { get; }

        string UserName { get; }

        string DisplayName { get; }

        string EmailAddress { get; }

        string TwitterUserName { get; }
    }
}
