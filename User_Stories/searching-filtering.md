# 1. User Stories

## User (Customer)
- As a user, I want to search properties by a free-text query so that I can find what I'm looking for by keywords.
- As a user, I want to filter by (price, location, area, rooms, estate type, contract type) so that I can narrow down my interests.
- As a user, I want to sort results by price, area, or date added so that I can see the latest deals first.

# 2. Acceptance Criteria & Edge Cases

## Feature: Property Searching, Filtering and Sorting

* **Acceptance Criteria:**
    * The system must record each advertisement's publish date (`publish_date`) to enable sorting by "Latest Added".
    * The system must allow users to apply multiple filters simultaneously.
    * Users should be able to set a minimum and maximum price range.
    * The system must filter properties based on the selected city or neighborhood.
    * Users should have an option to sort results by "Latest Added" (`sortBy=date`, `sortOrder=desc`).
    * The system must filter results by "Estate Type" (`APARTMENT`, `LAND`, `SHOP`, `VILLA`, `OFFICE`) and "Room Count".
    * Users must be able to switch between `RENT` and `SALE` (`contractType`).
    * There should be an option to clear all filters.
    * The free-text search (`q`) matches against `address`, `city`, `country`, and the advertisement `title`
      (no description/amenity search).
    * Results include only advertisements with `status` = `ACTIVE`. Advertisements of deactivated users
      or companies are excluded from results.
    * When an authenticated user performs a search, the system records the query to their search history as a side effect (see `history/RecentSearches.md` and `history/RecentFilters.md`).

* **Edge Cases:**
    * The search returns no matching results => returns `200 OK` with an empty array: `{ "data": [] }`.
    * `minPrice` > `maxPrice` or `minArea` > `maxArea` => `400 Bad Request`. Example: `{ "message": "Minimum price cannot be greater than maximum." }`.
    * Negative values (`price`, `area`, `rooms`, `page`, `pageSize`) => `400 Bad Request`.
    * A `String` provided for an `int`/`decimal` field => `400 Bad Request`.
    * Invalid enum value for `estateType`, `contractType`, `sortBy`, or `sortOrder` => `400 Bad Request`.
    * `page` < 1, or `pageSize` < 1 or > 50 => `400 Bad Request`.
    * Unauthenticated search still returns results, but the query is not recorded to history.

# 3. API Endpoints (Login & Roles)

## 1. Search and Filter Properties

* **Endpoint:** GET /api/v1/properties
* **Description:** Searches, filters, sorts, and paginates **active** advertisements (`status` = `ACTIVE`;
  advertisements of deactivated users/companies are excluded). When the authenticated user searches, the query
  is automatically recorded to their search history (refer to `history/RecentSearches.md` and `history/RecentFilters.md`).
* **Headers:** `Authorization: Bearer <User_Token>` (optional; required only to record search history)
* **Query Parameters:**
    * `q` (string, optional): Free-text keyword search (e.g., "house in newyork"). Matches against
      `address`, `city`, `country`, and the advertisement `title`. Recorded in the user's recent text searches
      when authenticated.
    * `minPrice` (decimal, optional): Minimum price.
    * `maxPrice` (decimal, optional): Maximum price.
    * `city` (string, optional): Filter by city or neighborhood.
    * `minRooms` (int, optional): Minimum number of rooms.
    * `minArea` (decimal, optional): Minimum area.
    * `maxArea` (decimal, optional): Maximum area.
    * `estateType` (string, optional): `APARTMENT`, `LAND`, `SHOP`, `VILLA`, `OFFICE`.
    * `contractType` (string, optional): `RENT`, `SALE`.
    * `page` (int, optional): Page number. Defaults to `1`.
    * `pageSize` (int, optional): Results per page. Defaults to `10`, maximum `50`.
    * `sortBy` (string, optional): `price`, `area`, `date`.
    * `sortOrder` (string, optional): `asc`, `desc`.
*   **Response (200 OK Status):**
```json
{
  "totalItems": 45,
  "totalPages": 5,
  "currentPage": 1,
  "pageSize": 10,
   "data": [
    {
      "property_id": 20,
      "price": 100000,
      "area": { "value": 120, "unit": "SqM" },
      "location": { "latitude": 36.2021, "longitude": 37.1343, "address": "Aleppo, Syria", "city": "Aleppo" },
      "rooms": 3,
      "estate_type": "APARTMENT",
      "contract_type": "SALE",
      "created_at": "2026-07-28T09:00:00Z"
    }
  ]
}
```
*   **Error Responses:**
    *   `400 Bad Request`: Missing required fields or invalid input. Example: `{ "message": "Minimum price cannot be greater than maximum." }`.

# 4. Database Schema (Entities & Attributes)

Refer to `PropertyDetails.md` for the full `Property` table definition (the `User` table is defined in `users_doc/all-users.md`), and `PropertyListing.md` for the `Advertisement` table. Filters and sorting operate on `Property`, joined with `Advertisement`:

> **ID strategy:** `property_id` in results is the internal `Property.Id` (**INT**) — `Property` has no public id (see `PropertyDetails.md`). The public-facing identifiers (`Advertisement.PublicId`, `User.PublicId`) are **UUID v7**.

| Query Parameter | Column |
|---|---|
| `minPrice` / `maxPrice` | `Property.Price` |
| `minArea` / `maxArea` | `Property.AreaValue` |
| `minRooms` | `Property.NumberOfRooms` |
| `city` | `Property.City` |
| `estateType` | `Property.PropertyType` |
| `q` (free-text) | `Property.Address`, `Property.City`, `Property.Country`, `Advertisement.Title` |
| `contractType` | `Advertisement.ContractType` (`RENT`, `SALE`) |
| `sortBy=date` | `Advertisement.PublishDate` |

Additionally, this feature relies on the `SearchQuery` and `RecentFilter` tables defined in
`history/RecentSearches.md` and `history/RecentFilters.md`:
the search endpoint records the `q` text to `SearchQuery` and the filter parameters to `RecentFilter`
(both automatically, independently of each other). Filters re-run against this endpoint
via "View Results".