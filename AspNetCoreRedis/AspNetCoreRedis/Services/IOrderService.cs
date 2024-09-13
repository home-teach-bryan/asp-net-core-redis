using AspNetCoreRedis.Models.Request;
using AspNetCoreRedis.Models.Response;

namespace AspNetCoreRedis.Services;

public interface IOrderService
{
    bool AddOrder(List<AddOrderRequest> addOrderRequests, Guid userId);
    IEnumerable<GetOrderDetailsResponse> GetOrderDetails(Guid userId);
}