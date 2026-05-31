# SaaSify Multi-Tenant API

A production-ready Multi-Tenant Web API built with ASP.NET Core 8, PostgreSQL, Entity Framework Core, Microsoft Identity, JWT Authentication, CQRS (MediatR), FluentValidation, API Versioning, and Clean Architecture.

---

# Architecture Overview

The solution follows Clean Architecture principles to ensure maintainability, scalability, and separation of concerns.

## Project Structure

```text
src
│
├── SaaSify.MultiTenant.Api
│   ├── Controllers
│   ├── Middleware
│   ├── Extensions
│   ├── Responses
│   └── Program.cs
│
├── SaaSify.MultiTenant.Application
│   ├── Abstractions
│   ├── Behaviors
│   ├── Common
│   ├── Exceptions
│   ├── Features
│   │   ├── Auth
│   │   ├── Employees
│   │   └── Tenants
│   └── Validators
│
├── SaaSify.MultiTenant.Core
│   ├── Entities
│   ├── Constants
│   └── Common
│
└── SaaSify.MultiTenant.Infrastructure
    ├── Authentication
    ├── Configurations
    ├── Database
    ├── Identity
    ├── MultiTenancy
    ├── Persistence
    └── Repositories
```

---

# Key Features

## Authentication & Authorization

* ASP.NET Core Identity
* JWT Authentication
* Policy-Based Authorization
* Role-Based Authorization

### Roles

| Role       | Permissions                                    |
| ---------- | ---------------------------------------------- |
| SuperAdmin | Manage Tenants                                 |
| Admin      | Manage Users and Employees within their Tenant |
| Employee   | View Own Profile Only                          |

---

## Multi-Tenant Architecture

### Master Database

Contains:

* Tenants
* AspNetUsers
* AspNetRoles
* Identity-related tables

```text
MasterDb
├── Tenants
├── AspNetUsers
├── AspNetRoles
└── Identity Tables
```

### Tenant Databases

Each tenant has a dedicated PostgreSQL database.

```text
tenant_1234
└── Employees

tenant_5678
└── Employees
```

This provides complete tenant-level data isolation.

---

# Technologies Used

* ASP.NET Core 8
* PostgreSQL
* Entity Framework Core
* Microsoft Identity
* JWT Authentication
* MediatR
* FluentValidation
* Swagger / OpenAPI
* API Versioning
* Clean Architecture

---

# Setup Instructions

## Prerequisites

Install:

* .NET 8 SDK
* PostgreSQL 15+
* Visual Studio 2022 / Rider / VS Code

---

## Clone Repository

```bash
git clone <repository-url>

cd SaaSify.MultiTenant
```

---

## Configure Database Connection

Update the PostgreSQL connection string in:

```json
appsettings.json
```

Example:

```json
{
  "DatabaseSettings": {
    "MasterConnection": "Host=localhost;Port=5432;Database=SaaSifyMasterDb;Username=postgres;Password=postgres"
  },

  "Jwt": {
    "Key": "your-super-secret-key-at-least-32-characters",
    "Issuer": "SaaSify",
    "Audience": "SaaSifyUsers",
    "ExpiryMinutes": 60
  }
}
```

Use your own PostgreSQL credentials.

---

## Run the Application

```bash
dotnet run --project src/SaaSify.MultiTenant.Api
```

The application automatically:

* Applies Master Database migrations
* Creates required Identity tables
* Seeds application roles
* Seeds the SuperAdmin account

No manual migration commands are required.

---

## Swagger

After running the application:

```text
https://localhost:{port}/swagger
```

---

# Initial SuperAdmin Credentials

The application seeds a default SuperAdmin user.

| Field    | Value                                                   |
| -------- | ------------------------------------------------------- |
| Email    | [assessment@yopmail.com](mailto:assessment@yopmail.com) |
| Password | Tester@123                                              |
| Role     | SuperAdmin                                              |
| TenantId | null                                                    |

---

# Tenant Database Creation Flow

When a SuperAdmin creates a new tenant:

### Step 1

A tenant record is created in the Master Database.

### Step 2

A unique Tenant Identifier is generated.

Example:

```text
4832
```

### Step 3

A dedicated PostgreSQL database is created.

Example:

```text
tenant_4832
```

### Step 4

TenantDbContext migrations are executed automatically.

This creates:

```text
Employees
```

table inside the tenant database.

### Step 5

A default Admin user is created for the tenant.

The password is supplied during tenant creation.

### Step 6

The tenant database connection string is stored in:

```text
Tenants.DbConnStr
```

### Step 7

Future requests resolve the tenant database dynamically using the authenticated user's TenantId claim.

---

# API Examples

Base Route

```text
/api/v1
```

---

# Authentication

## Login

### Request

```http
POST /api/v1/auth/login
```

Request Body:

```json
{
  "email": "assessment@yopmail.com",
  "password": "Tester@123"
}
```

### Response

```json
{
  "success": true,
  "message": "Login successful.",
  "data": {
    "token": "jwt-token"
  }
}
```

---

# Tenant Management

Authorization:

```text
SuperAdmin Only
```

---

## Create Tenant

```http
POST /api/v1/tenants
```

Request Body:

```json
{
  "name": "ABC Company",
  "emailAddress": "admin@abc.com",
  "adminPassword": "Admin@123"
}
```

---

## Get All Tenants

```http
GET /api/v1/tenants
```

---

## Get Tenant By Id

```http
GET /api/v1/tenants/{id}
```

---

## Update Tenant

```http
PUT /api/v1/tenants/{id}
```

Examples:

```json
{
  "name": "Updated Company Name"
}
```

or

```json
{
  "emailAddress": "newadmin@abc.com"
}
```

---

## Delete Tenant

```http
DELETE /api/v1/tenants/{id}
```

---

# Employee Management

Authorization:

```text
Admin Only
```

---

## Create Employee

```http
POST /api/v1/employees
```

Request Body:

```json
{
  "fullName": "John Doe",
  "emailAddress": "john@company.com",
  "password": "Password@123"
}
```

This operation:

* Creates an Employee record
* Creates an Identity user
* Assigns Employee role
* Associates the user with the current tenant

---

## Get Employees

```http
GET /api/v1/employees
```

---

## Get Employee By Id

```http
GET /api/v1/employees/{id}
```

---

## Update Employee

```http
PUT /api/v1/employees/{id}
```

Examples:

```json
{
  "fullName": "John Updated"
}
```

or

```json
{
  "emailAddress": "john.updated@company.com"
}
```

---

## Delete Employee

```http
DELETE /api/v1/employees/{id}
```

---

# Employee Self-Service

Authorization:

```text
Employee Only
```

---

## Get My Profile

```http
GET /api/v1/employees/me
```

Returns the profile of the currently authenticated employee.

---

# Error Handling

Global exception handling middleware provides consistent API responses.

Custom exceptions:

* NotFoundException → HTTP 404
* ConflictException → HTTP 409
* UnauthorizedException → HTTP 401
* ValidationException → HTTP 400

JWT authentication also returns standardized responses for:

* Missing token
* Invalid token
* Forbidden access

---

# Security Features

* JWT Authentication
* ASP.NET Core Identity
* Tenant Isolation
* Policy-Based Authorization
* Role-Based Authorization
* FluentValidation Pipeline
* Global Exception Handling
* Dynamic Tenant Resolution
* Secure Password Policies

---

# Author

Bibek Chaudhary

ASP.NET Core Developer
