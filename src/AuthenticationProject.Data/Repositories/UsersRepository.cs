namespace AuthenticationProject.Data.Repositories
{
    using Database.QueryResults;
    using Extensions.Security;
    using Microsoft.Extensions.Localization;
    using Shared.Interfaces.ConnectionWrappers;
    using Shared.Interfaces.Repositories;
    using Shared.Results;
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.Contracts;
    using System.Text;

    public class UserRepository : IUserRepository
    {
        private readonly IStringLocalizer _stringLocalizer;

        private readonly ISecuritySqlConnectionWrapper _securitySqlConnectionWrapper;

        public UserRepository(IStringLocalizer stringLocalizer, ISecuritySqlConnectionWrapper securitySqlConnectionWrapper)
        {
            _stringLocalizer = stringLocalizer;

            _securitySqlConnectionWrapper = securitySqlConnectionWrapper;
        }

        public UserResult CreateUser(string userName, string displayName, string emailAddress, string password, string twitterUserName)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(userName));
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(displayName));
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(emailAddress));
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(password));

            var userNameExistsCommand = new SqlCommand("security.UserNameExists")
            {
                CommandType = CommandType.StoredProcedure,
                Connection = _securitySqlConnectionWrapper.SqlConnection
            };

            userNameExistsCommand.Parameters.AddWithValue("userName", userName);

            var userNameExists = Convert.ToBoolean(userNameExistsCommand.ExecuteScalar());

            var emailAddressExistsCommand = new SqlCommand("security.EmailAddressExists")
            {
                CommandType = CommandType.StoredProcedure,
                Connection = _securitySqlConnectionWrapper.SqlConnection
            };

            emailAddressExistsCommand.Parameters.AddWithValue("emailAddress", emailAddress);

            var emailExists = Convert.ToBoolean(emailAddressExistsCommand.ExecuteScalar());

            if (userNameExists || emailExists)
            {
                var result = new UserResult(false);

                if (userNameExists)
                {
                    result.AddValidationMessage("UserName", _stringLocalizer.GetString("UsernameInUse"));
                }

                if (emailExists)
                {
                    result.AddValidationMessage("EmailAddress", _stringLocalizer.GetString("EmailAddressInUse"));
                    // Nb - This is proof of concept code, production systems should probably include email
                    // validation as part of the process and email the address provided to state the address
                    // is already in use to prevent enumeration attacks. 
                }

                return result;
            }

            var defaultIdentityVersionCommand = new SqlCommand("security.GetDefaultIdentityVersion")
            {
                CommandType = CommandType.StoredProcedure,
                Connection = _securitySqlConnectionWrapper.SqlConnection
            };

            var identityId = 1;

            var hashIterations = 2500;

            using (var defaultIdentityVersion = defaultIdentityVersionCommand.ExecuteReader())
            {
                if (defaultIdentityVersion.Read())
                {
                    var identityVersionIndex = defaultIdentityVersion.GetOrdinal("IdentityVersion");

                    identityId = defaultIdentityVersion.GetInt32(identityVersionIndex);

                    var hashIterationsIndex = defaultIdentityVersion.GetOrdinal("HashIterations");

                    hashIterations = defaultIdentityVersion.GetInt32(hashIterationsIndex);
                }
            }

            var salt = UserExtensions.GenerateSalt();

            var passwordHash = UserExtensions.HashPassword(Encoding.UTF8.GetBytes(password), salt, hashIterations);

            var createUserCommand = new SqlCommand("security.CreateUser")
            {
                CommandType = CommandType.StoredProcedure,
                Connection = _securitySqlConnectionWrapper.SqlConnection
            };

            createUserCommand.Parameters.AddWithValue("userName", userName);
            createUserCommand.Parameters.AddWithValue("displayName", displayName);
            createUserCommand.Parameters.AddWithValue("emailAddress", emailAddress);
            createUserCommand.Parameters.AddWithValue("passwordHash", Convert.ToBase64String(passwordHash));
            createUserCommand.Parameters.AddWithValue("passwordSalt", Convert.ToBase64String(salt));
            createUserCommand.Parameters.AddWithValue("twitterUserName", twitterUserName);
            createUserCommand.Parameters.AddWithValue("userIdentityVersion", identityId);

            createUserCommand.ExecuteNonQuery();

            return new UserResult(true);
        }

        public UserResult UpdatePassword(string username, string existingPassword, string newPassword)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(username));
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(existingPassword));
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(newPassword));

            UserResult result;

            var userCredentials = GetUserCredentials(username);

            if (userCredentials == null)
            {
                // User does not exist
                result = new UserResult(false);

                return result;
            }

            var isPasswordMatch = IsPasswordMatch(userCredentials, existingPassword);

            if (!isPasswordMatch)
            {
                result = new UserResult(false);

                result.AddValidationMessage(string.Empty, _stringLocalizer.GetString("ExistingPasswordIncorrect"));
            }

            throw new NotImplementedException();
        }

        public UserResult ValidatePassword(string username, string password)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(username));
            Contract.Requires(!string.IsNullOrWhiteSpace(password)); // Intentionally no exception

            UserResult result;

            if (string.IsNullOrWhiteSpace(password))
            {
                result = new UserResult(false);

                result.AddValidationMessage(string.Empty, _stringLocalizer.GetString("UsernamePasswordIncorrect"));

                return result;
            }

            var userCredentials = GetUserCredentials(username);

            if (userCredentials == null)
            {
                // User does not exist
                result = new UserResult(false);

                result.AddValidationMessage(string.Empty, _stringLocalizer.GetString("UsernamePasswordIncorrect"));

                return result;
            }

            var isPasswordMatch = IsPasswordMatch(userCredentials, password);

            if (isPasswordMatch)
            {
                return new UserResult(true);
            }

            result = new UserResult(false);

            result.AddValidationMessage(string.Empty, _stringLocalizer.GetString("UsernamePasswordIncorrect"));

            return result;
        }

        private UserCredentials GetUserCredentials(string username)
        {
            var getUserCredentialsCommand = new SqlCommand("security.GetUserCredentials")
            {
                CommandType = CommandType.StoredProcedure,
                Connection = _securitySqlConnectionWrapper.SqlConnection
            };

            getUserCredentialsCommand.Parameters.AddWithValue("userName", username);

            using (var userCredentials = getUserCredentialsCommand.ExecuteReader())
            {
                if (userCredentials.Read())
                {
                    var passwordHashIndex = userCredentials.GetOrdinal("PasswordHash");

                    var passwordHash = userCredentials.GetString(passwordHashIndex);

                    var passwordSaltIndex = userCredentials.GetOrdinal("PasswordSalt");

                    var passwordSalt = userCredentials.GetString(passwordSaltIndex);

                    var hashIterationsIndex = userCredentials.GetOrdinal("HashIterations");

                    var hashIterations = userCredentials.GetInt32(hashIterationsIndex);

                    return new UserCredentials(passwordHash, passwordSalt, hashIterations);
                }

                return null; // User does not exist
            }
        }

        private static bool IsPasswordMatch(UserCredentials userCredentials, string suppliedPassword)
        {
            var databaseHashedUserPassword = Convert.FromBase64String(userCredentials.PasswordHash);

            var databaseUserSalt = Convert.FromBase64String(userCredentials.PasswordSalt);

            var providedPasswordHash = UserExtensions.HashPassword(Encoding.UTF8.GetBytes(suppliedPassword), databaseUserSalt, userCredentials.HashIterations);

            return UserExtensions.PasswordHashesEqual(databaseHashedUserPassword, providedPasswordHash);
        }
    }
}