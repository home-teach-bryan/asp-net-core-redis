using AspNetCoreRedis.DbContext;

namespace AspNetCoreRedis.Services;

public interface IUserService
{
    (bool isValid, User user) IsValid(string name, string password);
    bool AddUser(string requestName, string requestPassword, string[] requestRoles);
}