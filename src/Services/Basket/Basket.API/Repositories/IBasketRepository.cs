using System.Threading.Tasks;
using Basket.API.Entities;

namespace Basket.API.Repositories
{
    public interface IBasketRepository
    {
        Task<ShoppingCart> GetShoppingCart(string userName);
        Task<ShoppingCart> UpdateShoppingCart(ShoppingCart shoppingCart);
        Task DeleteShoppingCart(string userName);
    }
}
