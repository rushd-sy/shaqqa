# 1. User Stories

## User
- As a user, I want my filter combinations to be recorded automatically so that I can re-run a specific search later from "Recent Filters".
- As a user, I want to remove any single item from my Recent Filters.

# 2. Acceptance Criteria & Edge Cases

## Feature: Recent Filters

> **Clearing up the two search records — what's the difference?**
>
> | | 🅰 Recent Searches | 🅱 Recent Filters |
> |---|---|---|
> | **Meaning of the name** | "Recent" = the last keywords you typed | "Recent" = the last filter combinations you applied |
> | **What it stores** | The raw text keyword only (`q`, e.g. "al-furqan") | Filter parameters only (price, city, rooms, ...) |
> | **Never stores** | Any filter parameters | Any `q` text |
> | **Displayed as** | A text list under the search field | A card in the "Recent Filters" section |
> | **Purpose** | Quickly re-pick a keyword you typed before | Re-run a specific, filtered search |

* **Acceptance Criteria:**
  * **Recent Filters:** filter combinations are recorded automatically, and displayed as cards in the
    "Recent Filters" section, each showing the applied criteria.
  * A Recent Filters record **never** stores the `q` text: Recent Searches and Recent Filters are **independent**.
  * The user can remove any single record from the filter cards list.
* **Business Rules:**
  * Each user is limited to the last 10 saved filters.
  * Filters: keep only the **last 10** (most recently recorded first); adding an 11th record evicts the oldest.
  * Recording a filter combination identical to an existing one updates the `saved_at` timestamp instead of
    creating a duplicate card — implemented as an **upsert** (`MERGE` on `(user_id, filters_hash)`, see
    Database Schema), so the timestamp is refreshed without a duplicate-key error.
  * Before storing, the `filters_json` payload must be validated against a strict schema: **only the allowed
    keys** (the search filter parameters in `searching-filtering.md`, excluding `q`, `page`, `pageSize`,
    `sortBy`, and `sortOrder`), correct types, valid enum values, and valid ranges (`minPrice` <= `maxPrice`,
    `minArea` <= `maxArea`). Records failing validation are rejected with `400` and never written, keeping the
    column clean.
  * When returning recent filters, `filters_json` is parsed and re-serialized by the server so the frontend
    always receives a valid `filters` object — never raw, corrupted, or partial JSON.
* **Edge Cases:**
  * A `RecentFilter` row already exists in the database with corrupted or out-of-schema JSON (e.g., legacy
    data, manual DB edits) → the endpoint must not crash: the row is skipped (or returned with a sanitized,
    empty `filters: {}` and a flag) so the frontend never renders a broken card.
  * User has never saved any filter → the endpoint returns an empty list (200), not an error.
  * A search with no filter parameters → nothing is recorded to Recent Filters.
  * Invalid filter values in a search request (e.g., malformed enum, `minPrice` > `maxPrice`) → `400 Bad Request`,
    nothing is recorded.
  * A filter whose criteria no longer match any available property → "View Results" returns an empty
    list (`200 data: []`); the filter card remains intact.
  * Deleting a record that does not exist or does not belong to the user → `404 Not Found`.
  * Unauthenticated search still returns results, but nothing is recorded.
  * Unauthenticated request on the recent-filters endpoints → `401 Unauthorized`.
  * The `userId` is always derived from the token; any `userId` sent in the body is ignored → a user can never
    view or modify another user's history.

# 3. API Endpoints

## 1. Search (Records Recent Filter Automatically)
* **Endpoint:** GET /api/v1/properties
* **Description:** Performs the search and returns results. As a side effect, the server automatically
  records the filter combination → `RecentFilter` (Recent Filters) — but only if filter parameters were sent.
* **Headers:** `Authorization: Bearer <User_Token>`
* **Note:** Refer to `searching-filtering.md` for the full query parameters and response format. If the user
  is not authenticated, the search still works but nothing is recorded.

## 2. Get Recent Filters
* **Endpoint:** GET /api/user/recent-filters
* **Description:** select the user's recorded filter combinations (Recent Filters — displayed as cards with their criteria)
* **Headers:** `Authorization: Bearer <User_Token>`
* **Response (200 OK Status):**
```json
{
  "data": [
    {
      "recent_filter_id": "8f92a3b5-e7f8-09a1-2b3c-00000000000e",
      "saved_at": "2026-08-03T11:00:00Z",
      "filters": {
        "min_price": 10000,
        "max_price": 100000,
        "city": "Aleppo New",
        "min_rooms": 3,
        "min_area": 80,
        "max_area": 200,
        "estate_type": "APARTMENT",
        "contract_type": "SALE"
      }
    }
  ]
}
```
* **Error Responses:**
  * `401 Unauthorized`: Missing or invalid token. Empty list simply returns `200` with `data: []`.

## 3. Delete Recent Filter
* **Endpoint:** DELETE /api/user/recent-filters/{recentFilterId}
* **Description:** removes a single filter card from the user's "Recent Filters"
* **Path Parameters:**
  * `recentFilterId` (UUID v7, required): The `PublicId` of the filter card.
* **Headers:** `Authorization: Bearer <User_Token>`
* **Response (204 No Content):**
* **Error Responses:**
  * `401 Unauthorized`: Missing or invalid token.
  * `404 Not Found`: Recent filter does not exist or does not belong to the user.

# 4. Database Schema (Entities & Attributes)

> **ID strategy:** `RecentFilter` exposes `PublicId` (UUID v7, indexed, UNIQUE) as `recent_filter_id` in the response. The internal `Id` (INT, PK) is never exposed.

## 1. Table: `RecentFilter`
* **`Id`** (PK, INT, IDENTITY): Internal identifier — **never exposed**.
* **`PublicId`** (UUID v7, UNIQUE, INDEXED): Public identifier; exposed as `recent_filter_id` in the response.
* **`UserId`** (FK -> `User.PublicId`, UUID): Public identifier of the user.
* **`FiltersJson`** (NVARCHAR(MAX)): JSON payload containing the filter combination. Keys match the
  search filter parameters in `searching-filtering.md`. Never contains the `q` text. Must always be written
  through schema validation (see Business Rules) to keep the column clean.
* **`FiltersHash`** (CHAR(64)): SHA-256 hash of the canonicalized `FiltersJson` (keys sorted alphabetically,
  whitespace normalized). Uniqueness lives here because `NVARCHAR(MAX)` cannot be indexed.
* **`SavedAt`** (DATETIME): Timestamp when the filter combination was recorded.
* **Unique:** `UNIQUE(UserId, FiltersHash)` — an identical filter combination is an upsert
  (`MERGE ... ON (UserId, FiltersHash) WHEN MATCHED THEN UPDATE SET SavedAt = GETDATE()`), so the
  timestamp is refreshed instead of a duplicate-key error. The "keep last 10" eviction runs after the upsert.
