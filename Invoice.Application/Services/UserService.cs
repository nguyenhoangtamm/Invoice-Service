using System.Security.Claims;
using AutoMapper;
using Invoice.Application.Interfaces;
using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Entities;
using Invoice.Domain.Enums;
using Invoice.Domain.Interfaces;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProfileEntity = Invoice.Domain.Entities.Profile;

namespace Invoice.Application.Services;

public class UserService : BaseService, IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<UserService> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUserRepository userRepository)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<int>> Create(CreateUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating user with username: {request.Username}");

            // Check if username already exists
            var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
            if (existingUser != null)
            {
                return Result<int>.Failure("Username already exists");
            }

            // Check if email already exists
            var existingEmail = await _userRepository.GetByEmailAsync(request.Email);
            if (existingEmail != null)
            {
                return Result<int>.Failure("Email already exists");
            }

            // Create user entity
            var user = new User
            {
                UserName = request.Username, // Identity uses UserName instead of Username
                Email = request.Email,
                RoleId = request.RoleId,
                Status = UserStatus.Active,
                EmailConfirmed = true,
                CreatedDate = DateTime.UtcNow,
            };

            // Create user with password
            await _userRepository.CreateAsync(user, request.Password);

            // Create profile
            var profile = new ProfileEntity
            {
                UserId = user.Id,
                Fullname = $"{request.FirstName} {request.LastName}".Trim(),
                Email = request.Email,
                Gender = request.Gender.ToString(),
                BirthDate = DateTime.MinValue, // Default birth date to avoid null
                Address = string.Empty, // Use empty string to avoid not-null constraint
                Bio = string.Empty, // Use empty string to avoid not-null constraint
                PhoneNumber = string.Empty, // Use empty string to avoid not-null constraint
                AvatarUrl = string.Empty, // Use empty string to avoid not-null constraint
                CreatedDate = DateTime.UtcNow,
            };

            var profileRepo = _unitOfWork.Repository<ProfileEntity>();
            await profileRepo.AddAsync(profile);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"User created successfully with ID: {user.Id}");
            return Result<int>.Success(user.Id, "User created successfully");
        }
        catch (Exception ex)
        {
            LogError("Error creating user", ex);
            return Result<int>.Failure("An error occurred while creating the user");
        }
    }

    public async Task<Result<int>> Update(int id, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating user with ID: {id}");

            // Get existing user
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return Result<int>.Failure("User not found");
            }

            // Check if username already exists (if changing username)
            if (!string.IsNullOrEmpty(request.Username) && request.Username != user.UserName)
            {
                var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
                if (existingUser != null && existingUser.Id != id)
                {
                    return Result<int>.Failure("Username already exists");
                }
            }

            // Check if email already exists (if changing email)
            if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
            {
                var existingEmail = await _userRepository.GetByEmailAsync(request.Email);
                if (existingEmail != null && existingEmail.Id != id)
                {
                    return Result<int>.Failure("Email already exists");
                }
            }

            // Update user properties
            if (!string.IsNullOrEmpty(request.Username))
                user.UserName = request.Username;

            if (!string.IsNullOrEmpty(request.Email))
                user.Email = request.Email;

            if (request.RoleId.HasValue)
                user.RoleId = request.RoleId.Value;

            if (request.Status.HasValue)
                user.Status = request.Status.Value;

            user.UpdatedDate = DateTime.UtcNow;

            // Update profile if FirstName, LastName, or Gender provided
            if (!string.IsNullOrEmpty(request.FirstName) || !string.IsNullOrEmpty(request.LastName) || request.Gender.HasValue)
            {
                var profile = await _unitOfWork.Repository<ProfileEntity>().Entities
                    .FirstOrDefaultAsync(p => p.UserId == id, cancellationToken);

                if (profile != null)
                {
                    // Update name if provided
                    if (!string.IsNullOrEmpty(request.FirstName) || !string.IsNullOrEmpty(request.LastName))
                    {
                        var nameParts = profile.Fullname?.Split(' ') ?? new string[0];
                        var firstName = !string.IsNullOrEmpty(request.FirstName) ? request.FirstName : nameParts.FirstOrDefault() ?? "";
                        var lastName = !string.IsNullOrEmpty(request.LastName) ? request.LastName : nameParts.LastOrDefault() ?? "";

                        profile.Fullname = $"{firstName} {lastName}".Trim();
                    }

                    // Update gender if provided
                    if (request.Gender.HasValue)
                    {
                        profile.Gender = request.Gender.Value.ToString();
                    }

                    profile.UpdatedDate = DateTime.UtcNow;
                }

                await _unitOfWork.Save(cancellationToken);
            }

            LogInformation($"User updated successfully with ID: {id}");
            return Result<int>.Success(id, "User updated successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error updating user with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while updating the user");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting user with ID: {id}");

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return Result<int>.Failure("User not found");
            }

            await _userRepository.DeleteAsync(id);

            LogInformation($"User deleted successfully with ID: {id}");
            return Result<int>.Success(id, "User deleted successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error deleting user with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while deleting the user");
        }
    }

    public async Task<Result<GetUserDto>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting user by ID: {id}");

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return Result<GetUserDto>.Failure("User not found");
            }

            var userDto = _mapper.Map<GetUserDto>(user);

            LogInformation($"User retrieved successfully with ID: {id}");
            return Result<GetUserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            LogError($"Error getting user by ID: {id}", ex);
            return Result<GetUserDto>.Failure("An error occurred while retrieving the user");
        }
    }

    public async Task<Result<List<GetAllUsersDto>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting all users");

            var users = await _userRepository.GetAllAsync();
            var usersDto = _mapper.Map<List<GetAllUsersDto>>(users);

            LogInformation($"Retrieved {users.Count} users successfully");
            return Result<List<GetAllUsersDto>>.Success(usersDto);
        }
        catch (Exception ex)
        {
            LogError("Error getting all users", ex);
            return Result<List<GetAllUsersDto>>.Failure("An error occurred while retrieving users");
        }
    }

    public async Task<PaginatedResult<GetUsersWithPaginationDto>> GetUsersWithPagination(GetUsersWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting users with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var users = await _userRepository.GetPagedAsync(query.PageNumber, query.PageSize);
            var totalCount = await _userRepository.GetCountAsync();

            var usersDto = _mapper.Map<List<GetUsersWithPaginationDto>>(users);

            var result = PaginatedResult<GetUsersWithPaginationDto>.Create(usersDto, totalCount, query.PageNumber, query.PageSize);

            LogInformation($"Retrieved {users.Count} users successfully for page {query.PageNumber}");
            return result;
        }
        catch (Exception ex)
        {
            LogError("Error getting users with pagination", ex);
            throw new Exception("An error occurred while retrieving user with pagination");
        }
    }

    public async Task<Result<GetUserDto>> GetMe(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting current user information");

            // L?y User ID t? JWT token
            var userIdClaim = HttpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                LogError("User ID not found in token or invalid format", null!);
                return Result<GetUserDto>.Failure("User not authenticated or invalid token");
            }

            LogInformation($"Getting current user with ID: {userId}");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                LogError($"User not found with ID: {userId}", null!);
                return Result<GetUserDto>.Failure("User not found");
            }

            var userDto = _mapper.Map<GetUserDto>(user);

            LogInformation($"Current user retrieved successfully with ID: {userId}");
            return Result<GetUserDto>.Success(userDto, "Current user information retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting current user information", ex);
            return Result<GetUserDto>.Failure("An error occurred while retrieving current user information");
        }
    }

    public async Task<Result<DashboardStatsDto>> GetDashboardStats(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting dashboard stats");

            var allUsers = await _userRepository.GetAllAsync();
            var totalUsers = allUsers.Count;
            var activeUsers = allUsers.Count(u => u.Status == UserStatus.Active);
            var inactiveUsers = totalUsers - activeUsers;

            var stats = new DashboardStatsDto
            {
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                InactiveUsers = inactiveUsers
            };

            LogInformation("Dashboard stats retrieved successfully");
            return Result<DashboardStatsDto>.Success(stats);
        }
        catch (Exception ex)
        {
            LogError("Error getting dashboard stats", ex);
            return Result<DashboardStatsDto>.Failure("An error occurred while retrieving dashboard stats");
        }
    }
}

