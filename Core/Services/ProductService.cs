using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserProduct.Core.Abstractions;
using UserProduct.Core.Dtos;
using UserProduct.Domain.Entities;

namespace UserProduct.Core.Services
{
    public class ProductService
    {
        private readonly IRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task AddProduct(ProductDto productDto)
        {
            var product = new Product
            {
                UserId = productDto.UserId,
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            await _repository.Add(product);
            await _unitOfWork.SaveChangesAsync();
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
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteProduct(string id)
        {
            var existingProduct = await _repository.FindById<Product>(id);
            if (existingProduct == null)
            {
                throw new Exception("Product not found");
            }

            _repository.Remove(existingProduct);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
