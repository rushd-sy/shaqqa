# 1. User Stories

## User
- As a user, I want my keywords to be recorded automatically so that I can quickly re-pick one from the list under the search field.
- As a user, I want my filter combinations to be recorded automatically so that I can re-run a specific search later from "Saved Searches".
- As a user, I want to remove any single item from my Recent Searches or my Saved Searches.
- As a user, I want to save my recent visits so that I can find anything I visited and then wanted to return to, or that I lost.

# 2. Acceptance Criteria & Edge Cases

## Feature: Recent Visits, Recent Searches, and Saved Searches

> **Clearing up the two search records — what's the difference?**
>
> | | 🅰 Recent Searches | 🅱 Saved Searches |
> |---|---|---|
> | **Meaning of the name** | "Recent" = the last keywords you typed | "Saved" = stored filter combinations |
> | **What it stores** | The raw text keyword only (`q`, e.g. "al-furqan") | Filter parameters only (price, city, rooms, ...) |
> | **Never stores** | Any filter parameters | Any `q` text |
> | **Displayed as** | A text list under the search field | A card in the "Saved Searches" section |
> | **Purpose** | Quickly re-pick a keyword you typed before | Re-run a specific, filtered search |

* **Acceptance Criteria:**
  * **Recent Searches:** text keywords (`q`) are recorded automatically as a side effect of the search
    endpoint — no separate client request is required. They appear as a list under the search field.
  * **Saved Searches:** filter combinations are also recorded automatically, and displayed as cards
    in the "Saved Searches" section, each showing the applied criteria.
  * Recent and Saved records are **independent**: a Recent record never stores filters, and a Saved
    record never stores the `q` text.
  * The user can remove any single record from both the recent-search list and the saved-filter cards list.
  * The system should save recent visits for each user.
* **Business Rules:**
  * Each user is limited to the last 10 visits, 10 keyword searches, and 10 saved filters.
  * Visits, keyword searches, and saved filters: keep only the **last 10** (`LIFO` - most recently recorded
    first); adding an 11th record evicts the oldest.
  * A repeat visit to a property, or a repeated identical keyword search, updates the existing
    record's `viewed_at` / `searched_at` timestamp instead of inserting a duplicate entry.
  * Recording a filter combination identical to an existing one updates the `saved_at` timestamp instead of
    creating a duplicate card.
  * Before storing, the `filters_json` payload must be validated against a strict schema: **only the allowed
    keys** (the search query parameters in `searching-filtering.md`, excluding `q`), correct types, valid
    enum values, and valid ranges (`minPrice` <= `maxPrice`, `minArea` <= `maxArea`). Records failing
    validation are rejected with `400` and never written, keeping the column clean.
  * When returning saved searches, `filters_json` is parsed and re-serialized by the server so the frontend
    always receives a valid `filters` object — never raw, corrupted, or partial JSON.
* **Edge Cases:**
  * A `SavedSearch` row already exists in the database with corrupted or out-of-schema JSON (e.g., legacy
    data, manual DB edits) → the endpoint must not crash: the row is skipped (or returned with a sanitized,
    empty `filters: {}` and a flag) so the frontend never renders a broken card.
  * User has never visited/searched anything → the endpoint returns an empty list (200), not an error.
  * Two rapid/concurrent visits for the same property → must still result in a single record, no duplicates.
  * History is exactly at the limit (10) → nothing is pruned (boundary of the limit rule).
  * A property in the user's "Recent Visits" list was deleted from the platform → filter it out of results
    to avoid broken links.
  * Empty or blank keyword → nothing is recorded to Recent Searches.
  * A search with no filter parameters → nothing is recorded to Saved Searches.
  * Invalid filter values in a search request (e.g., malformed enum, `minPrice` > `maxPrice`) → `400 Bad Request`,
    nothing is recorded.
  * A saved filter whose criteria no longer match any available property → "View Results" returns an empty
    list (`200 data: []`); the saved card remains intact.
  * Deleting a record that does not exist or does not belong to the user → `404 Not Found`.
  * Property does not exist when saving a visit → `404 Not Found`, no record is saved.
  * Unauthenticated request on the visits / history endpoints → `401 Unauthorized`.
  * Unauthenticated search still returns results, but nothing is recorded.
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
* **Description:** Performs the search and returns results. As side effects, the server automatically records
  (independently of each other):
  * the text query → `SearchQuery` (Recent Searches) — only if a `q` was sent,
  * the filter combination → `SavedSearch` (Saved Searches) — only if filter parameters were sent.
* **Headers:** `Authorization: Bearer <User_Token>`
* **Note:** Refer to `searching-filtering.md` for the full query parameters and response format. If the user
  is not authenticated, the search still works but nothing is recorded.

## 4. Get Recent Searches
* **Endpoint:** GET /api/user/recent-searches
* **Description:** select latest 10 keyword searches (Recent Searches — displayed as a list under the search field)
* **Headers:** `Authorization: Bearer <User_Token>`
* **Response (200 OK Status):**
```json
{
  "data": [
    {
      "searchQueryId": 231,
      "query": "al-furqan",
      "searchedAt": "2026-08-03T10:30:00Z"
    },
    {
      "searchQueryId": 218,
      "query": "shahbaa",
      "searchedAt": "2026-08-01T15:45:12Z"
    }
  ]
}
```
* **Error Responses:**
  * `401 Unauthorized`: Missing or invalid token. Empty history simply returns `200` with `data: []`.

## 5. Delete Recent Search
* **Endpoint:** DELETE /api/user/recent-searches/{searchQueryId}
* **Description:** removes a single Recent Search entry (keyword)
* **Path Parameters:**
  * `searchQueryId` (integer, required): The unique ID of the Recent Search entry.
* **Headers:** `Authorization: Bearer <User_Token>`
* **Response (204 No Content):**
* **Error Responses:**
  * `401 Unauthorized`: Missing or invalid token.
  * `404 Not Found`: Entry does not exist or does not belong to the user.

## 6. Get Saved Searches
* **Endpoint:** GET /api/user/saved-searches
* **Description:** select the user's recorded filter combinations (Saved Searches — displayed as cards with their criteria)
* **Headers:** `Authorization: Bearer <User_Token>`
* **Response (200 OK Status):**
```json
{
  "data": [
    {
      "savedSearchId": 96,
      "savedAt": "2026-08-03T11:00:00Z",
      "filters": {
        "minPrice": 10000,
        "maxPrice": 100000,
        "city": "Aleppo New",
        "minRooms": 3,
        "minArea": 80,
        "maxArea": 200,
        "estateType": "APARTMENT",
        "contractType": "SALE"
      }
    }
  ]
}
```
* **Error Responses:**
  * `401 Unauthorized`: Missing or invalid token. Empty list simply returns `200` with `data: []`.

## 7. Delete Saved Search
* **Endpoint:** DELETE /api/user/saved-searches/{savedSearchId}
* **Description:** removes a single saved filter card from the user's "Saved Searches"
* **Path Parameters:**
  * `savedSearchId` (integer, required): The unique ID of the saved filter.
* **Headers:** `Authorization: Bearer <User_Token>`
* **Response (204 No Content):**
* **Error Responses:**
  * `401 Unauthorized`: Missing or invalid token.
  * `404 Not Found`: Saved search does not exist or does not belong to the user.

# 4. Database Schema (Entities & Attributes)

## 1. Table: `PropertyViews`
* **`property_views_id`** (PK): Primary key.
* **`user_id`** (FK): Identifier for the user.
* **`property_id`** (FK): Identifier for the property.
* **`viewed_at`** (DATETIME): Timestamp when property was viewed.

## 2. Table: `SearchQuery`
* **`search_query_id`** (PK): Primary key.
* **`user_id`** (FK): Identifier for the user.
* **`query`** (NVARCHAR(255)): The raw text query entered by the user. Never stores filters.
* **`searched_at`** (DATETIME): Timestamp when search was executed.

## 3. Table: `SavedSearch`
* **`saved_search_id`** (PK): Primary key.
* **`user_id`** (FK): Identifier for the user.
* **`filters_json`** (NVARCHAR(MAX)): JSON payload containing the filter combination. Keys match the
  search query parameters in `searching-filtering.md`. Never contains the `q` text. Must always be written
  through schema validation (see Business Rules) to keep the column clean.
* **`saved_at`** (DATETIME): Timestamp when the filter combination was recorded.