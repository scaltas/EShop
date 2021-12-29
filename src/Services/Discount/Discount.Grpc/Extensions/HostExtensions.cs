using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Discount.Grpc.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int retry = 0)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var configuration = services.GetRequiredService<IConfiguration>();
            var logger = services.GetRequiredService<ILogger<TContext>>();

            //TODO use Polly
            var retryCount = -1;
            while (retryCount < retry)
            {
                try
                {
                    logger.LogInformation("Migrating postgresql database.");

                    using var connection = new NpgsqlConnection
                        (configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                    connection.Open();

                    using var command = new NpgsqlCommand
                    {
                        Connection = connection
                    };

                    command.ExecuteNonQuery("DROP TABLE IF EXISTS Coupon");
                    command.ExecuteNonQuery(@"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY, 
                                                                ProductName VARCHAR(24) NOT NULL,
                                                                Description TEXT,
                                                                Amount INT)");
                    command.ExecuteNonQuery("INSERT INTO Coupon(ProductName, Description, Amount) VALUES('IPhone X', 'IPhone Discount', 150);");
                    command.ExecuteNonQuery("INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Samsung 10', 'Samsung Discount', 100);");

                    logger.LogInformation("Migrated postgresql database.");
                    break;
                }
                catch (NpgsqlException ex)
                {
                    retryCount++;
                    logger.LogError(ex, "An error occurred while migrating the postgresql database");
                    System.Threading.Thread.Sleep(2000);
                }
                
            }

            return host;
        }
    }
}
