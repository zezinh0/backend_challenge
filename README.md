# Backend Challenge

## Overview

This mini-project is a backend service built with **ASP.NET Core**. It provides API functionality and runs in a containerized environment using **Docker**. For more detailed information about the mini-project, refer to the `Backend_Challenge_Devlop.pdf`.

The API handles various operations related to managing books, including:

- Retrieving all books
- Adding a new book
- Searching books by various parameters such as author, title, etc.
- Retrieving detailed information about a specific book by ID

## Prerequisites

Before running the project, ensure you have the following installed:

- [Docker](https://www.docker.com/get-started) and [Docker Compose](https://docs.docker.com/compose/) (for running the application in containers)
- [Postman](https://www.postman.com/) (for testing the API)

### .NET SDK Installation (Optional)

- **Installing .NET 9.0 SDK** is **not necessary** if you're running the project using **Docker**.
  - Docker will handle the required environment setup, including the .NET runtime and SDK.
  - You only need to install the **.NET SDK** locally if you plan to **develop** or **build the project** without using Docker.
## Running the Project

### Using Docker

To run the project using Docker, follow these steps:

1. **Build the Docker Containers**

   First, build the Docker containers using the following command:

   ```sh
   docker-compose build
   ```

   This command builds the container.

2. **Start Containers**

   ```sh
   docker-compose up
   ```

   This command starts the container.

3. **Running in Debug Mode**\
   If you need to debug, use:

   ```sh
   docker-compose -f docker-compose.debug.yml up --build
   ```

   This enables debugging configurations.

4. **Stopping the Containers**

   ```sh
   docker-compose down
   ```

## Postman Collection

The project includes a **Postman collection** to **test the API**. You can import the `backend_challenge.postman_collection.json` file into Postman to test the following API requests:

- **GET** `/books` - Retrieve all books.
- **POST** `/books` - Add a new book.
- **GET** `/books/search` - Search books by parameters (author, title, etc.).
- **GET** `/books/{id}` - Get details of a specific book by ID.

### Importing into Postman

1. **Open Postman** and click on **Import** in the top-left corner.
2. Choose the **File** tab.
3. Click **Choose Files** and select the `backend_challenge.postman_collection.json` file.
4. After the file is selected, click **Import**.

### Setting the URL in Postman

Once you have imported the collection, ensure that the **URL** variable in Postman is set to `http://localhost:8080` (or wherever your containerized API is running).


## Environment Variables

The application uses the following environment variables:

- `ASPNETCORE_ENVIRONMENT=Development` (set in `docker-compose.debug.yml`)

## Logs

Application logs are stored in the `./Logs` directory (mounted in `docker-compose.yml`).

## Notes

- The API runs on port **8080** inside the container.
- Make sure Docker is running before executing commands.

