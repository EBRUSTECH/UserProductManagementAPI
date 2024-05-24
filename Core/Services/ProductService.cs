using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserProduct.Core.Abstractions;
using UserProduct.Core.Dtos;
using UserProduct.Domain.Entities;
using Core;

namespace UserProduct.Core.Services
{
    public class ProductService
    {
        private readonly IRepository _repository;

        public ProductService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task AddProduct(ProductDto productDto)
        {
            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            await _repository.Add(product);
        }

        public async Task<PaginatorDto<IEnumerable<ProductDto>>> GetProducts(PaginationFilter paginationFilter, string userId)
        {
            var productsQuery = _repository.GetAll<Product>().Where(p => p.UserId == userId);

            var pagedProducts = await productsQuery
                .Select(p => new ProductDto
                {
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price
                })
                .Paginate(paginationFilter);

            return pagedProducts;
        }

        public async Task UpdateProduct(string id, ProductDto productDto)
        {
            var existingProduct = await _repository.FindById<Product>(id);
            if (existingProduct == null)
            {
                throw new Exception("Product not found");
            }

            existingProduct.Name = productDto.Name;
            existingProduct.Description = productDto.Description;
            existingProduct.Price = productDto.Price;
            existingProduct.UpdatedAt = DateTimeOffset.UtcNow;

            _repository.Update(existingProduct);
        }

        public async Task DeleteProduct(string id)
        {
            var existingProduct = await _repository.FindById<Product>(id);
            if (existingProduct == null)
            {
                throw new Exception("Product not found");
            }

            _repository.Remove(existingProduct);
        }
    }
}
