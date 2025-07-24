## Pokemon Web API
Pokemon Web API is a Pokemon database management system and review platform. The API provides a set of endpoints for managing Pokemon, categories, reviews, and owners. This API provides backend services for clients such as web and mobile applications.

#### Project Summary
This project is a RESTful Web API developed using ASP.NET Core. The API provides all the CRUD (Create, Read, Update, Delete) operations required to manage resources such as Pokemon, Category, Owner, and Review. The API allows users to review Pokemon, manage Pokemon and categories, and maintain owner information.

#### Technologies Used
ASP.NET Core: A modern framework used to create the Web API.

Entity Framework Core: An ORM (Object Relational Mapper) tool used for database operations.

SQL Server: Used as the database management system.

Swagger: A tool used for automated documentation and testing of the API.

AutoMapper: A tool used for data transfer between objects (model transformation using DTO - Data Transfer Object).

### Project Features
#### Pokemon Management:

Pokemons can be added, updated, deleted, and listed.

Relationships can be established with categories and reviews of Pokemon.

#### Category Management:

Categories can be added and updated for Pokemon.

CRUD operations can be performed for categories.

#### Review Management:

User-made comments and ratings can be added to Pokemon.

Review data such as title, comment, and score can be managed.

#### Owner Management:

Each Pokemon's owner can be linked to a specific country and user.

Owner information can be managed, and related Pokemon can be listed.

#### API Testing and Documentation:

Swagger allows for easy API testing. API endpoints can be tested through the Swagger UI.

#### User Instructions
Initializing the API:
The project can be run on ASP.NET Core. To get started, you can run the application from Visual Studio or the .NET CLI.

#### Database Configuration:
Database operations are performed using Entity Framework Core. You can apply migrations using the dotnet ef database update command to update the database.

API Testing:
You can use the Swagger UI to test the API. Swagger visually presents all endpoints, allowing you to test each one.

#### Technologies Used
ASP.NET Core: The main framework used for web API development.

Entity Framework Core: An ORM (Object Relational Mapping) tool for database operations.

Swagger: Used for API documentation and testing.
