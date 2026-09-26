using System.Net;
using AutoMapper;
using Microservices.Services.ProductAPI.Data;
using Microservices.Services.ProductAPI.Models;
using Microservices.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Microservices.Services.ProductAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(AppDbContext context, IMapper mapper, IWebHostEnvironment environment) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
    {
        var result = await context.Products.AsNoTracking().ToListAsync(cancellationToken);
        return Ok(ResultDto<IEnumerable<ProductDto>>.SuccessResult(mapper.Map<List<ProductDto>>(result)));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken = default)
    {
        var product = await context.Products.FindAsync(id, cancellationToken);
        if (product is null) return NotFound(ResultDto<ProductDto>.FailureResult($"No product found with id = {id}"));
        return Ok(ResultDto<ProductDto>.SuccessResult(mapper.Map<ProductDto>(product)));
    }

    //[HttpGet("GetByCode/{code}")]
    //public async Task<IActionResult> Get(string code, CancellationToken cancellationToken = default)
    //{
    //    var product = await context.Products.AsNoTracking().FirstOrDefaultAsync(c => c.ProductCode.Trim().ToLower() == code.Trim().ToLower(), cancellationToken);
    //    if (product is null) return NotFound(ResultDto<ProductDto>.FailureResult($"No product found with code = {code}"));
    //    return Ok(ResultDto<ProductDto>.SuccessResult(mapper.Map<ProductDto>(product)));
    //}

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Post(ProductDto productDto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ResultDto<ProductDto>.FailureResult("Invalid Data"));

        var product = mapper.Map<Product>(productDto);

        context.Products.Add(product);
        await context.SaveChangesAsync(cancellationToken);
        if (productDto.Image is not null)
        {
            var fileName = $"{product.ProductId}{Path.GetExtension(productDto.Image.FileName)}";
            var filePathDir = Path.Combine(environment.WebRootPath, "ProductImages");
            var filePath = Path.Combine(filePathDir, fileName);
            if (!Directory.Exists(filePathDir))
            {
                Directory.CreateDirectory(filePathDir);
            }
            using var stream = new FileStream(filePath, FileMode.Create);
            productDto.Image.CopyTo(stream);
            product.ImageLocalPath = filePath;
            var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            product.ImageUrl = $"{baseUrl}/ProductImages/{fileName}";
        }
        else
        {
            product.ImageUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/ProductImages/default.png";
        }
        await context.SaveChangesAsync(cancellationToken);

        ResultDto<ProductDto> ResultDto = ResultDto<ProductDto>.SuccessResult(mapper.Map<ProductDto>(product));
        return StatusCode((int)HttpStatusCode.Created, ResultDto);
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Put(ProductDto productDto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ResultDto<ProductDto>.FailureResult("Invalid Data"));

        var productDB = await context.Products.AsNoTracking().Where(e => e.ProductId == productDto.ProductId).FirstOrDefaultAsync(cancellationToken);

        var product = mapper.Map<Product>(productDto);

        if (productDto.Image is not null)
        {
            var fileName = $"{product.ProductId}{Path.GetExtension(productDto.Image.FileName)}";
            var filePathDir = Path.Combine(environment.WebRootPath, "ProductImages");
            var filePath = Path.Combine(filePathDir, fileName);
            if (!Directory.Exists(filePathDir))
            {
                Directory.CreateDirectory(filePathDir);
            }
            using var stream = new FileStream(filePath, FileMode.Create);
            productDto.Image.CopyTo(stream);

            if (!string.IsNullOrWhiteSpace(productDB?.ImageLocalPath))
            {
                var file = new FileInfo(productDB.ImageLocalPath);
                if (file.Exists)
                {
                    file.Delete();
                }
            }
            product.ImageLocalPath = filePath;
            var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            product.ImageUrl = $"{baseUrl}/ProductImages/{fileName}";
        }

        context.Products.Update(product);
        await context.SaveChangesAsync(cancellationToken);
        ResultDto<ProductDto> ResultDto = ResultDto<ProductDto>.SuccessResult(mapper.Map<ProductDto>(product));
        return Ok(ResultDto);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var product = await context.Products.FirstOrDefaultAsync(c => c.ProductId == id, cancellationToken);
        if (product is null)
            return NotFound(ResultDto<ProductDto>.FailureResult($"Could not find product with id {id}"));
        context.Products.Remove(product);
        var rowsaffcted = await context.SaveChangesAsync(cancellationToken);
        if (rowsaffcted == 0)
            return StatusCode((int)HttpStatusCode.InternalServerError, ResultDto<ProductDto>.FailureResult($"Could not delete product with id {id}"));
        if (!string.IsNullOrWhiteSpace(product.ImageLocalPath))
        {
            var file = new FileInfo(product.ImageLocalPath);
            if (file.Exists)
            {
                file.Delete();
            }
        }
        return Ok(ResultDto<bool>.SuccessResult(true, $"Product with id {id} deleted successfully"));
    }
}
