using Microsoft.AspNetCore.Http;
using Npgsql;

namespace Discount.Grpc.Extensions
{
    public static class CommandExtensions
    {
        public static void ExecuteNonQuery(this NpgsqlCommand command, string commandText)
        {
            command.CommandText = commandText;
            command.ExecuteNonQuery();
        }
    }
}
