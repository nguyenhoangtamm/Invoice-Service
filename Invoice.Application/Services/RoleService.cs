using AutoMapper;
using Invoice.Application.Interfaces;
using Invoice.Application.Services;
using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Entities;
using Invoice.Domain.Interfaces;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Invoice.Application.Services;

public class RoleService : BaseService, IRoleService
{
    private readonly IUserRepository _userRepository;
    private readonly RoleManager<Role> _roleManager;

    public RoleService(IHttpContextAccessor httpContextAccessor, ILogger<RoleService> logger,
        IUnitOfWork unitOfWork, IMapper mapper, IUserRepository userRepository, RoleManager<Role> roleManager)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
        _userRepository = userRepository;
        _roleManager = roleManager;
    }

    public async Task<Result<int>> Create(CreateRoleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating role with name: {request.Name}");

            var role = new Role
            {
                Name = request.Name,
                Description = request.Description,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                var errorException = new InvalidOperationException($"Failed to create role: {errors}");
                LogError($"Failed to create role with name: {request.Name}. Errors: {errors}", errorException);
                return Result<int>.Failure($"Failed to create role: {errors}");
            }

            LogInformation($"Role created successfully with ID: {role.Id}");
            return Result<int>.Success(role.Id, "Role created successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error creating role with name: {request.Name}", ex);
            return Result<int>.Failure("An error occurred while creating the role");
        }
    }

    public async Task<Result<int>> Update(int id, UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating role with ID: {id}");

            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return Result<int>.Failure("Role not found");
            }

            role.Name = request.Name;
            role.Description = request.Description;
            role.UpdatedDate = DateTime.UtcNow;

            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                var errorException = new InvalidOperationException($"Failed to update role: {errors}");
                LogError($"Failed to update role with ID: {id}. Errors: {errors}", errorException);
                return Result<int>.Failure($"Failed to update role: {errors}");
            }

            LogInformation($"Role updated successfully with ID: {id}");
            return Result<int>.Success(role.Id, "Role updated successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error updating role with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while updating the role");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting role with ID: {id}");

            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return Result<int>.Failure("Role not found");
            }

            role.IsDeleted = true;
            role.UpdatedDate = DateTime.UtcNow;

            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                var errorException = new InvalidOperationException($"Failed to delete role: {errors}");
                LogError($"Failed to delete role with ID: {id}. Errors: {errors}", errorException);
                return Result<int>.Failure($"Failed to delete role: {errors}");
            }

            LogInformation($"Role deleted successfully with ID: {id}");
            return Result<int>.Success(id, "Role deleted successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error deleting role with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while deleting the role");
        }
    }

    public async Task<Result<object>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting role with ID: {id}");

            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return Result<object>.Failure("Role not found");
            }

            var result = new
            {
                role.Id,
                role.Name,
                role.Description,
                role.CreatedBy,
                role.CreatedDate,
                role.UpdatedBy,
                role.UpdatedDate
            };

            return Result<object>.Success(result, "Role retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error getting role with ID: {id}", ex);
            return Result<object>.Failure("An error occurred while retrieving the role");
        }
    }

    public async Task<Result<List<object>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting all roles");

            // Global Query Filter s? t? ??ng lo?i b? các role có IsDeleted = true
            var roles = await _roleManager.Roles
                .Select(role => new
                {
                    role.Id,
                    role.Name,
                    role.Description,
                    role.CreatedBy,
                    role.CreatedDate,
                    role.UpdatedBy,
                    role.UpdatedDate
                })
                .ToListAsync(cancellationToken);

            var result = roles.Cast<object>().ToList();
            return Result<List<object>>.Success(result, "Roles retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting all roles", ex);
            return Result<List<object>>.Failure("An error occurred while retrieving roles");
        }
    }

    public async Task<PaginatedResult<GetRolesWithPaginationDto>> GetRolesWithPagination(GetRolesWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting roles with pagination - Page: {query.PageNumber}, Size: {query.PageSize}, KeyWord: {query.KeyWord}");

            var rolesQuery = _roleManager.Roles.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(query.KeyWord))
            {
                var searchTerm = query.KeyWord.Trim().ToLower();
                rolesQuery = rolesQuery.Where(r =>
                    r.Name.ToLower().Contains(searchTerm) ||
                    r.Description.ToLower().Contains(searchTerm));
            }

            var totalCount = await rolesQuery.CountAsync(cancellationToken);

            var roles = await rolesQuery
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            var rolesDto = _mapper.Map<List<GetRolesWithPaginationDto>>(roles);

            var result = PaginatedResult<GetRolesWithPaginationDto>.Create(rolesDto, totalCount, query.PageNumber, query.PageSize);

            LogInformation($"Retrieved {roles.Count} roles successfully for page {query.PageNumber}");
            return result;
        }
        catch (Exception ex)
        {
            LogError("Error getting roles with pagination", ex);
            throw new Exception("An error occurred while retrieving user with pagination");
        }
    }
}

