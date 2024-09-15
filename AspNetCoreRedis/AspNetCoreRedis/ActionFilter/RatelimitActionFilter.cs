using AspNetCoreRedis.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StackExchange.Redis;

namespace AspNetCoreRedis.ActionFilter;

public class RatelimitActionFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var connectionMultiplexer = context.HttpContext.RequestServices.GetService<IConnectionMultiplexer>();
        var database = connectionMultiplexer.GetDatabase();
        var path = context.HttpContext.Request.Path.Value;

        var accessKey = $"{RedisKeyConst.AccessCount}:{path}";
        var rateLimitKey = $"{RedisKeyConst.RateLimit}:{path}";
        if (database.KeyExists(rateLimitKey))
        {
            context.Result = new ContentResult
            {
                Content = "Too Many Requests",
                StatusCode = 429
            };
            return;
        }

        var count = database.StringIncrement(accessKey);
        if (count > 10)
        {
            database.StringSet(rateLimitKey, 1);
            database.KeyExpire(rateLimitKey, TimeSpan.FromSeconds(10));
            database.StringSet(accessKey, 0);
            context.Result = new ContentResult
            {
                Content = "Too Many Requests",
                StatusCode = 429
            };
        }
    }
}