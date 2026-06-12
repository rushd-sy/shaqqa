# 1. User Stories

## Admin
- As an Admin, I want to view the trust score details for any agent so that I can make an informed decision about granting or revoking the badge.
- As an Admin, I want to grant the "Professional Agent" badge to a specific agent so that users can recognize trusted agents on the platform.
- As an Admin, I want to revoke the "Professional Agent" badge from a specific agent so that the badge remains credible and reflects current actual performance.
- As an Admin, I want to review the submitted property listing so that I can approve it, request edits, or reject it.
- As an Admin, I want to request edits on the property listing so that the user can correct it before publishing.
- As an Admin, I want to reject a property listing that does not meet platform standards so that low-quality or fake listings are removed.
- As an Admin, I want to grant a verification mark to a property listing when supporting documents are available so that users trust the authenticity of the listed property.

## Regular User
- As a Regular User, I want to see the "Professional Agent" badge on the agent’s profile so that I can trust the agent I interact with.

## Regular User or Agent
- As a Regular User or Agent, I want to publish a property listing so that it becomes visible to the public.

## User
- As a User, I want to upload supporting documents with my property listing so that the admin can verify the property and grant a trust mark.

# 2. Acceptance Criteria & Edge Cases

## Feature: View Trust Score Details
* **Acceptance Criteria:**
    * The admin can view the total `trust_score` for any agent.
    * The admin can view each criterion score separately.
    * The date of the last calculation (`calculated_at`) is displayed.
* **Edge Cases:**
    * Record exists but is partially complete -> missing fields are displayed as `0` instead of `NULL`.
    * Outdated data (last calculation is very old) -> the system may indicate that the data is not up-to-date while still displaying it.

## Feature: Grant Professional Agent Badge
* **Acceptance Criteria:**
    * The admin can only grant the badge to users with role = `AGENT`.
    * `trust_score` must be >= `80` at the time of granting.
    * The agent’s profile photo must be verified and approved by the admin (`has_photo_verified_by_admin` = `TRUE`).
    * `granted_by`, `granted_at`, and `granted_at_score` are recorded.
    * The badge status is set to `ACTIVE` immediately upon granting.
* **Edge Cases:**
    * The agent temporarily meets the criteria due to a sudden score spike -> an optional waiting period can be applied before granting.

## Feature: Revoke Professional Agent Badge
* **Acceptance Criteria:**
    * The badge is automatically revoked when `trust_score` drops to <= `70`.
    * `revoked_by`, `revoked_at`, and `revoked_at_score` are recorded.
    * The badge status changes from `ACTIVE` to `REVOKED`.
    * The badge is immediately hidden from the agent’s profile.
* **Edge Cases:**
    * None specified.

## Feature: Display Professional Agent Badge
* **Acceptance Criteria:**
    * The badge is displayed only when status = `ACTIVE`.
    * No badge is displayed if the agent never received it or if it has been revoked.
* **Edge Cases:**
    * None specified.

## Feature: Submit Property Listing for Review
* **Acceptance Criteria:**
    * Any user with role = `REGULAR` or `AGENT` can submit a property listing.
    * A `VerificationRequest` record is automatically created upon submission.
    * `VerificationRequest.status` defaults to `PENDING`.
    * `Property.status` defaults to `PENDING`.
    * The listing is not visible to other users until status = `ACTIVE`.
    * A clear and deterministic state mapping between `VerificationRequest.status` and `Property.status` must be defined and enforced as follows:
        * `PENDING` ⟶ `Property.status` = `PENDING`
        * `APPROVED` ⟶ `Property.status` = `ACTIVE`
        * `REJECTED` ⟶ `Property.status` = `REJECTED`
        * `NEEDS_EDIT` ⟶ `Property.status` = `PENDING`
    * State transitions must be deterministic with no undefined or implicit behaviors.
    * `Property.status` can only become `ACTIVE` through an `APPROVED` `VerificationRequest` transition.
* **Edge Cases:**
    * If a listing is submitted without required fields → the operation is rejected before creating `Property` or `VerificationRequest`.
    * If edits are requested and the user does not respond within one week → `VerificationRequest.status` is set to `REJECTED` and `Property.status` = `REJECTED`.
    * A property cannot become visible (`ACTIVE`) except through an explicit `APPROVED` transition.

## Feature: Upload Supporting Documents
* **Acceptance Criteria:**
    * The user can upload one or more documents to `VerificationRequest`.
    * Documents are stored in the `documents_url` field.
    * Uploading documents does not automatically change the status ← admin reviews manually.
* **Edge Cases:**
    * User uploads an unsupported file format -> rejected with a clear error message.
    * User uploads a file exceeding the allowed size -> rejected with a clear error message.

## Feature: Admin Review of Property Listing
* **Acceptance Criteria:**
    * The admin can view all `VerificationRequest` records with status = `PENDING`.
    * The admin can change the status to: `APPROVED`, `NEEDS_EDIT`, or `REJECTED`.
    * `reviewed_by` is set to the admin’s `User.id` upon any action.
    * `reviewed_at` is recorded at the time of action.
    * The admin can add an `admin_note` to explain the decision.
* **Edge Cases:**
    * Two admins open the same request simultaneously -> the first action is applied, the second admin sees the updated status.
    * Admin rejects without `admin_note` -> the system requires a note for `REJECTED` and `NEEDS_EDIT`.

## Feature: Request Edits on Property Listing
* **Acceptance Criteria:**
    * Admin sets status = `NEEDS_EDIT` with mandatory `admin_note`.
    * The user is notified of required changes.
    * The user can edit and resubmit the listing.
    * Upon resubmission, status returns to `PENDING` and a new review cycle begins.
    * `reviewed_by` and `reviewed_at` are updated with each admin action.
* **Edge Cases:**
    * User ignores `NEEDS_EDIT` for a long period -> the listing is automatically deleted.

## Feature: Reject Property Listing
* **Acceptance Criteria:**
    * Admin sets status = `REJECTED` with mandatory `admin_note`.
    * `Property.status` is set to `REJECTED`.
    * The listing is not visible to other users.
    * The user is notified of the rejection and its reason.
* **Edge Cases:**
    * Admin rejects without `admin_note` -> the system requires adding a note before confirming rejection.

## Feature: Grant Property Verification Mark
* **Acceptance Criteria:**
    * Admin sets status = `APPROVED` and `Property.is_verified` = `TRUE` only when documents are uploaded.
    * If no documents exist, the admin may approve but `is_verified` remains `FALSE`.
    * `verified_by` is set to the admin’s `User.id`.
    * `verified_at` is recorded at the time of verification.
* **Edge Cases:**
    * Admin attempts to set `is_verified` = `TRUE` without uploaded documents -> the system prevents the action or displays a confirmation warning.

# 3. API Endpoints (Login & Roles)

## 1. Create Agent Badge
* **Endpoint:** POST /agent-badges
* **Description:** Creates a "Professional Agent" badge and assigns it to a specific agent.
* **Headers:** Host: Shaqqa.com, Accept: application/json
* **Request Body:**
```json
{
  "agent_id": "12",
  "badge_name": "Professional Agent",
  "granted_at_score": 82.5
}
```
* **Response (201 Created):**
```json
{
  "id": "1",
  "agent_id": "12",
  "granted_by": "845",
  "badge_name": "Professional Agent",
  "status": "active",
  "granted_at": "2026-04-29T10:00:00Z",
  "granted_at_score": 82.5
}
```

## 2. Revoke Agent Badge
* **Endpoint:** PUT /agent-badges/{id}
* **Description:** Revokes an active "Professional Agent" badge.
* **Path Parameters:**
    * `id`: The unique identifier of the agent badge.
* **Headers:** Host: Shaqqa.com, Accept: application/json
* **Request Body:**
```json
{
  "status": "revoked",
  "revoked_at_score": 58.0
}
```
* **Response (200 OK):**
```json
{
  "id": "1",
  "agent_id": "12",
  "granted_by": "845",
  "revoked_by": "154",
  "badge_name": "Professional Agent",
  "status": "revoked",
  "granted_at": "2026-04-20T09:00:00Z",
  "revoked_at": "2026-04-29T12:00:00Z",
  "granted_at_score": 82.5,
  "revoked_at_score": 58.0
}
```

# 4. Database Schema (Entities & Attributes)

## 1. Table: `RealEstateCompany`
* **`id`** (PK): Unique identifier for the real estate company.
* **`name`** (VARCHAR): Name of the real estate company.
* **`license_number`** (VARCHAR, UNIQUE): Unique license number of the company.
* **`logo_url`** (VARCHAR): URL path for the company logo.
* **`verified`** (BOOLEAN): `TRUE` if the company is verified.
* **`created_at`** (TIMESTAMP): Timestamp indicating when the record was created.

## 2. Table: `User`
* **`id`** (PK): Unique identifier for the user.
* **`name`** (VARCHAR): Full name of the user.
* **`email`** (VARCHAR, UNIQUE): Unique email address of the user.
* **`phone`** (VARCHAR): Contact phone number.
* **`password_hash`** (VARCHAR): Encrypted password hash for authentication.
* **`role`** (ENUM): The role of the user (`REGULAR`, `AGENT`, `COMPANY_ADMIN`, `ADMIN`).
* **`company_id`** (FK -> `RealEstateCompany.id`, NULLABLE): Foreign key referencing the user's affiliated real estate company.
* **`created_at`** (TIMESTAMP): Timestamp indicating when the user was created.

## 3. Table: `VerificationRequest`
* **`id`** (PK): Unique identifier for the verification request.
* **`property_id`** (FK -> `Property.id`): Foreign key referencing the associated property.
* **`reviewed_by`** (FK -> `User.id`, NULLABLE): Foreign key referencing the admin who reviewed the request.
* **`status`** (ENUM): The current status of the request (`PENDING`, `APPROVED`, `NEEDS_EDIT`, `REJECTED`).
* **`documents_url`** (TEXT[]): Array of URLs pointing to uploaded supporting documents.
* **`admin_note`** (TEXT, NULLABLE): Internal note left by the reviewing admin.
* **`created_at`** (TIMESTAMP): Timestamp indicating when the request was created.
* **`reviewed_at`** (TIMESTAMP): Timestamp indicating when the request was reviewed.

## 4. Table: `TrustMetric`
* **`id`** (PK): Unique identifier for the trust metric record.
* **`agent_id`** (FK -> `User.id`): Foreign key referencing the agent.
* **`has_photo_verified_by_admin`** (BOOLEAN): `TRUE` if the admin has verified the agent's photo.
* **`professional_posts_ratio`** (DECIMAL(3,2)): The ratio calculated for professional posts.
* **`posts_this_month`** (INT): The total number of posts created by the agent this month.
* **`active_days_last_30`** (INT): Number of days the agent was active in the last 30 days.
* **`posts_prof_score`** (DECIMAL(5,2)): The calculated score derived from professional posts.
* **`monthly_posts_score`** (DECIMAL(5,2)): The calculated score derived from monthly posts.
* **`activity_score`** (DECIMAL(5,2)): The calculated overall activity score.
* **`trust_score`** (DECIMAL(5,2)): The final calculated overall trust score.
* **`calculated_at`** (TIMESTAMP): Timestamp indicating when the metrics were last calculated.

## 5. Table: `AgentBadge`
* **`id`** (PK): Unique identifier for the agent badge record.
* **`agent_id`** (FK -> `User.id`): Foreign key referencing the agent receiving the badge.
* **`granted_by`** (FK -> `User.id`): Foreign key referencing the admin who granted the badge.
* **`revoked_by`** (FK -> `User.id`, NULLABLE): Foreign key referencing the admin who revoked the badge.
* **`badge_name`** (VARCHAR, DEFAULT 'Professional Agent'): The name identifier of the badge.
* **`status`** (ENUM): The current status of the agent's badge (`ACTIVE`, `REVOKED`).
* **`granted_at`** (TIMESTAMP): Timestamp indicating when the badge was granted.
* **`revoked_at`** (TIMESTAMP, NULLABLE): Timestamp indicating when the badge was revoked.
* **`granted_at_score`** (DECIMAL(5,2)): The trust score evaluated at the time the badge was granted.
* **`revoked_at_score`** (DECIMAL(5,2)): The trust score evaluated at the time the badge was revoked.