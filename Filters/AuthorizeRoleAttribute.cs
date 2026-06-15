using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace ManagementOfForeigners.Filters;

/// <summary>
/// Filter phân quyền theo vai trò, kiểm tra Claims từ Cookie Authentication
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class AuthorizeRoleAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _roles;

    /// <summary>
    /// Yêu cầu đăng nhập với một trong các vai trò được chỉ định
    /// </summary>
    /// <param name="roles">Danh sách vai trò được phép truy cập</param>
    public AuthorizeRoleAttribute(params string[] roles)
    {
        _roles = roles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        // Kiểm tra đã đăng nhập chưa
        if (user?.Identity == null || !user.Identity.IsAuthenticated)
        {
            context.Result = new RedirectToActionResult("DangNhap", "TaiKhoan", new { returnUrl = context.HttpContext.Request.Path });
            return;
        }

        // Nếu không chỉ định role cụ thể, chỉ cần đăng nhập là đủ
        if (_roles.Length == 0)
            return;

        // Kiểm tra vai trò
        var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
        if (string.IsNullOrEmpty(userRole) || !_roles.Contains(userRole))
        {
            context.Result = new RedirectToActionResult("TuChoiTruyCap", "TaiKhoan", null);
        }
    }
}
