namespace AuthenticationProject.Data.Database.ConnectionWrappers
{
    using System.Data;
    using System.Data.SqlClient;
    using Shared.Interfaces.ConnectionWrappers;

    public class SqlConnectionWrapper : ISqlConnectionWrapper, ISecuritySqlConnectionWrapper
    {
        public SqlConnectionWrapper(string connectionString)
        {
            SqlConnection = new SqlConnection(connectionString);

            SqlConnection.Open();
        }

        public SqlConnection SqlConnection { get; }

        public void Dispose()
        {
            if (SqlConnection.State == ConnectionState.Open)
            {
                SqlConnection.Close();
            }
        }
    }
}
