using System.Threading.Tasks;
using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redisCache;

        public BasketRepository(IDistributedCache redisCache)
        {
            _redisCache = redisCache;
        }
        public async Task<ShoppingCart> GetShoppingCart(string userName)
        {
            var cart = await _redisCache.GetStringAsync(userName);
            if (string.IsNullOrEmpty(cart)) return null;
            return JsonConvert.DeserializeObject<ShoppingCart>(cart);
        }

        public async Task<ShoppingCart> UpdateShoppingCart(ShoppingCart shoppingCart)
        {
            await _redisCache.SetStringAsync(shoppingCart.UserName, JsonConvert.SerializeObject(shoppingCart));
            return await GetShoppingCart(shoppingCart.UserName);
        }

        public async Task DeleteShoppingCart(string userName)
        {
            await _redisCache.RemoveAsync(userName);
        }
    }
}