# FinTrack360 API

![.NET](https://img.shields.io/badge/.NET-9-blueviolet) ![C#](https://img.shields.io/badge/C%23-12-blue) ![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8-purple) ![EF Core](https://img.shields.io/badge/EF%20Core-8-orange) ![JWT](https://img.shields.io/badge/Authentication-JWT-red)

FinTrack360 is a modern financial tracking application backend built with .NET, following Clean Architecture principles to ensure a scalable, maintainable, and testable codebase.

## ✨ Features

- **Authentication**: Secure user registration, login, email confirmation, and password management.
- **Token-Based Security**: JWT for secure API access, with token revocation (logout) functionality.
- **User Profile Management**: Users can update their personal information and manage their account.
- **Soft Delete**: Account deletion is handled via soft delete, preserving data for auditing purposes while preventing access.

## 🛠️ Tech Stack

- **Framework**: .NET 9 / ASP.NET Core 8
- **Architecture**: Clean Architecture
- **Database**: Entity Framework Core 8 with SQLite (for development)
- **Authentication**: ASP.NET Core Identity with JWT Bearer Tokens
- **Mediation**: MediatR for implementing the CQRS pattern
- **Validation**: FluentValidation for robust request validation

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- A code editor like VS Code or Visual Studio

### Configuration

1.  **Clone the repository**:
    ```bash
    git clone <your-repository-url>
    cd FinTrack360
    ```

2.  **Configure User Secrets**:
    This project uses user secrets to store sensitive information like database connection strings, JWT keys, and email service credentials. 

    Initialize user secrets for the API project:
    ```bash
    cd FinTrack360.API
    dotnet user-secrets init
    ```

    Set the required secrets:
    ```bash
    dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=../fintrack360.db"
    dotnet user-secrets set "Jwt:Key" "[YOUR_SUPER_SECRET_JWT_KEY_THAT_IS_LONG_ENOUGH]"
    dotnet user-secrets set "SmtpSettings:FromEmail" "your-email@example.com"
    dotnet user-secrets set "SmtpSettings:Username" "your-smtp-username"
    dotnet user-secrets set "SmtpSettings:Password" "your-smtp-password"
    ```

3.  **Apply Database Migrations**:
    The application will automatically apply pending migrations on startup.

### Running the Application

Navigate to the API project directory and run the application:

```bash
cd FinTrack360.API
dotnet run
```

The API will be available at `https://localhost:7241`.

## 📖 API Endpoints

### Authentication (`/api/auth`)

- `POST /register`: Register a new user.
- `POST /login`: Log in and receive a JWT.
- `POST /confirm-email`: Confirm a user's email address.
- `POST /forgot-password`: Request a password reset link.
- `POST /reset-password`: Reset the user's password.
- `POST /resend-confirmation`: Resend the email confirmation link.
- `POST /change-password` (**Auth Required**): Change the logged-in user's password.
- `POST /logout` (**Auth Required**): Log out by revoking the current JWT.

### Profile (`/api/profile`)

- `PUT /me` (**Auth Required**): Update the logged-in user's profile (first name, last name, phone number).
- `DELETE /me` (**Auth Required**): Delete the logged-in user's account (requires password confirmation).