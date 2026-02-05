# Body4uHUB

A comprehensive fitness and wellness platform built with microservices architecture, enabling trainers to offer services, share expertise, and connect with clients seeking professional guidance.

## Overview

Body4uHUB is an enterprise-grade fitness marketplace that connects certified trainers with clients through three core services: secure identity management, rich content & community features, and a full-service marketplace for bookings and payments.

## Architecture

- **Microservices Architecture** - Three independent services with clear boundaries
- **Domain-Driven Design (DDD)** - Proper aggregates, value objects, and domain events
- **CQRS Pattern** - Separated read and write operations for optimal performance
- **Event-Driven Communication** - Asynchronous integration via RabbitMQ
- **Clean Architecture** - Maintainable, testable, and independent layers

## Tech Stack

- **.NET 8**
- **Entity Framework Core**
- **MediatR/CQRS implementation with pipeline behaviors**
- **RabbitMQ**
- **JWT Authentication**
- **SQL Server**
- **FluentValidation**
- **Docker**

## User Roles

- **Clients** - Browse trainers, book services, read articles, participate in forums
- **Trainers** - Create profiles, offer services, publish articles, manage bookings
- **Admins** - Platform management and moderation

## Microservices

### Identity Service
Authentication, authorization, user management, and JWT token handling

### Content & Community Service
Articles, forum discussions, comments, bookmarks, and community engagement

### Services Marketplace
Trainer profiles, service offerings, bookings, orders, payments, and reviews

## Design Principles

- **Separation of Concerns** - Each service owns its data and business logic
- **Eventual Consistency** - Services communicate asynchronously via events
- **Database Per Service** - No shared databases or foreign keys between services
- **Domain-Driven Design** - Business logic lives in the domain layer
