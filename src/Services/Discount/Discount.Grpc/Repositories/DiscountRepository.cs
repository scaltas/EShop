using System.Threading.Tasks;
using Dapper;
using Discount.Grpc.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Discount.Grpc.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration _configuration;
        private readonly NpgsqlConnection _connection;
        public DiscountRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
        }
        public async Task<Coupon> GetDiscount(string productName)
        {
            var coupon = await _connection.QueryFirstOrDefaultAsync<Coupon>
                ("SELECT * FROM Coupon Where ProductName = @ProductName", new {ProductName = productName});

            if (coupon == null)
                return new Coupon()
                {
                    ProductName = "No Discount", Amount = 0, Description = "No Discount"
                };

            return coupon;
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            var affected =
                await _connection.ExecuteAsync
                ("INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)",
                    new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });

            return affected != 0;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            var affected = await _connection.ExecuteAsync
            ("UPDATE Coupon SET ProductName=@ProductName, Description = @Description, Amount = @Amount WHERE Id = @Id",
                new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount, Id = coupon.Id });

            return affected != 0;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            var affected = await _connection.ExecuteAsync("DELETE FROM Coupon WHERE ProductName = @ProductName",
                new { ProductName = productName });

            return affected != 0;
        }
    }
}