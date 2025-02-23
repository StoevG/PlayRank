# PlayRank API

**PlayRank** is a robust football ranking API built on Clean Architecture principles. It provides a well-structured, maintainable, and extensible solution for managing football matches, teams, and rankings while integrating external football data and offering CSV export functionality.

---

## Table of Contents

- [Technical Overview](#technical-overview)
- [Product Structure](#product-structure)
- [Endpoints & Functionality](#endpoints--functionality)
  - [Match Management](#match-management)
  - [Ranking Management](#ranking-management)
  - [Team Management](#team-management)
  - [External Football Teams](#external-football-teams)
  - [CSV Export Endpoints](#csv-export-endpoints)
- [External API Integration](#external-api-integration)
- [Future Development](#future-development)

---

## Technical Overview

PlayRank is built using **Clean Architecture** with clear separation of concerns:

- **API Layer:**  
  Contains controllers that handle HTTP requests and responses, with custom Swagger setup for interactive HTML documentation.

- **Application Layer:**  
  Contains services, view models, and mappers that orchestrate business logic and integrate with external APIs.

- **Domain Layer:**  
  Defines core entities, repository interfaces, and shared constants (such as error messages and export constants).

- **Infrastructure Layer:**  
  Implements data access using the Repository Pattern with EF Core DbContext.

The project employs several design patterns including the **Repository Pattern**, **Chain of Responsibility**, and **Strategy Design Pattern** across the overall architecture. This design ensures that the core business logic is decoupled from external dependencies and is very well designed for maintainability and extensibility.

---

## Product Structure

- **PlayRank.Api**  
  Contains all API controllers (e.g., `MatchController`, `RankingController`, `TeamController`, `ExportController`) and global error handling middleware.

- **PlayRank.Application.Core**  
  Implements core services (e.g., `MatchService`, `TeamService`, `RankingService`, `FootballTeamService`, `ExportService`), view models, mappers, and external API integrations.

- **PlayRank.Domain**  
  Defines core domain entities (`Match`, `Team`, `Ranking`), repository interfaces, and shared constants.

- **PlayRank.Data**  
  Implements data access using the Repository Pattern and provides the EF Core DbContext (`PlayRankContext`).

- **PlayRank.Tests**  
  Contains unit and integration tests using **xUnit**, **Moq**, and **MockQueryable.Moq**. A `TestDbContextFactory` is provided for in-memory testing.

---

## Endpoints & Functionality

### Match Management

- **GET** `/api/match/all`  
  *Retrieves all matches.*

- **GET** `/api/match/{id}`  
  *Retrieves a match by its unique identifier.*

- **POST** `/api/match/create`  
  *Creates a new match.*  
  **Body:** JSON object with match details (team IDs, scores, and match status).

- **PUT** `/api/match/update/{id}`  
  *Updates an existing match and recalculates rankings.*  
  **Body:** JSON object with updated match details.

- **GET** `/api/match/predict/{matchId}`  
  *Predicts the match score based on team rankings if the match is not over.*

- **DELETE** `/api/match/{id}`  
  *Deletes a match and updates rankings accordingly.*

### Ranking Management

- **GET** `/api/ranking/all`  
  *Retrieves all team rankings.*

- **GET** `/api/ranking/top3`  
  *Retrieves the top 3 ranked teams.*

- **GET** `/api/ranking/leaderboard/paged?page={page}&pageSize={pageSize}`  
  *Retrieves a paginated leaderboard.*

- **GET** `/api/ranking/statistics/{teamId}`  
  *Retrieves detailed statistics for a specific team.*

- **GET** `/api/ranking/position/{teamId}`  
  *Retrieves the rank position of a specific team.*

- **GET** `/api/ranking/topwins/{count}`  
  *Retrieves the top teams with the most wins.*

- **GET** `/api/ranking/topdraws/{count}`  
  *Retrieves the top teams with the most draws.*

- **GET** `/api/ranking/toplosses/{count}`  
  *Retrieves the top teams with the least losses.*

- **POST** `/api/ranking/update-match/{matchId}`  
  *Updates the ranking for a specific match.*

- **POST** `/api/ranking/recalculate`  
  *Recalculates rankings for all teams.*

### Team Management

- **GET** `/api/team/all`  
  *Retrieves all teams.*

- **GET** `/api/team/{id}`  
  *Retrieves a team by its unique identifier.*

- **POST** `/api/team/create`  
  *Creates a new team.*  
  **Body:** JSON object with team details.

- **PUT** `/api/team/edit/{id}`  
  *Updates an existing team.*  
  **Body:** JSON object with updated team details.

- **DELETE** `/api/team/{id}`  
  *Deletes a team and updates rankings accordingly.*

### External Football Teams

- **GET** `/api/externalteams/all`  
  *Retrieves all external football teams from the external API.*

- **GET** `/api/externalteams/search?query={query}`  
  *Searches for external teams by name or country.*

### CSV Export Endpoints

- **GET** `/api/export/teams`  
  *Exports all teams to a CSV file.*

- **GET** `/api/export/matches`  
  *Exports all matches to a CSV file.*

- **GET** `/api/export/rankings`  
  *Exports all rankings to a CSV file.*

- **GET** `/api/export/externalteams`  
  *Exports external football teams data to a CSV file.*

---

## External API Integration

The project integrates with an external football API to fetch up-to-date team data.

### **Setup Instructions:**

1. **Obtain an API Key:**  
   Register with [API-SPORTS](https://www.api-football.com/news/post/how-to-get-all-teams-and-their-ids) (or via RapidAPI) to obtain your API key.

2. **Configure Your Application:**  
   In your `appsettings.json`, add:
   ```json
   {
     "FootballApi": {
       "ApiKey": "YOUR_API_KEY"
     }
   }
   
3. **Replace `YOUR_API_KEY` with your actual API key.**

4. **Access External Endpoints:**
- **GET** `/api/externalteams/all` — Retrieve all external teams.
- **GET** `/api/externalteams/search?q=yourQuery` — Search external teams by name or country.
- **GET** `/api/export/externalteams` — Export external teams data to CSV.

## **Future Development:**
- **Enhanced Score Prediction:**  
  Future enhancements may include more sophisticated prediction algorithms utilizing historical data, advanced statistics, or machine learning techniques.
- **Authorization and Security:**  
  Future plans include implementing robust authentication and authorization (e.g., JWT-based security) to safeguard endpoints and data.
- **Additional Integrations:**  
  The system may be extended 
