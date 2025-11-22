# ElasticSearchDotNet.Api

## Overview

ElasticSearchDotNet.Api is a .NET-based API that integrates with Elasticsearch to manage and query location-based data such as cities, districts, and neighbors. The project is built using C# and includes middleware for exception handling.

## Features

- **Elasticsearch Integration**: Perform CRUD operations on Elasticsearch indices for cities, districts, and neighbors.
- **Search Functionality**:
  - Search districts by city code.
  - Search neighbors by district code or description.
- **Error Handling**: Middleware for centralized exception handling with proper HTTP status codes.
- **Scalable Design**: Supports large datasets with configurable query sizes.

## Technologies Used

- **Languages**: C#, JavaScript
- **Frameworks**: ASP.NET Core
- **Database**: Elasticsearch
- **Logging**: Microsoft.Extensions.Logging
- **Serialization**: System.Text.Json

## Project Structure

- `ElasticSearchDotNet.Api/Services/ElasticsearchLocationDataService.cs`: Contains services for querying Elasticsearch indices.
- `ElasticSearchDotNet.Api/Middleware/ExceptionHandlingMiddleware.cs`: Middleware for handling exceptions and returning structured error responses.

## Installation

1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd ElasticSearchDotNet.Api
   
2. Restore dependencies:
   ```bash
   dotnet restore
   
3. Update the `appsettings.json` file with your Elasticsearch configuration.

4. Run the application:
   ```bash
   dotnet run
   
## Usage

### Endpoints
  Search Districts: Search districts by city code or description.
  Search Neighbors: Search neighbors by district code or description.
  Get City by Code: Retrieve a city by its unique code.
  Get District by Code: Retrieve a district by its unique code.
  
  #### Example Requests
GET /api/districts?cityCode=12345

#### Example Response
[
  {
    "code": "123",
    "name": "District A",
    "cityCode": "12345"
  }
]