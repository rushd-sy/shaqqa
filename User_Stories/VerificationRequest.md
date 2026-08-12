# 1. User Stories

## Customer
- As a Customer (property owner), **I want** to submit my property advertisement for verification **so that** Shaqqa Admin/Staff can review it before it becomes public.
- As a Customer, **I want** to receive the review decision (approved / needs edits / rejected) with the reviewer's notes **so that** I know what to fix and resubmit.

## Broker
- As a Broker, **I want** my verification requests to be marked as high priority **so that** my advertisements are fast-tracked and published quickly.
- As a Broker, **I want** to submit my property advertisement for verification **so that** I can publish all properties available from my real estate office.

## Company Staff
- As a Company Staff, **I want** my verification requests to be treated like a Broker's (`HIGH` priority) **so that** my company advertisements are published quickly.

## Shaqqa Admin and Staff
- As a Shaqqa Admin OR Shaqqa Staff, **I want** to review verification requests and approve, request edits, or reject them **so that** only valid advertisements become public (full stories in `users_doc/shaqqa-admin-staff.md`).

---

# 2. Acceptance Criteria & Edge Cases

## Feature: Submit for Verification (PUBLISH / UPDATE)
* **Acceptance Criteria:**
    * Verification applies **only** to publishing (`PUBLISH`) and updating (`UPDATE`) advertisements.
    * **Deletion of advertisements and media never requires verification** — those actions are direct.
    * Creating an advertisement immediately creates it with status `PENDING` and automatically creates a `PUBLISH` verification request (one transaction).
    * `request_type`:
        * `PUBLISH`: the advertisement was created and (auto-submitted to verification request) for the first time.
        * `UPDATE`: replacing an `ACTIVE` advertisement with a new version.
    * At most **one `PENDING` verification request per advertisement version** at any time.
    * `requester_role` is derived at creation from users table (point to user role): `CUSTOMER`, `BROKER`, or `COMPANY_STAFF`.
    * `priority` is derived from `requester_role`:
        * `HIGH`: `BROKER` and `COMPANY_STAFF` (fast-tracked review).
        * `NORMAL`: `CUSTOMER`.
    * `PUBLISH` requires at least 1 image on the advertisement.
    * An `UPDATE` request is tied to a **new `Advertisement` row** (a new version) that carries the staged changes; the current `ACTIVE` row is **not** modified until approval.
* **State Mapping (deterministic — no undefined states):**

| VerificationRequest.status | Advertisement.status (`PUBLISH`) | Advertisement.status (`UPDATE`) |
|---|---|---|
| `PENDING` | `PENDING` | new version `PENDING`; superseded version unchanged (`ACTIVE` — stays live) |
| `APPROVED` | `ACTIVE` | new version `ACTIVE`; superseded version set to `DELETED` |
| `REJECTED` | `REJECTED` | new version `REJECTED`; superseded version unchanged (`ACTIVE` — stays live) |
| `NEEDS_EDIT` | `DRAFT` (author notified, then edits/resubmits) | new version `DRAFT` (author notified); superseded version unchanged (`ACTIVE` — stays live) |

    * `DRAFT` is used **only** as the outcome of a `NEEDS_EDIT` review (or admin/staff edits after review). It is **never** created by the user — users create advertisements directly as `PENDING`.
    * The author is **notified** whenever their advertisement becomes `DRAFT`; they can edit and resubmit to renew the verification cycle.
    * An advertisement can become `ACTIVE` **only** through an `APPROVED` verification request.
    * When an `UPDATE` request is `APPROVED`, the superseded `ACTIVE` version is set to `DELETED` and the new version becomes `ACTIVE` (replacement — see `PropertyListing.md`).
* **Edge Cases:**
    * Creation with missing required fields or no image: `400 Bad Request` — no advertisement, `VerificationRequest`, or status change is created.
    * A second submission while a request is already `PENDING` for the same advertisement version: `409 Conflict`.
    * Updating a `PENDING` advertisement is allowed: the existing `PENDING` request is **hard-deleted** and a new one is created (replacement — no `409`).
    * The author does not respond to `NEEDS_EDIT` within 7 days: the request is set to `REJECTED` (and the `DRAFT` advertisement to `REJECTED`).

## Feature: Review Outcome (Admin side)
* **Acceptance Criteria:**
    * `APPROVED`: the advertisement version becomes `ACTIVE`; for `UPDATE`, the superseded version becomes `DELETED` and `publish_date` is refreshed on the new version.
    * `NEEDS_EDIT`: the advertisement version becomes `DRAFT` and the author receives a notification.
    * `REJECTED` and `NEEDS_EDIT` require a mandatory `admin_note`.
    * `reviewed_by` and `reviewed_at` are recorded on every review action.
* **Edge Cases:**
    * Admin attempts to reject/request edits without a note: blocked with `400 Bad Request`.
    * Two admins review the same request simultaneously: the first action wins; the second admin sees the updated status.

---

# 3. API Endpoints (Verification)

## 1. Create Advertisement (Auto-Submits a PUBLISH Request)
achived through add advertisement endpoint in `PropertyListing.md`.

## 2. Resubmit Advertisement after NEEDS_EDIT
achived through update advertisement endpoint in `PropertyListing.md`.

## 3. Get My Verification Requests
* **Endpoint:** `GET /api/v1/verification-requests`
* **Description:** Returns the authenticated user's verification requests with their current status.
* **Headers:** `Authorization: Bearer <User_Token>`
* **Response (200 OK):**
```json
{
  "data": [
    {
      "id_verification_request": "9a31d4e6-...-uuid",
      "id_advertisement": "b7f0c8a2-...-uuid",
      "request_type": "PUBLISH",
      "priority": "NORMAL",
      "status": "PENDING",
      "admin_note": null,
      "created_at": "2026-08-10T09:00:00Z",
      "reviewed_at": null
    }
  ]
}
```
* **Error Responses:**
    * `401 Unauthorized`: Missing or invalid token.

> Admin/Staff review endpoints (list `PENDING` requests ordered oldest-first, and review them) are documented in `users_doc/shaqqa-admin-staff.md`.

---

# 4. Database Schema (Entities & Attributes)

## 1. Table: `VerificationRequest`
* **`id_verification_request`** (PK, UUID): Unique identifier for the verification request.
* **`id_advertisement`** (FK -> `Advertisement.id_advertisement`, UUID): The advertisement **version** being verified.
* **`id_user`** (FK -> `User.id_user`, UUID): The user who submitted the request.
* **`request_type`** (ENUM): `PUBLISH`, `UPDATE`. Whether the request verifies a new publication or a replacement version of an `ACTIVE` advertisement.
* **`requester_role`** (ENUM): `CUSTOMER`, `BROKER`, `COMPANY_STAFF`. **Derived** from User table.
* **`priority`** (ENUM): `HIGH`, `NORMAL`. Derived from `requester_role` (`HIGH` for `BROKER`/`COMPANY_STAFF`, `NORMAL` for `CUSTOMER`).
* **`status`** (ENUM): `PENDING`, `APPROVED`, `NEEDS_EDIT`, `REJECTED`.
* **`reviewed_by`** (FK -> `User.id_user`, NULLABLE): The Shaqqa Admin/Staff member who reviewed the request.
* **`admin_note`** (TEXT, NULLABLE): The reviewer's note (mandatory for `REJECTED` and `NEEDS_EDIT`).
* **`created_at`** (TIMESTAMP): Request creation time.
* **`reviewed_at`** (TIMESTAMP, NULLABLE): Time of the last review action.

> The staged data of an `UPDATE` request lives in the new `Advertisement` row (see `PropertyListing.md`) — there is no `proposed_changes` JSON column. When a `PENDING` advertisement is updated, its previous `PENDING` verification request is **hard-deleted** (no `SUPERSEDED` status).
