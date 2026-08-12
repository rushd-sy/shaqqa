# 1. User Stories

## User
- As a user, I want to save my recent visits so that I can find anything I visited and then wanted to return to, or that I lost.

# 2. Acceptance Criteria & Edge Cases

## Feature: Recent Visits

* **Acceptance Criteria:**
  * Recent visits are recorded automatically as a side effect of opening the advertisement (GET advertisement
    details) — no separate client request is required.
  * The system should save recent visits for each user.
* **Business Rules:**
  * Each user is limited to the last 10 visits.
  * Visits: keep only the **last 10** (most recently recorded first); adding an 11th record evicts the oldest.
  * A repeat visit to an advertisement updates the existing record's `viewed_at` timestamp instead of inserting
    a duplicate entry — implemented as an **upsert** on `UNIQUE(user_id, advertisement_id)` (see Database Schema).
* **Edge Cases:**
  * User has never visited anything → the endpoint returns an empty list (200), not an error.
  * Two rapid/concurrent visits for the same advertisement → must still result in a single record, no duplicates.
  * History is exactly at the limit (10) → nothing is pruned (boundary of the limit rule).
  * An advertisement in the user's "Recent Visits" list was deleted from the platform → filter it out of results to avoid broken links.
  * Opening an advertisement that does not exist or is inactive → `404 Not Found`, no record is saved.
  * Unauthenticated request on the history endpoints → `401 Unauthorized`; an unauthenticated user can still
    open and view the advertisement, but nothing is recorded.
  * The `userId` is always derived from the token → a user can never view or modify another user's history.

# 3. API Endpoints

## 1. Get Advertisement Details (Records Visit Automatically)
* **Endpoint:** GET /api/advertisements/{id_advertisement}
* **Description:** Displays the full advertisement (property details, media, description). As a side effect, the
  server automatically records a recent visit — no separate client request is required.
* **Path Parameters:**
  * `id_advertisement` (INT, required): The unique identifier of the advertisement.
* **Headers:** `Authorization: Bearer <User_Token>` (Optional for public viewing)
* **Note:** Refer to `PropertyListing.md` and `PropertyDetails.md` for the full response format. If the user is
  not authenticated, the advertisement still loads but the visit is not recorded.
* **Error Responses:**
  * `400 Bad Request`: Invalid `id_advertisement` (e.g., negative or zero).
  * `404 Not Found`: Advertisement does not exist or is inactive/deleted. Nothing is recorded.

## 2. Get Recent Visits
* **Endpoint:** GET /api/user/recent-visits
* **Description:** select latest 10 visits
* **Headers:** `Authorization: Bearer <User_Token>`
* **Response (200 OK Status):**
```json
{
  "data": [
    {
      "advertisementId": 20,
      "viewedAt": "2026-08-03T10:30:00Z"
    },
    {
      "advertisementId": 14,
      "viewedAt": "2026-08-01T15:45:12Z"
    }
  ]
}
```
* **Error Responses:**
  * `401 Unauthorized`: Missing or invalid token.

# 4. Database Schema (Entities & Attributes)

## 1. Table: `AdvertisementViews`
* **`advertisement_views_id`** (PK): Primary key.
* **`user_id`** (FK): Identifier for the user.
* **`advertisement_id`** (FK -> `Advertisement.id_advertisement`): Identifier for the advertisement.
* **`viewed_at`** (DATETIME): Timestamp when the advertisement was viewed.
* **Unique:** `UNIQUE(user_id, advertisement_id)` — a repeat visit is an upsert
  (`MERGE ... ON (user_id, advertisement_id) WHEN MATCHED THEN UPDATE SET viewed_at = GETDATE()`), so the
  timestamp is refreshed instead of a duplicate-key error. This also guarantees that two rapid/concurrent
  visits still result in a single record. The "keep last 10" eviction runs after the upsert.