using Invoice.Application.Interfaces;
using Invoice.Domain.Entities;
using Invoice.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Invoice.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<User> _userManager;

    public UserRepository(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _userManager.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _userManager.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserName == username);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _userManager.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _userManager.Users
            .Include(u => u.Role)
            .Where(u => u.Status == UserStatus.Active)
            .ToListAsync();
    }

    public async Task<List<User>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return await _userManager.Users
            .Include(u => u.Role)
            .Where(u => u.Status == UserStatus.Active)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetCountAsync()
    {
        return await _userManager.Users
            .Where(u => u.Status == UserStatus.Active)
            .CountAsync();
    }

    public async Task<User> CreateAsync(User user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create user: {errors}");
        }
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to update user: {errors}");
        }
        return user;
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user != null)
        {
            user.Status = UserStatus.Inactive;
            user.UpdatedDate = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
        }
    }
}

