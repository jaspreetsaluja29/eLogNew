using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class DatabaseHelper
{
    private readonly string _connectionString;

    public DatabaseHelper(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    // CreateConnection Method (Fixes the issue in CodeAController)
    public SqlConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }

    // Execute Query (Returns a DataTable)
    public DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
    {
        using SqlConnection conn = new SqlConnection(_connectionString);
        using SqlCommand cmd = new SqlCommand(query, conn);
        if (parameters != null) cmd.Parameters.AddRange(parameters);

        using SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        return dt;
    }

    // Execute Non-Query (Insert, Update, Delete)
    public int ExecuteNonQuery(string query, CommandType storedProcedure, SqlParameter[] parameters = null)
    {
        using SqlConnection conn = new SqlConnection(_connectionString);
        conn.Open();
        using SqlCommand cmd = new SqlCommand(query, conn);
        if (parameters != null) cmd.Parameters.AddRange(parameters);
        return cmd.ExecuteNonQuery();
    }

    // Execute Stored Procedure (Returns DataTable)
    public DataTable ExecuteStoredProcedure(string storedProcedureName, SqlParameter[] parameters = null)
    {
        using SqlConnection conn = new SqlConnection(_connectionString);
        using SqlCommand cmd = new SqlCommand(storedProcedureName, conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        if (parameters != null) cmd.Parameters.AddRange(parameters);

        conn.Open(); // Ensure connection is opened
        using SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        return dt;
    }

    // Async Version of ExecuteStoredProcedure
    public async Task<DataTable> ExecuteStoredProcedureAsync(string storedProcedureName, SqlParameter[] parameters = null)
    {
        using SqlConnection conn = new SqlConnection(_connectionString);
        using SqlCommand cmd = new SqlCommand(storedProcedureName, conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        if (parameters != null) cmd.Parameters.AddRange(parameters);

        await conn.OpenAsync(); // Open connection asynchronously
        using SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        return dt;
    }
}
