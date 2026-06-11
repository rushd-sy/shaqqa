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
    * Given a user is attempting to report a property, when the user submits a report for a propertyId that does not exist in the database, then the system should reject the request and return a 404 Not Found response.
    * Given the user is not logged in, when the user reports an ad, then the system will reject the report and return a 401 Unauthorized response.
    * Given an authenticated user with the role of `ADMIN` or `BROKER`, when they attempt to submit a report for an ad, then the system should reject the request and return a 403 Forbidden status code.

# 3. API Endpoints (Login & Roles)

## 1. Submit a Property Report
* **Endpoint:** POST /api/reports
* **Description:** Allows a user to submit a report for a specific property listing with a predefined reason and an optional description.
* **Headers:** `Content-Type: application/json`
* **Request Body:**
```json
{
  "user_id": 105,
  "advertisement_id": 25,
  "reason": "fake_listing",
  "description": "The images used in this property listing are fake and taken from another website."
}
```
* **Response (201 Created):**
```json
{
  "message": "Report submitted successfully. Thank you for keeping our platform safe.",
  "report_id": 512,
  "status": "pending"
}
```
* **Error Responses:**
    * `404 Not Found`: If the `propertyId` does not exist in the database.
    * `409 Conflict`: If the user has already reported this specific property.
    * `400 Bad Request`: If the sent data is incomplete (e.g., missing reason).
    * `401 Unauthorized`: If the user reports without logging in.
    * `403 Forbidden`: If an `ADMIN` or `BROKER` attempts to submit a report.

# 4. Database Schema (Entities & Attributes)

## 1. Table: `User`
*(No specific attributes provided in the requirements)*

## 2. Table: `Property`
*(No specific attributes provided in the requirements)*

## 3. Table: `Report`
* **`report_id`** (PK): The unique identifier for the report.
* **`user_id`** (FK, COMPOSITE UNIQUE): The identifier of the user who submitted the report.
* **`advertisement_id`** (FK, COMPOSITE UNIQUE): The identifier of the reported property advertisement.
* **`reason`** (ENUM): The predefined reason for the report. Values include `FAKE_LISTING`, `SPAM`, `INAPPROPRIATE`, `OTHER`.
* **`description`** (TEXT, NULLABLE): An optional text description providing more context or proof about the violation.
* **`status`** (ENUM): The current handling status of the report. Values include `PENDING`, `UNDER_REVIEW`, `RESOLVED`, `REJECTED`.
* **`created_at`** (TIMESTAMP): The date and time when the report was successfully submitted.