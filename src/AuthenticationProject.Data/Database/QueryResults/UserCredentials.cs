namespace AuthenticationProject.Data.Database.QueryResults
{
    internal class UserCredentials
    {
        public UserCredentials(string passwordHash, string passwordSalt, int hashIterations)
        {
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
            HashIterations = hashIterations;
        }

        internal string PasswordHash { get; }

        internal string PasswordSalt { get; }

        internal int HashIterations { get; }
    }
}