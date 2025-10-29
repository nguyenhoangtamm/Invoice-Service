# ASP.NET Core Identity Configuration for InvoiceApp

## Overview

This document outlines the ASP.NET Core Identity configuration implemented for the InvoiceApp project.

## Changes Made

### 1. Domain Layer Updates

-   **User Entity**: Updated to inherit from `IdentityUser<int>` and implement `IAuditableEntity`
-   **Role Entity**: Updated to inherit from `IdentityRole<int>` and implement `IAuditableEntity`
-   Both entities now include audit fields (CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsDeleted)

### 2. Infrastructure Layer Updates

-   **ApplicationDbContext**: Now inherits from `IdentityDbContext<User, Role, int>`
-   **Identity Services**: Configured with password requirements, lockout settings, and user validation
-   **UserRepository**: Custom repository for User operations using UserManager<User>
-   **Entity Configurations**: Updated for Identity compatibility
-   **Database Seeder**: Automatically creates default roles and admin user

### 3. Application Layer Updates

-   **AuthService**: New service for authentication operations (login, register, logout)
-   **IUserRepository**: Interface for user repository operations
-   **UserService**: Updated to work with Identity User entity
-   **DTOs**: New authentication DTOs (LoginRequest, RegisterRequest, AuthResponse, LogoutRequest)
-   **Validators**: FluentValidation validators for authentication requests

### 4. API Layer Updates

-   **JWT Authentication**: Configured JWT Bearer authentication
-   **AuthController**: New controller with login, register, logout, and user info endpoints
-   **Program.cs**: Updated to use JWT authentication and database seeding

## Configuration Details

### Identity Options

```csharp
services.AddIdentity<User, Role>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;

    // Sign-in settings
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
```

### JWT Configuration

JWT authentication is configured with the settings from appsettings.json:

```json
{
    "JwtSettings": {
        "Key": "01eff7aa-8bdd-47b0-b493-33ff277a4dac",
        "Issuer": "InvoiceApp",
        "Audience": "InvoiceAppClients",
        "ExpiresInMinutes": 60
    }
}
```

### Default Roles Created

-   **Admin**: System Administrator
-   **Teacher**: Course Instructor
-   **Student**: Course Learner
-   **Manager**: Course Manager

### Default Admin User

-   **Email**: admin@InvoiceApp.com
-   **Username**: admin
-   **Password**: Admin123!

## API Endpoints

### Authentication Endpoints

-   `POST /api/auth/login` - User login
-   `POST /api/auth/register` - User registration
-   `POST /api/auth/logout` - User logout
-   `GET /api/auth/me` - Get current user information

### Example Usage

#### Login Request

```json
{
    "email": "admin@InvoiceApp.com",
    "password": "Admin123!"
}
```

#### Register Request

```json
{
    "email": "user@example.com",
    "password": "Password123!",
    "userName": "testuser",
    "roleId": 3
}
```

#### Response

```json
{
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "userId": "1",
    "userName": "admin",
    "email": "admin@InvoiceApp.com",
    "expiresAt": "2024-01-01T12:00:00Z"
}
```

## Database Migration

Run the following command to apply the Identity migration:

```bash
dotnet ef database update --project InvoiceApp.Infrastructure --startup-project InvoiceApp.API
```

## Security Features

-   JWT token-based authentication
-   Password hashing using Identity's built-in password hasher
-   Account lockout after failed attempts
-   Role-based authorization support
-   Secure password requirements

## Next Steps

1. Apply the database migration
2. Test the authentication endpoints
3. Implement role-based authorization in controllers
4. Add email confirmation if required
5. Implement password reset functionality
6. Add refresh token support if needed

## Notes

-   The existing JWT middleware and custom authentication handler have been replaced with standard JWT Bearer authentication
-   User passwords are now managed by Identity's password hasher instead of BCrypt
-   The UserRepository provides a clean abstraction over UserManager<User> for easier testing and maintenance
