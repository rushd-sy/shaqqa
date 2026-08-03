# 1. User Stories

## User
- As a user, I want my searches to be recorded automatically so that I can quickly check for updates later without re-typing them.
- As a user, I want to save my recent visits so that I can find anything I visited and then wanted to return to, or that I lost.

# 2. Acceptance Criteria & Edge Cases

## Feature: Recent Visits and Search History
* **Acceptance Criteria:**
  * Search queries are recorded automatically as a side effect of the search endpoint — no separate
    client request is required.
  * The system should save recent visits for each user.
* **Business Rules:**
  * Each user is limited to the last 10 visits and 10 searches (`FIFO` - First In, First Out).
  * Adding an 11th record prunes the oldest one automatically.
  * A repeat visit to a property, or a repeated identical search query, updates the existing
    record's `viewed_at` / `searched_at` timestamp instead of inserting a duplicate entry.
* **Edge Cases:**
  * User has never visited/searched anything → the endpoint returns an empty list (200), not an error.
  * Two rapid/concurrent visits for the same property → must still result in a single record, no duplicates.
  * History is exactly at the limit (10) → nothing is pruned (boundary of the FIFO rule).
  * A property in the user's "Recent Visits" list was deleted from the platform → filter it out of results
    to avoid broken links.
  * Empty or blank search query → no entry is recorded; blank searches are never added to history.
  * Property does not exist when saving a visit → `404 Not Found`, no record is saved.
  * Unauthenticated request on the visits / history endpoints → `401 Unauthorized`.
  * Unauthenticated search still returns results, but the query is not recorded to history.
  * The `userId` is always derived from the token; any `userId` sent in the body is ignored → a user can never
    view or modify another user's history.

# 3. API Endpoints (Login & Roles)

## 1. Save Recent Visit
* **Endpoint:** POST /api/user/recent-visits
* **Description:** save recent visits
* **Headers:** `Authorization: Bearer <User_Token>`
* **Request Body:**
```json
{
  "propertyId": 20
}
```
* **Response (201 Created):**
```json
{
  "message": "Property added to history.",
  "viewId": 482
}
```
* **Error Responses:**
  * `400 Bad Request`: Missing or invalid `propertyId` (e.g., negative or zero).
  * `401 Unauthorized`: Missing or invalid token.
  * `404 Not Found`: Property does not exist.
  * `409 Conflict`: User has already viewed this property (existing record updated instead).

## 2. Get Recent Visits
* **Endpoint:** GET /api/user/viewed-properties?limit=10
* **Description:** select latest 10 visits
* **Query Parameters:**
  * `limit` (integer, optional): max number of visits to return (default `10`, max `10`).
* **Headers:** `Authorization: Bearer <User_Token>`
* **Response (200 OK Status):**
```json
{
  "data": [
    {
      "propertyId": 20,
      "viewedAt": "2026-08-03T10:30:00Z"
    },
    {
      "propertyId": 14,
      "viewedAt": "2026-08-01T15:45:12Z"
    }
  ]
}
```
* **Error Responses:**
  * `400 Bad Request`: Invalid `limit` (e.g., negative or zero).
  * `401 Unauthorized`: Missing or invalid token.
  * `500 Internal Server Error`: An unexpected error occurred on the server. Please try again later.

## 3. Search (Records History Automatically)
* **Endpoint:** GET /api/v1/properties
* **Description:** Performs the search and returns results. The server automatically records the search
  query to the authenticated user's history as a side effect — no separate client call is needed.
* **Headers:** `Authorization: Bearer <User_Token>`
* **Note:** Refer to `searching-filtering.md` for the full query parameters and response format. If the user
  is not authenticated, the search still works but nothing is recorded.
* **Error Responses:**
  * `401 Unauthorized`: Missing or invalid token.

## 4. Get Search History
* **Endpoint:** GET /api/user/search-history
* **Description:** select latest 10 search queries
* **Headers:** `Authorization: Bearer <User_Token>`
* **Response (200 OK Status):**
```json
{
  "data": [
    {
      "searchQueryId": 231,
      "query": "house in newyork",
      "searchedAt": "2026-08-03T10:30:00Z"
    },
    {
      "searchQueryId": 218,
      "query": "villa for rent in damascus",
      "searchedAt": "2026-08-01T15:45:12Z"
    }
  ]
}
```
* **Error Responses:**
  * `401 Unauthorized`: Missing or invalid token. Empty history simply returns `200` with `data: []`.

# 4. Database Schema (Entities & Attributes)

## 1. Table: `PropertyViews`
* **`property_views_id`** (PK): Primary key.
* **`user_id`** (FK): Identifier for the user.
* **`property_id`** (FK): Identifier for the property.
* **`viewed_at`** (DATETIME): Timestamp when property was viewed.

## 2. Table: `SearchQuery`
* **`search_query_id`** (PK): Primary key.
* **`user_id`** (FK): Identifier for the user.
* **`query`** (NVARCHAR(255)): The search query text entered by the user.
* **`searched_at`** (DATETIME): Timestamp when search was executed.