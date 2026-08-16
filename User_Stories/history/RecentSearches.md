# 1. User Stories

## User
- As a user, I want my keywords to be recorded automatically so that I can quickly re-pick one from the list under the search field.
- As a user, I want to remove any single item from my Recent Searches.

# 2. Acceptance Criteria & Edge Cases

## Feature: Recent Searches

> **How Recent Searches differ from Recent Filters:**
>
> | | 🅰 Recent Searches | 🅱 Recent Filters |
> |---|---|---|
> | **Meaning of the name** | "Recent" = the last keywords you typed | "Recent" = the last filter combinations you applied |
> | **What it stores** | The raw text keyword only (`q`, e.g. "al-furqan") | Filter parameters only (price, city, rooms, ...) |
> | **Never stores** | Any filter parameters | Any `q` text |
> | **Displayed as** | A text list under the search field | A card in the "Recent Filters" section |
> | **Purpose** | Quickly re-pick a keyword you typed before | Re-run a specific, filtered search |

* **Acceptance Criteria:**
  * **Recent Searches:** text keywords (`q`) are recorded automatically as a side effect of the search
    endpoint — no separate client request is required. They appear as a list under the search field.
  * A Recent record never stores filters: Recent Searches and Recent Filters are **independent**.
  * The user can remove any single record from the recent-search list.
* **Business Rules:**
  * Each user is limited to the last 10 keyword searches.
  * Keyword searches: keep only the **last 10** (most recently recorded first); adding an 11th
    record evicts the oldest.
  * A repeated identical keyword search updates the existing record's `searched_at` timestamp instead of
    inserting a duplicate entry — implemented as an **upsert** on `UNIQUE(user_id, query)` (see Database Schema).
* **Edge Cases:**
  * User has never searched anything → the endpoint returns an empty list (200), not an error.
  * Empty or blank keyword → nothing is recorded to Recent Searches.
  * Unauthenticated search still returns results, but nothing is recorded.
  * Unauthenticated request on the recent-searches endpoints → `401 Unauthorized`.
  * Deleting a record that does not exist or does not belong to the user → `404 Not Found`.
  * The `userId` is always derived from the token → a user can never view or modify another user's history.

# 3. API Endpoints

## 1. Search (Records Recent Search Automatically)
* **Endpoint:** GET /api/v1/properties
* **Description:** Performs the search and returns results. As a side effect, the server automatically
  records the text query → `SearchQuery` (Recent Searches) — but only if a `q` was sent.
* **Headers:** `Authorization: Bearer <User_Token>`
* **Note:** Refer to `searching-filtering.md` for the full query parameters and response format. If the user
  is not authenticated, the search still works but nothing is recorded.

## 2. Get Recent Searches
* **Endpoint:** GET /api/user/recent-searches
* **Description:** select latest 10 keyword searches (Recent Searches — displayed as a list under the search field)
* **Headers:** `Authorization: Bearer <User_Token>`
* **Response (200 OK Status):**
```json
{
  "data": [
    {
      "search_query_id": "7f8192a4-d6e7-f809-1b2c-00000000000d",
      "query": "al-furqan",
      "searched_at": "2026-08-03T10:30:00Z"
    },
    {
      "search_query_id": "8f92a3b5-e7f8-09a1-2b3c-00000000000e",
      "query": "shahbaa",
      "searched_at": "2026-08-01T15:45:12Z"
    }
  ]
}
```
* **Error Responses:**
  * `401 Unauthorized`: Missing or invalid token. Empty history simply returns `200` with `data: []`.

## 3. Delete Recent Search
* **Endpoint:** DELETE /api/user/recent-searches/{searchQueryId}
* **Description:** removes a single Recent Search entry (keyword)
* **Path Parameters:**
  * `searchQueryId` (UUID v7, required): The `PublicId` of the Recent Search entry.
* **Headers:** `Authorization: Bearer <User_Token>`
* **Response (204 No Content):**
* **Error Responses:**
  * `401 Unauthorized`: Missing or invalid token.
  * `404 Not Found`: Entry does not exist or does not belong to the user.

# 4. Database Schema (Entities & Attributes)

> **ID strategy:** `SearchQuery` exposes `PublicId` (UUID v7, indexed, UNIQUE) as `search_query_id` in the response. The internal `Id` (INT, PK) is never exposed.

## 1. Table: `SearchQuery`
* **`Id`** (PK, INT, IDENTITY): Internal identifier — **never exposed**.
* **`PublicId`** (UUID v7, UNIQUE, INDEXED): Public identifier; exposed as `search_query_id` in the response.
* **`UserId`** (FK -> `User.PublicId`, UUID): Public identifier of the user.
* **`Query`** (NVARCHAR(255)): The raw text query entered by the user. Never stores filters.
* **`SearchedAt`** (DATETIME): Timestamp when search was executed.
* **Unique:** `UNIQUE(UserId, Query)` — a repeated identical keyword is an upsert
  (`MERGE ... ON (UserId, Query) WHEN MATCHED THEN UPDATE SET SearchedAt = GETDATE()`), so the timestamp is
  refreshed instead of a duplicate-key error. The "keep last 10" eviction runs after the upsert.
