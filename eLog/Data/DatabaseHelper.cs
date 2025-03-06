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
    public int ExecuteNonQuery(string query, CommandType commandType, SqlParameter[] parameters = null)
    {
        using SqlConnection conn = new SqlConnection(_connectionString);
        conn.Open();
        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.CommandType = commandType;

        if (parameters != null)
        {
            cmd.Parameters.AddRange(parameters);
        }

        return cmd.ExecuteNonQuery(); // Executes the command and returns the number of affected rows
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

    // Execute Stored Procedure for Insert (Returns Number of Rows Affected)
    public int ExecuteInsertStoredProcedure(string storedProcedureName, SqlParameter[] parameters = null)
    {
        using SqlConnection conn = new SqlConnection(_connectionString);
        using SqlCommand cmd = new SqlCommand(storedProcedureName, conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        if (parameters != null)
        {
            cmd.Parameters.AddRange(parameters);
        }

        // Add the OUTPUT parameter to the command
        SqlParameter outputParam = new SqlParameter("@InsertedID", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        cmd.Parameters.Add(outputParam);

        conn.Open(); // Open connection
        cmd.ExecuteNonQuery(); // Execute the procedure

        // Return the inserted ID
        return (int)outputParam.Value;
    }


    // Async Version of ExecuteInsertStoredProcedure (Returns Number of Rows Affected)
    public async Task<int> ExecuteInsertStoredProcedureAsync(string storedProcedureName, SqlParameter[] parameters = null)
    {
        using SqlConnection conn = new SqlConnection(_connectionString);
        using SqlCommand cmd = new SqlCommand(storedProcedureName, conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        if (parameters != null)
        {
            cmd.Parameters.AddRange(parameters);
        }

        await conn.OpenAsync(); // Open connection asynchronously
        return await cmd.ExecuteNonQueryAsync(); // Executes asynchronously and returns number of rows affected
    }

    // Execute Stored Procedure for Update (Returns Number of Rows Affected)
    public int ExecuteUpdateStoredProcedure(string storedProcedureName, SqlParameter[] parameters = null)
    {
        using SqlConnection conn = new SqlConnection(_connectionString);
        using SqlCommand cmd = new SqlCommand(storedProcedureName, conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        if (parameters != null)
        {
            cmd.Parameters.AddRange(parameters);
        }

        conn.Open(); // Open connection
        return cmd.ExecuteNonQuery(); // Execute the procedure and return affected rows
    }

    // Async Version of ExecuteUpdateStoredProcedure (Returns Number of Rows Affected)
    public async Task<int> ExecuteUpdateStoredProcedureAsync(string storedProcedureName, SqlParameter[] parameters = null)
    {
        using SqlConnection conn = new SqlConnection(_connectionString);
        using SqlCommand cmd = new SqlCommand(storedProcedureName, conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        if (parameters != null)
        {
            cmd.Parameters.AddRange(parameters);
        }

        await conn.OpenAsync(); // Open connection asynchronously
        return await cmd.ExecuteNonQueryAsync(); // Execute asynchronously and return affected rows
    }

}
