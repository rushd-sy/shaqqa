# 1. User Stories

## User
- As a user, I want to select a specific reason for my report from a predefined list (e.g., Fake Listing, Spam, Inappropriate Content), so that my report is clear and categorized.
- As a user, I want to optionally add a text description to my report, so that I can provide more context or proof about the violation.
- As a user, I want to receive a success message confirming that my report has been submitted, so that I know the system successfully received my request.

## Admin
- As an admin, I want to see a list of all submitted reports sorted by `created_at`, so I can review the most recent issues first.
- As an admin, I want to update the status of a report (from `PENDING` to `RESOLVED` or `REJECTED`), so that I can track which issues have been handled.

# 2. Acceptance Criteria & Edge Cases

## Feature: Submit a Property Report
* **Acceptance Criteria:**
    * Given a user is viewing an existing property, when the user submits a report with a valid reason from the predefined list, then the system should store the report with a `PENDING` status and return a 201 Created response with a success message.
* **Edge Cases:**
    * Given a user is attempting to report a property, when the user submits the report without providing a reason, then the system should reject the request and return a 400 Bad Request response with a validation error message.
    * Given a user has already reported a specific property, and the previous report's status is still `PENDING` or `UNDER_REVIEW`, when the user attempts to submit another report for the same property, then the system should prevent the duplication and return a 409 Conflict response.
    * Given a user is attempting to report a property, when the user submits a report for an `advertisement_id` that does not exist in the database, then the system should reject the request and return a 404 Not Found response.
    * Given the user is not logged in, when the user reports an ad, then the system will reject the report and return a 401 Unauthorized response.
    * Given an authenticated user with the role of `ADMIN` or `BROKER`, when they attempt to submit a report for an ad, then the system should reject the request and return a 403 Forbidden status code.

# 3. API Endpoints (Login & Roles)

## 1. Submit a Property Report
* **Endpoint:** POST /api/v1/reports
* **Description:** Allows a user to submit a report for a specific property advertisement with a predefined reason and an optional description.
* **Headers:** `Authorization: Bearer <User_Token>`
* **Request Body:**
```json
{
  "user_id": "3a1c9e57-1a2b-4c3d-8e9f-000000000001",
  "advertisement_id": "b7f0c8a2-3c4d-5e6f-7081-000000000003",
  "reason": "fake_listing",
  "description": "The images used in this property listing are fake and taken from another website."
}
```
* **Response (201 Created):**
```json
{
  "message": "Report submitted successfully. Thank you for keeping our platform safe.",
  "report_id": "2c3d4e5f-8192-a3b4-c5d6-000000000008",
  "status": "PENDING"
}
```
* **Error Responses:**
    * `404 Not Found`: If the `advertisement_id` does not exist in the database.
    * `409 Conflict`: If the user has already reported this specific property.
    * `400 Bad Request`: If the sent data is incomplete (e.g., missing reason).
    * `401 Unauthorized`: If the user reports without logging in.
    * `403 Forbidden`: If an `ADMIN` or `BROKER` attempts to submit a report.

# 4. Database Schema (Entities & Attributes)

> **ID strategy:** `Report` exposes `PublicId` (UUID v7, indexed, UNIQUE) as `report_id` in the response. The internal `Id` (INT, PK) is never exposed. `User` and `Advertisement` are referenced by their public identifiers.

## 1. Table: `User`
*(Referenced by `UserId` → `User.PublicId`. Full definition in `users_doc/all-users.md`.)*

## 2. Table: `Advertisement`
*(Referenced by `AdvertisementId` → `Advertisement.PublicId`. Full definition in `PropertyListing.md`.)*

## 3. Table: `Report`
* **`Id`** (PK, INT, IDENTITY): Internal identifier for the report — **never exposed**.
* **`PublicId`** (UUID v7, UNIQUE, INDEXED): Public identifier for the report; exposed as `report_id` in the response and used as the FK target.
* **`UserId`** (FK -> `User.PublicId`, UUID, COMPOSITE UNIQUE): Public identifier of the user who submitted the report.
* **`AdvertisementId`** (FK -> `Advertisement.PublicId`, UUID, COMPOSITE UNIQUE): Public identifier of the reported property advertisement.
* **`Reason`** (ENUM): The predefined reason for the report. Values include `FAKE_LISTING`, `SPAM`, `INAPPROPRIATE`, `OTHER`.
* **`Description`** (TEXT, NULLABLE): An optional text description providing more context or proof about the violation.
* **`Status`** (ENUM): The current handling status of the report. Values include `PENDING`, `UNDER_REVIEW`, `RESOLVED`, `REJECTED`.
* **`CreatedAt`** (TIMESTAMP): The date and time when the report was successfully submitted.
