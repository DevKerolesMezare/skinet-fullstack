# E-Commerce Store

A full-stack e-commerce application built with **ASP.NET Core Web API** and **Angular**.

The application implements the core e-commerce workflow, including product browsing, authentication and authorization, shopping basket management, checkout, order processing, and online payment integration.

## Tech Stack

- **Backend:** ASP.NET Core Web API, C#, Entity Framework Core, ASP.NET Core Identity
- **Frontend:** Angular, TypeScript, RxJS, Signals
- **Database:** SQL Server
- **Caching:** Redis
- **UI:** Angular Material, Tailwind CSS
- **Payments:** Stripe
- **Real-time Communication:** SignalR
- **Deployment:** Microsoft Azure

## Architecture & Design Patterns

The backend applies common software architecture and design practices, including:

- Repository Pattern
- Unit of Work Pattern
- Specification Pattern
- Dependency Injection
- Multiple `DbContext` boundaries
- Role-Based Authentication & Authorization
- Separation of application concerns

The **Specification Pattern** is used to encapsulate reusable query logic for searching, filtering, sorting, and pagination.

## Key Features

- User registration, authentication, and authorization
- Role-based access control
- Product searching, filtering, sorting, and pagination
- Redis-based shopping basket
- Multi-step checkout
- Order creation and management
- Stripe payment integration with 3D Secure
- Real-time communication using SignalR
- Reusable Angular form components
- Reactive Forms with synchronous and asynchronous validation
- Lazy-loaded Angular routes
- API error handling
- Caching

## Project Structure

The solution separates backend responsibilities into dedicated areas for the API, core/application logic, and infrastructure, alongside a separate Angular client.

## Project Context

This project was developed as part of a practical **ASP.NET Core and Angular** course, implementing a complete full-stack e-commerce application from initial setup through deployment.

The implementation provided hands-on experience with **RESTful APIs, database access, authentication and authorization, frontend development, caching, payment integration, real-time communication, and cloud deployment**.

## Possible Extensions

- Inventory management
- Product reviews and ratings
- Wishlist functionality
- Email notifications
- Automated unit and integration testing
