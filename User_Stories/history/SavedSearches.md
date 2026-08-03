# 1. User Stories

## User
- As a user, I want my filter combinations to be recorded automatically so that I can re-run a specific search later from "Saved Searches".
- As a user, I want to remove any single item from my Recent Searches or my Saved Searches.

# 2. Acceptance Criteria & Edge Cases

## Feature: Saved Searches

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
  * **Saved Searches:** filter combinations are recorded automatically, and displayed as cards in the
    "Saved Searches" section, each showing the applied criteria.
  * A Saved record **never** stores the `q` text: Recent and Saved records are **independent**.
  * The user can remove any single record from the saved-filter cards list.
* **Business Rules:**
  * Each user is limited to the last 10 saved filters.
  * Saved filters: keep only the **last 10** (`LIFO` - most recently recorded first); adding an 11th record
    evicts the oldest.
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
  * User has never saved any filter → the endpoint returns an empty list (200), not an error.
  * A search with no filter parameters → nothing is recorded to Saved Searches.
  * Invalid filter values in a search request (e.g., malformed enum, `minPrice` > `maxPrice`) → `400 Bad Request`,
    nothing is recorded.
  * A saved filter whose criteria no longer match any available property → "View Results" returns an empty
    list (`200 data: []`); the saved card remains intact.
  * Deleting a record that does not exist or does not belong to the user → `404 Not Found`.
  * Unauthenticated search still returns results, but nothing is recorded.
  * Unauthenticated request on the saved-searches endpoints → `401 Unauthorized`.
  * The `userId` is always derived from the token; any `userId` sent in the body is ignored → a user can never
    view or modify another user's history.

# 3. API Endpoints

## 1. Search (Records Saved Search Automatically)
* **Endpoint:** GET /api/v1/properties
* **Description:** Performs the search and returns results. As a side effect, the server automatically
  records the filter combination → `SavedSearch` (Saved Searches) — but only if filter parameters were sent.
* **Headers:** `Authorization: Bearer <User_Token>`
* **Note:** Refer to `searching-filtering.md` for the full query parameters and response format. If the user
  is not authenticated, the search still works but nothing is recorded.

## 2. Get Saved Searches
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

## 3. Delete Saved Search
* **Endpoint:** DELETE /api/user/saved-searches/{savedSearchId}
* **Description:** removes a single saved filter card from the user's "Saved Searches"
* **Path Parameters:**
  * `savedSearchId` (integer, required): The ID of the saved filter.
* **Headers:** `Authorization: Bearer <User_Token>`
* **Response (204 No Content):**
* **Error Responses:**
  * `401 Unauthorized`: Missing or invalid token.
  * `404 Not Found`: Saved search does not exist or does not belong to the user.

# 4. Database Schema (Entities & Attributes)

## 1. Table: `SavedSearch`
* **`saved_search_id`** (PK): Primary key.
* **`user_id`** (FK): Identifier for the user.
* **`filters_json`** (NVARCHAR(MAX)): JSON payload containing the filter combination. Keys match the
  search query parameters in `searching-filtering.md`. Never contains the `q` text. Must always be written
  through schema validation (see Business Rules) to keep the column clean.
* **`saved_at`** (DATETIME): Timestamp when the filter combination was recorded.