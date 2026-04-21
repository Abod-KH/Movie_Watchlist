using Microsoft.Data.SqlClient;
using System.Data;


namespace Movie_Watchlist.Infrastructure.Repositories
{
    public class Repository
    {
        protected readonly SqlConnectionFactory _connectionFactory;

        protected Repository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private static SqlCommand CreateCommand(string storedProcedure, SqlConnection connection, params (string name, object? value)[] parameters)
        {
            var command = new SqlCommand(storedProcedure, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.name, param.value ?? DBNull.Value);
            }
            return command;
        }

        private async Task<TResult> ExecuteWithCommandAsync<TResult>(
            string storedProcedure,
            Func<SqlCommand, Task<TResult>> action,
            params (string name, object? value)[] parameters)
        {
            
            using var connection = _connectionFactory.CreateConnection();
            using var command = CreateCommand(storedProcedure, connection, parameters);
            await connection.OpenAsync();
            return await action(command);
        }

        protected async Task ExecuteNonQueryAsync(string storedProcedure, params (string name, object? value)[] parameters)
        {

            await ExecuteWithCommandAsync(storedProcedure, async command =>
            {
                return await command.ExecuteNonQueryAsync();
            }, parameters);

            /* using var connection = _connectionFactory.CreateConnection();
            using var command = CreateCommand(storedProcedure, connection, parameters);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();*/
        }

        protected async Task<T?> ExecuteScalarAsync<T>(string storedProcedure, params (string name, object? value)[] parameters)
        {
            return await ExecuteWithCommandAsync(storedProcedure, async (command) =>
            {
                var result = await command.ExecuteScalarAsync();
                if (result == null || result == DBNull.Value)
                {
                    return default;
                }
                return (T)Convert.ChangeType(result, typeof(T));
            }, parameters);

  
        }

        protected async Task<T?> ExecuteQuerySingleAsync<T>(string storedProcedure, params (string name, object? value)[] parameters) where T : new()
        {
            return await ExecuteWithCommandAsync(storedProcedure, async (command) =>
            {
                using var reader = await command.ExecuteReaderAsync();
                return await reader.ReadAsync() ? MapReaderToObject<T>(reader) : default;


            }, parameters);
            // using var connection = _connectionFactory.CreateConnection();
            // using var command = CreateCommand(storedProcedure, connection, parameters);
            // await connection.OpenAsync();
            // using var reader = await command.ExecuteReaderAsync();
            // return await reader.ReadAsync() ? MapReaderToObject<T>(reader) : default;
        }

        protected async Task<List<T>> ExecuteQueryListAsync<T>(string storedProcedure, params (string name, object? value)[] parameters) where T : new()
        {

            return await ExecuteWithCommandAsync(storedProcedure, async (command) =>
            {
                var list = new List<T>();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(MapReaderToObject<T>(reader));
                }
                return list;
            }, parameters);

        }

        protected async Task ExecuteTableValueNonQueryAsync(string storedProcedure, string paramName, DataTable table, string typeName)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(storedProcedure, connection);
            command.CommandType = CommandType.StoredProcedure;

            var param = command.Parameters.AddWithValue(paramName, table);
            param.SqlDbType = SqlDbType.Structured;
            param.TypeName = typeName;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        protected static T MapReaderToObject<T>(SqlDataReader reader) where T : new()
        {
            var obj = new T();
            var properties = typeof(T).GetProperties();
            foreach (var property in properties) { Console.WriteLine($"properties:{property}"); }
           
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                var columnValue = reader.GetValue(i);
                
                Console.WriteLine($"Column: {columnName},\n Value: {columnValue}");
                // Find a property that matches the column name
                var property = properties.FirstOrDefault(p => p.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));
                Console.WriteLine($"Property: {property}");

                if (property != null && columnValue != DBNull.Value)
                {
                    property.SetValue(obj, columnValue);
                }
            }
            return obj;
        }
    }
}
