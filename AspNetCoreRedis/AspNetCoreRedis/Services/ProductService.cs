using System.Text.Json;
using AspNetCoreRedis.DbContext;
using AspNetCoreRedis.Models;
using AspNetCoreRedis.Models.Request;
using AspNetCoreRedis.Models.Response;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Order = StackExchange.Redis.Order;

namespace AspNetCoreRedis.Services;

public class ProductService : IProductService
{
    private readonly ProductDbContext _dbContext;
    private readonly IDatabase _database;


    public ProductService(ProductDbContext dbContext, IConnectionMultiplexer connectionMultiplexer)
    {
        _dbContext = dbContext;
        _database = connectionMultiplexer.GetDatabase();
    }


    public bool AddProduct(AddProductRequest product)
    {
        if (_dbContext.Products.Any(item => item.Name == product.Name))
        {
            return false;
        }
        var newProduct = new Product
        {
            Name = product.Name,
            Price = product.Price,
            Quantity = product.Quantity,
            Created = DateTime.Now,
            Updated = DateTime.Now
        };
        _dbContext.Products.Add(newProduct);
        _dbContext.SaveChanges();
        FlushCache(RedisKeyConst.Products);
        
        return true;
    }

    public bool UpdateProduct(Guid id, UpdateProductRequest product)
    {
        var existProduct = _dbContext.Products.FirstOrDefault(item => item.Id == id);
        if (existProduct == null)
        {
            return false;
        }
        existProduct.Name = product.Name;
        existProduct.Price = product.Price;
        existProduct.Quantity = product.Quantity;
        existProduct.Updated = DateTime.Now;
        _dbContext.SaveChanges();
        FlushCache(RedisKeyConst.Products);
        return true;
    }

    public bool RemoveProduct(Guid id)
    {
        var existProduct = _dbContext.Products.FirstOrDefault(item => item.Id == id);
        if (existProduct == null)
        {
            return false;
        }
        _dbContext.Products.Remove(existProduct);
        _dbContext.SaveChanges();
        FlushCache(RedisKeyConst.Products);
        return true;
    }

    public List<GetProductResponse> GetAllProducts()
    {
        List<Product> products;
        if (_database.KeyExists(RedisKeyConst.Products))
        {
            var productsJson = _database.StringGet(RedisKeyConst.Products);
            products = JsonSerializer.Deserialize<List<Product>>(productsJson);
        }
        else
        {
            products = _dbContext.Products.AsNoTracking().ToList();
            _database.StringSet(RedisKeyConst.Products, JsonSerializer.Serialize(products));
        }
        return products.Select(item => new GetProductResponse
        {
            Id = item.Id,
            Name = item.Name,
            Price = item.Price,
            Quantity = item.Quantity
        }).ToList();
    }

    public GetProductResponse GetProduct(Guid id)
    {
        var existProduct = _dbContext.Products.AsNoTracking().FirstOrDefault(item => item.Id == id);
        if (existProduct == null)
        {
            return new GetProductResponse();
        }
        return new GetProductResponse
        {
            Id = existProduct.Id,
            Name = existProduct.Name,
            Price = existProduct.Price,
            Quantity = existProduct.Quantity
        };
    }

    public List<GetProductSlaesRankingResponse> GetProductSlaesRanking()
    {
        var result =
            _database.SortedSetRangeByRankWithScores($"{RedisKeyConst.Products}:SaleRanking", order: Order.Descending);
        return result.Select(item => new GetProductSlaesRankingResponse
        {
            Name = item.Element.ToString(),
            Quantity = item.Score
        }).ToList();
    }
    private void FlushCache(string redisKey)
    {
        this._database.KeyDelete(redisKey);
    }
}

