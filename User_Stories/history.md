# 1. User Stories

## User
- As a user, I want to save my search criteria so that I can quickly check for updates later without re-entering filters.
- As a user, I want to save my recent visits so that I can find anything I visited and then wanted to return to, or that I lost.

# 2. Acceptance Criteria & Edge Cases

## Feature: Recent Visits and Search History
* **Acceptance Criteria:**
  * There should be a history for each user searching.
  * The system should save recent visits for each user.
* **Edge Cases:**
  * Maybe the user could not see all his history.
  * The user can only save recent filter.
  * The user can only show last 10 visits.
  * If a user visits the same property multiple times, the system should update the `viewed_at` timestamp of the existing record instead of creating a duplicate entry.
  * If a user performs an identical search (same filter parameters), the system should update the `searched_at` timestamp rather than appending a new record to the history.
  * The system should enforce a maximum limit of the last 10 visits/searches. Records exceeding this limit must be automatically pruned (`FIFO` - First In, First Out).
  * Deleted Property Handling: If a property in the user's "Recent Visits" list is deleted from the platform, the system must filter it out and ensure it does not appear in the results to prevent broken links.

# 3. API Endpoints (Login & Roles)

## 1. Save Recent Visits or Searches
* **Endpoint:** POST /api/v1/recent-visits
* **Description:** save recent visits
* **Request Body:**
```json
{
  "user_id": "",
  "property_id": ""
}
```
* **Response (201 OK Status):**
```json
{
  "message": "Property added to history."
}
```

## 2. Get Recent Visits
* **Endpoint:** GET /api/user/viewed-properties?limit=10
* **Description:** select latest 10 visits
* **Response (200 OK Status):**
```json
{
  "data": [
    {
      "propertyId": "",
      "viewedAt": ""
    }
  ]
}
```
* **Error Responses:**
  * `400 Bad Request`: not found property_id
  * `401 Unauthorized`: You must be logged in to view history.
  * `500 Internal Server Error`: An unexpected error occurred on the server. Please try again later.

## 3. Save Filter History
* **Endpoint:** POST /api/user/filter-history
* **Description:** save one recent filter
* **Request Body:**
```json
{
  "filters": {
    "minPrice": "",
    "maxPrice": "",
    "location": "",
    "minRooms": "",
    "minArea": "",
    "maxArea": "",
    "estateType": "",
    "contractType": ""
  }
}
```
* **Response (201 Created Status):**
```json
{
  "message": "Search history saved successfully.",
  "id": ""
}
```

## 4. Get Search History
* **Endpoint:** GET /api/user/search-history
* **Description:** save one recent filter
* **Response (200 OK Status):**
```json
{
  "data": [
    {
      "id": "",
      "searchedAt": "",
      "filters": {
        "minPrice": "",
        "maxPrice": "",
        "location": "",
        "minRooms": "",
        "minArea": "",
        "maxArea": "",
        "estateType": "",
        "contractType": ""
      }
    }
  ]
}
```

# 4. Database Schema (Entities & Attributes)

## 1. Table: `PropertyViews`
* **`property_views_id`** (PK): Primary key.
* **`user_id`** (FK): Identifier for the user.
* **`property_id`** (FK): Identifier for the property.
* **`viewed_at`** (DATETIME): Timestamp when property was viewed.

## 2. Table: `FilterHistory`
* **`filter_history_id`** (PK): Primary key.
* **`user_id`** (FK): Identifier for the user.
* **`filters_json`** (NVARCHAR(MAX)): JSON payload containing filter parameters.
* **`searched_at`** (DATETIME): Timestamp when search was executed.