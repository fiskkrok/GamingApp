# GamingApp

## Overview

GamingApp is a web application that allows users to explore, play, and manage various games. The application provides features such as user authentication, game sessions tracking, achievements, and more.

## Purpose and Main Features

The purpose of GamingApp is to provide a platform for gamers to discover new games, track their progress, and interact with other players. The main features of the application include:

- User authentication and profile management
- Game catalog with various genres and categories
- Game sessions tracking and statistics
- Achievements and rewards system
- Rate limiting and caching for improved performance
- Structured logging and error handling

## Setup Instructions

### Prerequisites

- .NET 6.0 SDK or later
- PostgreSQL database
- Redis server
- Docker (optional, for running the application in a container)

### Running the Project Locally

1. Clone the repository:

   ```bash
   git clone https://github.com/yourusername/GamingApp.git
   cd GamingApp
   ```

2. Set up the PostgreSQL database:

   - Create a new PostgreSQL database.
   - Update the connection string in `GamingApp.ApiService/appsettings.Development.json` to point to your PostgreSQL database.

3. Set up the Redis server:

   - Ensure that a Redis server is running and accessible.
   - Update the Redis connection string in `GamingApp.ApiService/appsettings.Development.json`.

4. Set up environment variables:

   - Create a `.env` file in the root of the project and add the following environment variables:

     ```env
     ASPNETCORE_ENVIRONMENT=Development
     ConnectionStrings__DefaultConnection=YourPostgreSQLConnectionString
     ConnectionStrings__Redis=YourRedisConnectionString
     ```

5. Run the application:

   ```bash
   dotnet run --project GamingApp.ApiService
   ```

6. Open your browser and navigate to `https://localhost:5001` to access the application.

## Usage Guidelines

- Register a new account or log in with an existing account.
- Browse the game catalog and explore different genres and categories.
- Start a game session and track your progress.
- Unlock achievements and view your statistics.
- Interact with other players and join the community.

For more detailed information, please refer to the documentation in the `docs` folder.

