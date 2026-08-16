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
* **Endpoint:** GET /api/v1/advertisements/{advertisementId}
* **Description:** Displays the full advertisement (property details, media, description). As a side effect, the
  server automatically records a recent visit — no separate client request is required.
* **Path Parameters:**
  * `advertisementId` (UUID v7, required): The `PublicId` of the advertisement.
* **Headers:** `Authorization: Bearer <User_Token>` (Optional for public viewing)
* **Note:** Refer to `PropertyListing.md` and `PropertyDetails.md` for the full response format. If the user is
  not authenticated, the advertisement still loads but the visit is not recorded.
* **Error Responses:**
  * `400 Bad Request`: Invalid `advertisementId` (e.g., malformed UUID).
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
      "advertisement_id": "b7f0c8a2-3c4d-5e6f-7081-000000000003",
      "viewed_at": "2026-08-03T10:30:00Z"
    },
    {
      "advertisement_id": "f3e1a9c7-6f70-8192-a3b4-000000000010",
      "viewed_at": "2026-08-01T15:45:12Z"
    }
  ]
}
```
* **Error Responses:**
  * `401 Unauthorized`: Missing or invalid token.

# 4. Database Schema (Entities & Attributes)

> **ID strategy:** `AdvertisementViews` is an internal join/audit table — its own identifier is **never exposed**, so it has **no `PublicId`** (only an internal `Id`). It references `User.PublicId` and `Advertisement.PublicId`.

## 1. Table: `AdvertisementViews`
* **`Id`** (PK, INT, IDENTITY): Internal identifier — **never exposed**.
* **`UserId`** (FK -> `User.PublicId`, UUID): Public identifier of the user.
* **`AdvertisementId`** (FK -> `Advertisement.PublicId`, UUID): Public identifier of the advertisement.
* **`ViewedAt`** (DATETIME): Timestamp when the advertisement was viewed.
* **Unique:** `UNIQUE(UserId, AdvertisementId)` — a repeat visit is an upsert
  (`MERGE ... ON (UserId, AdvertisementId) WHEN MATCHED THEN UPDATE SET ViewedAt = GETDATE()`), so the
  timestamp is refreshed instead of a duplicate-key error. This also guarantees that two rapid/concurrent
  visits still result in a single record. The "keep last 10" eviction runs after the upsert.
