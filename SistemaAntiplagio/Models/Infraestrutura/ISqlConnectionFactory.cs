using System.Data.SqlClient;
public interface ISqlConnectionFactory
{
    SqlConnection CreateConnection();
}