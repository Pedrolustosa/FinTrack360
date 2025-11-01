# FinTrack360 API

![Status](https://img.shields.io/badge/status-in%20development-yellow)
![.NET](https://img.shields.io/badge/.NET-9-blueviolet) 
![C#](https://img.shields.io/badge/C%23-12-blue) 
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8-purple) 
![EF Core](https://img.shields.io/badge/EF%20Core-8-orange) 
![JWT](https://img.shields.io/badge/Authentication-JWT-red)
![License](https://img.shields.io/badge/license-MIT-green)

FinTrack360 is a modern financial tracking application backend built with .NET, following Clean Architecture principles to ensure a scalable, maintainable, and testable codebase.

## 🏛️ Architecture

This project is built using **Clean Architecture**, which promotes a separation of concerns and makes the system easier to understand, maintain, and test. The solution is divided into the following layers:

-   **`FinTrack360.Domain`**: Contains the core business logic, entities, and interfaces. This layer is the heart of the application and has no dependencies on any other layer.
-   **`FinTrack360.Application`**: Implements the application's use cases by orchestrating the domain layer. It handles application-specific logic and is independent of the UI and infrastructure.
-   **`FinTrack360.Infrastructure`**: Provides implementations for the interfaces defined in the application layer, such as databases, email services, and other external systems.
-   **`FinTrack360.API`**: Exposes the application's functionality through a RESTful API. This is the entry point for clients.
-   **`FinTrack360.Infrastructure.IoC`**: Handles the dependency injection setup, wiring up the different layers of the application.
-   **`FinTrack360.Tests.Unit`**: Contains unit tests for the application, ensuring the quality and correctness of the code.

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

<details>
<summary>Click to view API endpoints</summary>

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
- `GET /activity-log` (**Auth Required**): Get the activity log for the logged-in user.

### Accounts (`/api/accounts`)

- `POST /`: Create a new account.
- `GET /{id}`: Get an account by its ID.
- `GET /`: Get all accounts for the logged-in user.
- `PUT /{id}`: Update an account.
- `DELETE /{id}`: Delete an account.
- `POST /{accountId}/import`: Import transactions from a file into an account.

### Assets (`/api/accounts/{accountId}/assets`)

- `POST /`: Add a new asset to an account.
- `GET /{id}`: Get an asset by its ID.
- `GET /`: Get all assets for an account.
- `PUT /{id}`: Update an asset.
- `DELETE /{id}`: Delete an asset.

### Budgets (`/api/budgets`)

- `POST /`: Create a new budget.
- `GET /{id}`: Get a budget by its ID.
- `GET /`: Get all budgets for the logged-in user, with optional filtering by month and year.
- `PUT /{id}`: Update a budget.
- `DELETE /{id}`: Delete a budget.

### Categories (`/api/categories`)

- `POST /`: Create a new category.
- `GET /{id}`: Get a category by its ID.
- `GET /`: Get all categories for the logged-in user.
- `PUT /{id}`: Update a category.
- `DELETE /{id}`: Delete a category.

### Dashboard (`/api/dashboard`)

- `GET /kpi/net-worth`: Get the net worth KPI.
- `GET /kpi/monthly-cash-flow`: Get the monthly cash flow KPI.
- `GET /budget-summary`: Get the budget summary.
- `GET /spending-by-category-chart`: Get the spending by category chart.
- `GET /upcoming-bills`: Get a list of upcoming bills.
- `GET /account-summary`: Get the account summary.
- `GET /recent-transactions`: Get a list of recent transactions.

### Debt Payoff Planner (`/api/debt-payoff-planner`)

- `GET /snowball`: Get a debt payoff plan using the snowball method.
- `GET /avalanche`: Get a debt payoff plan using the avalanche method.

### Financial Goals (`/api/financial-goals`)

- `POST /`: Create a new financial goal.
- `GET /{id}`: Get a financial goal by its ID.
- `GET /`: Get all financial goals for the logged-in user.
- `PUT /{id}`: Update a financial goal.
- `DELETE /{id}`: Delete a financial goal.
- `POST /{id}/contribute`: Contribute to a financial goal.

### Notifications (`/api/notifications`)

- `GET /`: Get all notifications for the logged-in user.
- `POST /{id}/mark-as-read`: Mark a notification as read.

### Recurring Transactions (`/api/recurring-transactions`)

- `POST /`: Create a new recurring transaction.
- `GET /{id}`: Get a recurring transaction by its ID.
- `GET /`: Get all recurring transactions for the logged-in user.
- `PUT /{id}`: Update a recurring transaction.
- `DELETE /{id}`: Delete a recurring transaction.

### Reports (`/api/reports`)

- `GET /spending-by-category`: Get a report of spending by category for a given date range.
- `GET /cash-flow`: Get a cash flow report for a given year.
- `GET /net-worth`: Get a net worth report.

</details>

## 🤝 Contributing

Contributions are welcome! Please follow these guidelines:

1.  **Fork the repository** and create a new branch for your feature or bug fix.
2.  **Follow the existing code style** and ensure all tests pass.
3.  **Submit a pull request** with a clear description of your changes.

## 📜 License

This project is licensed under the MIT License. See the [LICENSE.txt](LICENSE.txt) file for details.