using Invoice.Application.Services;
using Invoice.Domain.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Get device info and IP address for security tracking
        var deviceInfo = Request.Headers.UserAgent.ToString();
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        var result = await _authService.LoginAsync(request.UsernameOrEmail, request.Password, deviceInfo, ipAddress);

        if (!result.IsSuccess)
        {
            return BadRequest(new AuthResponse
            {
                Data = new AuthData(),
                Message = result.ErrorMessage ?? "Ðang nh?p th?t b?i"
            });
        }

        var response = new AuthResponse
        {
            success = result.IsSuccess,
            Data = new AuthData
            {
                AccessToken = result.AccessToken!,
                RefreshToken = result.RefreshToken!,
                User = new UserInfo
                {
                    UserName = result.UserName!,
                    FullName = result.FullName!,
                    Role = result.Role!
                }
            },
            Message = "Đăng nhập thành công"
        };

        return Ok(response);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.RegisterAsync(
  request.Email,
      request.Password,
            request.UserName,
  request.Fullname,
     request.Gender,
            request.BirthDate,
   request.Address,
            request.Bio,
     request.PhoneNumber
        );

        if (!result.IsSuccess)
        {
            return BadRequest(new AuthResponse
            {
                Data = new AuthData(),
                Message = result.ErrorMessage ?? "Ðang ký th?t b?i"
            });
        }

        var response = new AuthResponse
        {
            Data = new AuthData
            {
                AccessToken = result.AccessToken!,
                RefreshToken = result.RefreshToken!,
                User = new UserInfo
                {
                    UserName = result.UserName!,
                    FullName = result.FullName!,
                    Role = result.Role!
                }
            },
            Message = "Ðang ký thành công"
        };

        return Ok(response);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        // Get access token from Authorization header for blacklisting
        var accessToken = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

        var result = await _authService.LogoutAsync(userId, accessToken);

        if (!result)
        {
            return BadRequest(new { message = "Ðang xu?t th?t b?i" });
        }

        return Ok(new { message = "Ðang xu?t thành công" });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var user = await _authService.GetUserByIdAsync(userId);

        if (user == null)
        {
            return NotFound(new { message = "Không tìm th?y ngu?i dùng" });
        }

        return Ok(new
        {
            id = user.Id,
            userName = user.UserName,
            email = user.Email,
            roleId = user.RoleId,
            status = user.Status
        });
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.RefreshTokenAsync(request.RefreshToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new AuthResponse
            {
                Data = new AuthData(),
                Message = result.ErrorMessage ?? "Làm m?i token th?t b?i"
            });
        }

        var response = new AuthResponse
        {
            Data = new AuthData
            {
                AccessToken = result.AccessToken!,
                RefreshToken = result.RefreshToken!,
                User = new UserInfo
                {
                    UserName = result.UserName!,
                    FullName = result.FullName!,
                    Role = result.Role!
                }
            },
            Message = "Làm m?i token thành công"
        };

        return Ok(response);
    }
}


