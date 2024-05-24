using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using UserProduct.Core.Dtos;
using UserProduct.Core.Services;

namespace UserProduct.Api.Controllers
{
    [ApiController]
    [Route("api/v1/products")]
    /*[Authorize]*/
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductDto productDto)
        {
            await _productService.AddProduct(productDto);
            return Ok(new { message = "Product added successfully" });
        }

        [HttpGet]
        public async Task<ActionResult<PaginatorDto<IEnumerable<ProductDto>>>> GetProducts([FromQuery] PaginationFilter paginationFilter)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pagedProducts = await _productService.GetProducts(paginationFilter, userId);
            return Ok(pagedProducts);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(string id, ProductDto productDto)
        {
            await _productService.UpdateProduct(id, productDto);
            return Ok(new { message = "Product updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            await _productService.DeleteProduct(id);
            return Ok(new { message = "Product deleted successfully" });
        }
    }
}
