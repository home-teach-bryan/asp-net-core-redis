﻿using AspNetCoreRedis.Models.Enum;
using AspNetCoreRedis.Models.Request;
using AspNetCoreRedis.Models.Response;
using AspNetCoreRedis.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreRedis.Controllers;

/// <summary>
/// 產品控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    /// <summary>
    /// 產品服務
    /// </summary>
    private readonly IProductService _productService;

    /// <summary>
    /// 產品控制器
    /// </summary>
    /// <param name="productService"></param>
    public ProductController(IProductService productService)
    {
        _productService = productService;
    }


    /// <summary>
    /// 新增產品
    /// </summary>
    /// <param name="request"></param>
    /// <returns>回傳執行結果</returns>
    /// <response code="200">成功</response>
    /// <response code="400">失敗</response>
    [HttpPost]
    [Route("")]
    [Authorize(Roles = "Admin")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType<ApiResponse<object>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResponse<object>>(StatusCodes.Status400BadRequest)]
    public IActionResult AddProduct([FromBody] AddProductRequest request)
    {
        var result = _productService.AddProduct(request);
        var status = result ? ApiResponseStatus.Success : ApiResponseStatus.Fail;
        if (!result)
        {
            return BadRequest(new ApiResponse<object>(status));
        }
        return Ok(new ApiResponse<object>(status));
    }

    /// <summary>
    /// 更新產品
    /// </summary>
    /// <param name="id">產品ID</param>
    /// <param name="request">產品內容</param>
    /// <returns>回傳執行結果</returns>
    [HttpPut]
    [Route("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult UpdateProduct([FromRoute] Guid id, [FromBody] UpdateProductRequest request)
    {
        var result = _productService.UpdateProduct(id, request);
        var status = result ? ApiResponseStatus.Success : ApiResponseStatus.Fail;
        if (!result)
        {
            return BadRequest(new ApiResponse<object>(status));
        }
        return Ok(new ApiResponse<object>(status));
    }

    /// <summary>
    /// 刪除產品
    /// </summary>
    /// <param name="id">產品ID</param>
    /// <returns>回傳執行結果</returns>
    [HttpDelete]
    [Route("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteProduct([FromRoute] Guid id)
    {
        var result = _productService.RemoveProduct(id);
        var status = result ? ApiResponseStatus.Success : ApiResponseStatus.Fail;
        if (!result)
        {
            return BadRequest(new ApiResponse<object>(status));
        }
        return Ok(new ApiResponse<object>(status));
    }

    /// <summary>
    /// 取得所有產品
    /// </summary>
    /// <returns>產品清單</returns>
    [HttpGet]
    [Route("")]
    [Authorize(Roles = "Admin,User")]
    public IActionResult GetProducts()
    {
        var result = _productService.GetAllProducts();
        return Ok(new ApiResponse<object>(ApiResponseStatus.Success)
        {
            Data = result
        });
    }

    /// <summary>
    /// 取得單一產品
    /// </summary>
    /// <param name="id">產品ID</param>
    /// <returns>產品資料</returns>
    [HttpGet]
    [Route("{id}")]
    [Authorize(Roles = "Admin,User")]
    public IActionResult GetProduct([FromRoute] Guid id)
    {
        var result = _productService.GetProduct(id);
        return Ok(new ApiResponse<object>(ApiResponseStatus.Success)
        {
            Data = result
        });
    }

    /// <summary>
    /// 取得商品銷售排行
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("Sales/Ranking")]
    [Authorize(Roles = "Admin,User")]
    public IActionResult GetProductSlaesRanking()
    {
        var result = _productService.GetProductSlaesRanking();
        return Ok(new ApiResponse<object>(ApiResponseStatus.Success)
        {
            Data = result
        });
    }
}