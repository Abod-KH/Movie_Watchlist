using Microsoft.Data.SqlClient;
using Movie_Watchlist.Data;

namespace Movie_Watchlist.Repositories
{
    public class Repository
    {
        protected readonly SqlConnectionFactory _connectionFactory;

        protected Repository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        protected T MapReaderToObject<T>(SqlDataReader reader) where T : new()
        {
            var obj = new T();
            var properties = typeof(T).GetProperties();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                var columnValue = reader.GetValue(i);

                // Find a property that matches the column name
                var property = properties.FirstOrDefault(p => p.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));

                if (property != null && columnValue != DBNull.Value)
                {
                    property.SetValue(obj, columnValue);
                }
            }
            return obj;
        }
    }
}
