# Message Processor

A .NET 7 solution for processing incoming messages with optimized batch processing and concurrency handling.

## Projects

- **MessageProcessor.Api**: Web API project that receives messages and handles HTTP endpoints
- **MessageProcessor.Core**: Core library containing business logic, message processing, and data access

## Features

- Message receiving and queueing
- Batch processing with optimized DbContext usage
- Concurrency handling with optimistic locking
- Message type-based processing
- Error handling and logging

## Getting Started

1. Clone the repository
2. Open the solution in Visual Studio 2022
3. Update the connection string in `appsettings.json`
4. Run the database migrations
5. Start the application

## Architecture

The solution uses a clean architecture approach with:

- Separation of concerns between API and core business logic
- Channel-based message queuing for async processing
- Entity Framework Core for data access
- Optimized batch processing for better performance