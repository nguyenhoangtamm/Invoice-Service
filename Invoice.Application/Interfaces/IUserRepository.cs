using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.Entities;

namespace Invoice.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<List<User>> GetAllAsync();
    Task<List<User>> GetPagedAsync(int pageNumber, int pageSize);
    Task<int> GetCountAsync();
    Task<List<User>> GetPagedWithFilterAsync(int pageNumber, int pageSize, GetUsersWithPaginationQuery query);
    Task<int> GetCountWithFilterAsync(GetUsersWithPaginationQuery query);
    Task<User> CreateAsync(User user, string password);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(int id);
}

