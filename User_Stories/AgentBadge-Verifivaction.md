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
  "agent_id": "3a1c9e57-1a2b-4c3d-8e9f-000000000001",
  "badge_name": "Professional Agent",
  "granted_at_score": 82.5
}
```
* **Response (201 Created):**
```json
{
  "agent_badge_id": "5e6f7081-8192-a3b4-c5d6-000000000007",
  "agent_id": "3a1c9e57-1a2b-4c3d-8e9f-000000000001",
  "granted_by": "7c4d2a91-6f70-8192-a3b4-000000000006",
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
* **Endpoint:** `PUT /api/v1/agent-badges/{agentBadgeId}`
* **Description:** Revokes an active "Professional Agent" badge.
* **Path Parameters:**
    * `agentBadgeId` (UUID v7, required): The `PublicId` of the agent badge.
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
  "agent_badge_id": "5e6f7081-8192-a3b4-c5d6-000000000007",
  "agent_id": "3a1c9e57-1a2b-4c3d-8e9f-000000000001",
  "granted_by": "7c4d2a91-6f70-8192-a3b4-000000000006",
  "revoked_by": "b2e8f1c4-7f70-8192-a3b4-000000000011",
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
    * `404 Not Found`: No badge found with the provided `agentBadgeId`.

# 4. Database Schema (Entities & Attributes)

> **ID strategy:** `Company` and `AgentBadge` expose `PublicId` (UUID v7, indexed, UNIQUE) used in endpoints/FKs. The internal `Id` (INT, PK) is never exposed. `TrustMetric` is referenced only by `agent_id` (a User `PublicId`) and is never addressed by its own id, so it has **no `PublicId`** — only an internal `Id`.

## 1. Table: `Company`
* **`Id`** (PK, INT, IDENTITY): Internal identifier for the real estate company — **never exposed**.
* **`PublicId`** (UUID v7, UNIQUE, INDEXED): Public identifier for the company; exposed as `company_id` in JWT claims and responses and used as the FK target.
* **`Name`** (VARCHAR): Name of the real estate company.
* **`LicenseNumber`** (VARCHAR, UNIQUE): Unique license number of the company.
* **`LogoFileId`** (FK -> `File.PublicId`, UUID, NULLABLE): Public identifier of the stored company logo. Logos live in the shared `File` table (see `FileStorage.md`) and are served via `GET /api/v1/files/{fileId}` — no URL/path is stored.
* **`Verified`** (BOOLEAN): `TRUE` if the company is verified.
* **`IsActive`** (BOOLEAN): Default `TRUE`. If `FALSE`, all company staff under this company are prevented from logging in.
* **`CreatedAt`** (TIMESTAMP): Timestamp indicating when the record was created.

> This entity was previously named `RealEstateCompany` in this document; it is unified to `Company` (canonical name in `users_doc/company.md`).

## 2. Table: `User`
* **`Id`** (PK, INT, IDENTITY): Internal identifier for the user — **never exposed**.
* **`PublicId`** (UUID v7, UNIQUE, INDEXED): Public identifier for the user; exposed as `user_id` in all endpoints/JWT claims and used as the FK target.
* **`CompanyId`** (FK -> `Company.PublicId`, UUID, NULLABLE): Public identifier of the user's affiliated real estate company.
* **`Username`** (VARCHAR, Unique): Unique identifier name (optional).
* **`Phone`** (VARCHAR, Unique): Primary login identifier (with country code).
* **`FirstName`** (VARCHAR, NULLABLE): User's first name. Set during the registration name step.
* **`LastName`** (VARCHAR, NULLABLE): User's last name. Set during the registration name step.
* **`Role`** (ENUM): The role of the user (`CUSTOMER`, `BROKER`, `COMPANY_ADMIN`, `COMPANY_STAFF`, `SHAQQA_ADMIN`, `SHAQQA_STAFF`). See `users_doc/all-users.md` for the canonical definition.
* **`IsActive`** (BOOLEAN): Default `TRUE`. Used by admins to ban or suspend users.
* **`IsVerified`** (BOOLEAN): Default `FALSE`. Set to `TRUE` after the user verifies their phone number via registration OTP.
* **`CreatedAt`** (TIMESTAMP): Timestamp indicating when the user was created.

> The `User` table is defined in full in `users_doc/all-users.md`; it is repeated here for the FK relationships in this document.

## 3. Table: `TrustMetric`
* **`Id`** (PK, INT, IDENTITY): Internal identifier for the trust metric record — **never exposed** (queried only by `agent_id`).
* **`AgentId`** (FK -> `User.PublicId`, UUID): Public identifier of the broker.
* **`HasPhotoVerifiedByAdmin`** (BOOLEAN): `TRUE` if the admin has verified the broker's photo.
* **`ProfessionalPostsRatio`** (DECIMAL(3,2)): The ratio calculated for professional posts.
* **`PostsThisMonth`** (INT): The total number of posts created by the broker this month.
* **`ActiveDaysLast30`** (INT): Number of days the broker was active in the last 30 days.
* **`PostsProfScore`** (DECIMAL(5,2)): The calculated score derived from professional posts.
* **`MonthlyPostsScore`** (DECIMAL(5,2)): The calculated score derived from monthly posts.
* **`ActivityScore`** (DECIMAL(5,2)): The calculated overall activity score.
* **`TrustScore`** (DECIMAL(5,2)): The final calculated overall trust score.
* **`CalculatedAt`** (TIMESTAMP): Timestamp indicating when the metrics were last calculated.

## 4. Table: `AgentBadge`
* **`Id`** (PK, INT, IDENTITY): Internal identifier for the agent badge record — **never exposed**.
* **`PublicId`** (UUID v7, UNIQUE, INDEXED): Public identifier for the badge; exposed as `agent_badge_id` in all endpoints and used as the FK target.
* **`AgentId`** (FK -> `User.PublicId`, UUID): Public identifier of the broker receiving the badge.
* **`GrantedBy`** (FK -> `User.PublicId`, UUID): Public identifier of the admin who granted the badge.
* **`RevokedBy`** (FK -> `User.PublicId`, UUID, NULLABLE): Public identifier of the admin who revoked the badge.
* **`BadgeName`** (VARCHAR, DEFAULT 'Professional Agent'): The name identifier of the badge.
* **`Status`** (ENUM): The current status of the agent's badge (`ACTIVE`, `REVOKED`).
* **`GrantedAt`** (TIMESTAMP): Timestamp indicating when the badge was granted.
* **`RevokedAt`** (TIMESTAMP, NULLABLE): Timestamp indicating when the badge was revoked.
* **`GrantedAtScore`** (DECIMAL(5,2)): The trust score evaluated at the time the badge was granted.
* **`RevokedAtScore`** (DECIMAL(5,2)): The trust score evaluated at the time the badge was revoked.
