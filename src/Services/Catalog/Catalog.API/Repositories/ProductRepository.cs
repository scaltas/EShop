using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Driver;

namespace Catalog.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ICatalogContext _catalogContext;

        public ProductRepository(ICatalogContext catalogContext)
        {
            _catalogContext = catalogContext;
        }

        public async Task<IEnumerable<Product>> GetProducts()
        { 
            return await _catalogContext.Products.Find(p => true).ToListAsync();
        }

        public async Task<Product> GetProduct(string id)
        {
            var filter = GetFilterDefinition(p => p.Id, id);
            return await _catalogContext.Products.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProductByName(string name)
        {
            var filter = GetFilterDefinition(p => p.Name, name);
            return await _catalogContext.Products.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductByCategory(string category)
        {
            var filter = GetFilterDefinition(p => p.Category, category);
            return await _catalogContext.Products.Find(filter).ToListAsync();
        }

        public async Task CreateProduct(Product product)
        {
            await _catalogContext.Products.InsertOneAsync(product);
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            var filter = GetFilterDefinition(p => p.Id, product.Id);
            var updateResult = await _catalogContext.Products.ReplaceOneAsync(filter, product);

            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }

        public async Task<bool> DeleteProduct(string id)
        {
            var filter = GetFilterDefinition(p => p.Id, id);
            var deleteResult = await _catalogContext.Products.DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        private FilterDefinition<Product> GetFilterDefinition<T>(Expression<Func<Product, T>> field, T value)
        {
            return Builders<Product>.Filter.Eq(field, value);
        }
    }
}