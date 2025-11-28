using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.API.Controllers;

public class UsersController(ILogger<UsersController> logger, IUserService userService) : ApiControllerBase(logger)
{
    private readonly IUserService _userService = userService;

    [HttpPost("create")]
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating user with username: {request.Username}");

            return await _userService.Create(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error creating user", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while creating the user"));
        }
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (id != id)
            {
                return BadRequest(Result<int>.Failure("ID in route does not match ID in body"));
            }
            LogInformation($"Updating user with ID: {id}");

            var updateRequest = new UpdateUserRequest
            {
                Username = request.Username,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Gender = request.Gender,
                RoleId = request.RoleId,
                Status = request.Status,
                Phone = request.Phone,
                Address = request.Address
            };

            return await _userService.Update(id, updateRequest, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error updating user with ID: {id}", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while updating the user"));
        }
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<ActionResult<Result<int>>> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting user with ID: {id}");

            return await _userService.Delete(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting user with ID: {id}", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while deleting the user"));
        }
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<ActionResult<Result<GetUserDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting user with ID: {id}");

            var result = await _userService.GetById(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting user with ID: {id}", ex);
            return StatusCode(500, Result<GetUserDto>.Failure("An error occurred while retrieving the user"));
        }
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<ActionResult<Result<List<GetAllUsersDto>>>> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting all users");

            return await _userService.GetAll(cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting all users", ex);
            return StatusCode(500, Result<List<GetAllUsersDto>>.Failure("An error occurred while retrieving users"));
        }
    }

    [HttpGet("get-pagination")]
    public async Task<ActionResult<PaginatedResult<GetUsersWithPaginationDto>>> GetUsersWithPagination([FromQuery] GetUsersWithPaginationQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting users with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            return await _userService.GetUsersWithPagination(query, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting users with pagination", ex);
            return StatusCode(500, Result<PaginatedResult<GetUsersWithPaginationDto>>.Failure("An error occurred while retrieving users"));
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<Result<GetUserDto>>> GetMe(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting current user information");

            var result = await _userService.GetMe(cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting current user information", ex);
            return StatusCode(500, Result<GetUserDto>.Failure("An error occurred while retrieving current user information"));
        }
    }

    [HttpGet("dashboard-stats")]
    [Authorize]
    public async Task<ActionResult<Result<DashboardStatsDto>>> GetDashboardStats(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting user dashboard stats");

            return await _userService.GetDashboardStats(cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting dashboard stats", ex);
            return StatusCode(500, Result<DashboardStatsDto>.Failure("An error occurred while retrieving dashboard stats"));
        }
    }
}



