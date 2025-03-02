# Backend Challenge

## Overview

This project is a backend service built with **ASP.NET Core**. It provides API functionality and runs in a containerized environment using **Docker**.

## Prerequisites

Before running the project, ensure you have the following installed:

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (if running locally)
- [Docker](https://www.docker.com/get-started) and [Docker Compose](https://docs.docker.com/compose/)
- [Postman](https://www.postman.com/) (for testing the API)
- Visual Studio Code (optional, for local development)

## Running the Project

### Using Docker

To run the project using Docker, follow these steps:

1. **Build and Start Containers**

   ```sh
   docker-compose up --build
   ```

   This command builds and starts the container, mapping port **8080** to the host.

2. **Running in Debug Mode**\
   If you need to debug, use:

   ```sh
   docker-compose -f docker-compose.debug.yml up --build
   ```

   This enables debugging configurations.

3. **Stopping the Containers**

   ```sh
   docker-compose down
   ```

## Project Structure

- **backend\_challenge.sln**: Visual Studio solution file
- **Dockerfile**: Defines how to build the Docker image
- **docker-compose.yml**: Configuration for running the service in Docker

## Environment Variables

The application uses the following environment variables:

- `ASPNETCORE_ENVIRONMENT=Development` (set in `docker-compose.debug.yml`)

## Logs

Application logs are stored in the `./Logs` directory (mounted in `docker-compose.yml`).

## Notes

- The API runs on port **8080** inside the container.
- Make sure Docker is running before executing commands.

