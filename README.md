# 🎬 Movies REST API

A clean architecture-based Movies REST API built with .NET Core. This project includes:
- 🔐 JWT Authentication
- 🛠️ Dapper as the ORM
- 🧼 Clean Architecture
- 🔄 API Versioning
- 📄 Swagger for interactive documentation
- 📦 SDK generation for easy client integration
- ⭐ Ratings Support
- 🩺 Health Checks
- 🐳 Docker Compose for containerization

---

## 📌 Features

- **JWT Authentication**: Secure endpoints with token-based auth.
- **Dapper ORM**: Lightweight and high-performance data access.
- **Clean Architecture**: Decoupled, maintainable, and testable project structure.
- **API Versioning**: Seamlessly support multiple API versions.
- **Swagger**: Interactive and informative API documentation.
- **SDK**: Auto-generated SDK for consuming the API in other applications.
- **User Ratings**: Users can rate and remove ratings for movies.
- **Health Checks**: Built-in health endpoint for service monitoring.
- **Docker Compose**: Simplified setup using containerization.

---

## 🏗️ Project Structure

```
/MoviesApi
│
├── MoviesApi.Contracts      # Use cases and interfaces
├── MoviesApi.Domain           # Domain entities and value objects
├── MoviesApi.Application   # Dapper implementation and DB context
├── MoviesApi.API              # Controllers, versioning, middleware
├── MoviesApi.Sdk              # Generated SDK
├── docker-compose.yml         # Docker Compose for services
└── README.md
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/)
- SQL Server / PostgreSQL
- Docker & Docker Compose

### Setup

1. **Clone the repository**

```bash
git clone https://github.com/sina-ashtari/Movie_Api.git
cd movies-api
```

2. **Configure your database connection**

Update `appsettings.json` in the `MoviesApi.API` project:

```json
"ConnectionStrings": {
  "DefaultConnection": "Your_Connection_String_Here"
}
```

3. **Run with Docker Compose**

```bash
docker-compose up --build
```

4. **Run locally (without Docker)**

```bash
dotnet run --project MoviesApi.API
```

The API will start at `https://localhost:5001` or `http://localhost:5000`.

---

## 🔑 Authentication

Use [this](https://github.com/sina-ashtari/Identity) repository to get token.

Include the token in the `Authorization` header as:

```
Authorization: Bearer <your_token>
```

---

## 📘 Swagger Documentation

Once running, navigate to:

- [Swagger UI](https://localhost:5001/swagger): Interactive API testing and documentation.

Supports versioning like:
- `/swagger/v1/swagger.json`


---

## 🌟 Ratings API

| Method | Endpoint                               | Description                         |
|--------|----------------------------------------|-------------------------------------|
| PUT    | `/api/v1/movies/{id}/rate`             | Rate a movie (authorized)          |
| DELETE | `/api/v1/movies/{id}/rating`           | Remove a movie rating (authorized) |
| GET    | `/api/v1/ratings/user`                 | Get all user ratings                |

---

## 🎥 Movies API

| Method | Endpoint                        | Description           |
|--------|----------------------------------|-----------------------|
| POST   | `/api/v1/movies`                | Create a new movie   |
| GET    | `/api/v1/movies/{idOrSlug}`     | Get movie by ID/slug |
| GET    | `/api/v1/movies`                | Get all movies       |
| PUT    | `/api/v1/movies/{id}`           | Update movie         |
| DELETE | `/api/v1/movies/{id}`           | Delete movie         |

---

## 🩺 Health Check

Access the health check endpoint:

```
GET /_healthcheck
```

Returns application status, ideal for uptime monitoring.

---

## 🔧 Tools & Technologies

- ASP.NET Core 9
- JWT Authentication
- Dapper
- Swagger
- FluentValidation
- Clean Architecture
- Docker Compose
- Output Caching
- Serilog (optional)

---

## 👨‍💻 Contributing

1. Fork the project
2. Create a branch (`feature/your-feature`)
3. Commit your changes
4. Push and create a PR
