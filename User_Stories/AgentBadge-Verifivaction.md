# 1. User Stories

## Shaqqa Admin
- As a Shaqqa Admin, I want to view the trust score details for any broker so that I can make an informed decision about granting or revoking the badge.
- As a Shaqqa Admin, I want to grant the "Professional Agent" badge to a specific broker so that users can recognize trusted brokers on the platform.
- As a Shaqqa Admin, I want to revoke the "Professional Agent" badge from a specific broker so that the badge remains credible and reflects current actual performance.

## Regular User (Customer)
- As a Regular User, I want to see the "Professional Agent" badge on the broker's profile so that I can trust the broker I interact with.

> **Note:** Property listing verification is **not** part of this document anymore. All advertisement review/verification workflows live in `VerificationRequest.md` (and the admin review stories in `users_doc/shaqqa-admin-staff.md`).

# 2. Acceptance Criteria & Edge Cases

## Feature: View Trust Score Details
* **Acceptance Criteria:**
    * The admin can view the total `trust_score` for any broker.
    * The admin can view each criterion score separately.
    * The date of the last calculation (`calculated_at`) is displayed.
* **Edge Cases:**
    * Record exists but is partially complete -> missing fields are displayed as `0` instead of `NULL`.
    * Outdated data (last calculation is very old) -> the system may indicate that the data is not up-to-date while still displaying it.

## Feature: Grant Professional Agent Badge
* **Acceptance Criteria:**
    * The admin can only grant the badge to users with role = `BROKER`.
    * `trust_score` must be >= `80` at the time of granting.
    * The broker's profile photo must be verified and approved by the admin (`has_photo_verified_by_admin` = `TRUE`).
    * `granted_by`, `granted_at`, and `granted_at_score` are recorded.
    * The badge status is set to `ACTIVE` immediately upon granting.
* **Edge Cases:**
    * The broker temporarily meets the criteria due to a sudden score spike -> an optional waiting period can be applied before granting.

## Feature: Revoke Professional Agent Badge
* **Acceptance Criteria:**
    * The badge is automatically revoked when `trust_score` drops to <= `70`.
    * `revoked_by`, `revoked_at`, and `revoked_at_score` are recorded.
    * The badge status changes from `ACTIVE` to `REVOKED`.
    * The badge is immediately hidden from the broker's profile.
* **Edge Cases:**
    * None specified.

## Feature: Display Professional Agent Badge
* **Acceptance Criteria:**
    * The badge is displayed only when status = `ACTIVE`.
    * No badge is displayed if the broker never received it or if it has been revoked.
* **Edge Cases:**
    * None specified.

# 3. API Endpoints (Login & Roles)

> All endpoints are versioned under `/api/v1/`.

## 1. Create Agent Badge
* **Endpoint:** `POST /api/v1/agent-badges`
* **Description:** Creates a "Professional Agent" badge and assigns it to a specific broker.
* **Headers:** `Authorization: Bearer <Shaqqa_Admin_Token>`
* **Request Body:**
```json
{
  "agent_id": "3a1c9e57-...-uuid",
  "badge_name": "Professional Agent",
  "granted_at_score": 82.5
}
```
* **Response (201 Created):**
```json
{
  "id": 1,
  "agent_id": "3a1c9e57-...-uuid",
  "granted_by": "7c4d2a91-...-uuid",
  "badge_name": "Professional Agent",
  "status": "ACTIVE",
  "granted_at": "2026-04-29T10:00:00Z",
  "granted_at_score": 82.5
}
```
* **Error Responses:**
    * `400 Bad Request`: `trust_score` below 80, profile photo not verified, or invalid payload.
    * `401 Unauthorized`: Missing or invalid token.
    * `403 Forbidden`: User is not a Shaqqa Admin.

## 2. Revoke Agent Badge
* **Endpoint:** `PUT /api/v1/agent-badges/{id}`
* **Description:** Revokes an active "Professional Agent" badge.
* **Path Parameters:**
    * `id`: The unique identifier of the agent badge.
* **Headers:** `Authorization: Bearer <Shaqqa_Admin_Token>`
* **Request Body:**
```json
{
  "status": "REVOKED",
  "revoked_at_score": 58.0
}
```
* **Response (200 OK):**
```json
{
  "id": 1,
  "agent_id": "3a1c9e57-...-uuid",
  "granted_by": "7c4d2a91-...-uuid",
  "revoked_by": "b2e8f1c4-...-uuid",
  "badge_name": "Professional Agent",
  "status": "REVOKED",
  "granted_at": "2026-04-20T09:00:00Z",
  "revoked_at": "2026-04-29T12:00:00Z",
  "granted_at_score": 82.5,
  "revoked_at_score": 58.0
}
```
* **Error Responses:**
    * `400 Bad Request`: Invalid status value provided.
    * `401 Unauthorized`: Missing or invalid token.
    * `403 Forbidden`: User is not a Shaqqa Admin.
    * `404 Not Found`: No badge found with the provided `id`.

# 4. Database Schema (Entities & Attributes)

## 1. Table: `RealEstateCompany`
* **`id`** (PK, INT): Unique identifier for the real estate company.
* **`name`** (VARCHAR): Name of the real estate company.
* **`license_number`** (VARCHAR, UNIQUE): Unique license number of the company.
* **`logo_url`** (VARCHAR): URL path for the company logo.
* **`verified`** (BOOLEAN): `TRUE` if the company is verified.
* **`created_at`** (TIMESTAMP): Timestamp indicating when the record was created.

## 2. Table: `User`
* **`id_user`** (PK): Unique identifier for the user.
* **`name`** (VARCHAR): Full name of the user.
* **`email`** (VARCHAR, UNIQUE): Unique email address of the user.
* **`phone`** (VARCHAR): Contact phone number.
* **`password_hash`** (VARCHAR): Encrypted password hash for authentication.
* **`role`** (ENUM): The role of the user (`CUSTOMER`, `BROKER`, `COMPANY_ADMIN`, `COMPANY_STAFF`, `SHAQQA_ADMIN`, `SHAQQA_STAFF`). See `users_doc/all-users.md` for the canonical definition.
* **`company_id`** (FK -> `RealEstateCompany.id`, NULLABLE): Foreign key referencing the user's affiliated real estate company.
* **`created_at`** (TIMESTAMP): Timestamp indicating when the user was created.

## 3. Table: `TrustMetric`
* **`id`** (PK, INT): Unique identifier for the trust metric record.
* **`agent_id`** (FK -> `User.id_user`, UUID): Foreign key referencing the broker.
* **`has_photo_verified_by_admin`** (BOOLEAN): `TRUE` if the admin has verified the broker's photo.
* **`professional_posts_ratio`** (DECIMAL(3,2)): The ratio calculated for professional posts.
* **`posts_this_month`** (INT): The total number of posts created by the broker this month.
* **`active_days_last_30`** (INT): Number of days the broker was active in the last 30 days.
* **`posts_prof_score`** (DECIMAL(5,2)): The calculated score derived from professional posts.
* **`monthly_posts_score`** (DECIMAL(5,2)): The calculated score derived from monthly posts.
* **`activity_score`** (DECIMAL(5,2)): The calculated overall activity score.
* **`trust_score`** (DECIMAL(5,2)): The final calculated overall trust score.
* **`calculated_at`** (TIMESTAMP): Timestamp indicating when the metrics were last calculated.

## 4. Table: `AgentBadge`
* **`id`** (PK, INT): Unique identifier for the agent badge record.
* **`agent_id`** (FK -> `User.id_user`, UUID): Foreign key referencing the broker receiving the badge.
* **`granted_by`** (FK -> `User.id_user`, UUID): Foreign key referencing the admin who granted the badge.
* **`revoked_by`** (FK -> `User.id_user`, UUID, NULLABLE): Foreign key referencing the admin who revoked the badge.
* **`badge_name`** (VARCHAR, DEFAULT 'Professional Agent'): The name identifier of the badge.
* **`status`** (ENUM): The current status of the agent's badge (`ACTIVE`, `REVOKED`).
* **`granted_at`** (TIMESTAMP): Timestamp indicating when the badge was granted.
* **`revoked_at`** (TIMESTAMP, NULLABLE): Timestamp indicating when the badge was revoked.
* **`granted_at_score`** (DECIMAL(5,2)): The trust score evaluated at the time the badge was granted.
* **`revoked_at_score`** (DECIMAL(5,2)): The trust score evaluated at the time the badge was revoked.
