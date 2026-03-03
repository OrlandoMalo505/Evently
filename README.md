# Evently
> Modular Monolith architecture built with .NET 8

## Overview
Evently is an event management application built as a **Modular Monolith** in .NET 8. The project demonstrates how to structure a scalable, maintainable system using module boundaries without the operational overhead of full microservices.

## Architecture
- **Modular Monolith** — each module is self-contained with its own domain, application, and infrastructure layers
- Clean separation of concerns between modules via well-defined contracts
- Designed to be easily decomposable into microservices if needed

## Tech Stack
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- MediatR / CQRS
- Docker

## Getting Started
```bash
git clone https://github.com/OrlandoMalo505/Evently
cd Evently
dotnet restore
dotnet run
```
