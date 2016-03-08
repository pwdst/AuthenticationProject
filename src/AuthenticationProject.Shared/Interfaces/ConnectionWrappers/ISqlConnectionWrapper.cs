namespace AuthenticationProject.Shared.Interfaces.ConnectionWrappers
{
    using System;
    using System.Data.SqlClient;

    public interface ISqlConnectionWrapper : IDisposable
    {
        SqlConnection SqlConnection { get; }
    }
}
