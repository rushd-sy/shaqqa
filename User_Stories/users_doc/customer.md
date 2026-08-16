# 1. User Stories

## Customer
- As a customer, I want to login on the system then see customer tabs like discover advertisments and brokers, search, history, favorite, add property, my advertisements and customer settings, so that I can discover properties easily and deal with intrested features only.
- As a customer, I want to submit a broker application form with my basic details, so that I can provide my qualifications and initiate the process of upgrading my role to a Broker.

# 2. Acceptance Criteria & Edge Cases

## **Feature: Request Broker Status**
*   **Acceptance Criteria:**
    *   A user with role `CUSTOMER` can successfully create a PENDING `Broker_Request` by filling a form with basic details: whether they worked as a broker before (checkbox), an optional office location (picked on a map), and optional notes.
    *   When an Admin changes the request status to `APPROVED`, the user's role in the `User` table updates to `BROKER`.
*   **Edge Cases:**
    *   A user submits a request while they already have a `PENDING` request (System should block and return 409 Conflict).
    *   A user who is a `COMPANY_STAFF` tries to apply as an independent `BROKER` (System should block; roles are mutually exclusive).
    *   Pick public locations like schools on map as a office.
    *   The "prior experience" checkbox defaults to `FALSE` when not provided.

# 3. API Endpoints (Login & Roles)

## 3. Apply to become a Broker
*   **Endpoint:** `POST /api/users/broker-requests`
*   **Description:** Submits a broker application form with basic details: whether the applicant worked as a broker before (checkbox), an optional office location (picked on a map, derived from the `Location` table as documented in the property user stories), and optional notes.
*   **Headers:** `Authorization: Bearer <Customer_Token>`
*   **Request Body:**
    ```json
    {
      "prior_experience": true,
      "id_location": "c5d6e7f8-09a1-2b3c-4d5e-000000000012",
      "request_notes": "I worked as a licensed broker in Aleppo for 3 years."
    }
    ```
*   **Response (201 Created):**
    ```json
    {
      "message": "Broker request submitted for review.",
      "request_id": "6f708193-c5d6-e7f8-0a1b-00000000000c"
    }
    ```
*   **Error Responses:**
    *   `400 Bad Request`: `id_location` does not exist in the `Location` table or is invalid.
    *   `409 Conflict`: User already has a pending request.

# 4. Database Schema (Entities & Attributes)

> **ID strategy:** `Broker_Request` exposes `PublicId` (UUID v7, indexed, UNIQUE) as `request_id` in the response / admin endpoints. The internal `Id` (INT, PK) is never exposed. `Location` is referenced but not defined in these documents; its public identifier (`Location.PublicId`) is the FK target for `IdLocation`.

## 3. Table: `Broker_Request`
Handles the workflow of a `CUSTOMER` applying to become a `BROKER`.
*   **`Id`** (PK, INT, IDENTITY): Internal identifier for the broker application — **never exposed**.
*   **`PublicId`** (UUID v7, UNIQUE, INDEXED): Public identifier for the broker application; exposed as `request_id` in the response / admin endpoints.
*   **`UserId`** (FK -> `User.PublicId`, UUID): Public identifier of the customer submitting the request.
*   **`PriorExperience`** (BOOLEAN): Checkbox indicating whether the applicant previously worked as a broker. Default `FALSE`.
*   **`IdLocation`** (FK -> `Location.PublicId`, UUID, Nullable): Public identifier of the optional office location picked on the map; references the `Location` table (see property user stories).
*   **`Status`** (ENUM): The current state of the request. Values: `PENDING`, `APPROVED`, `REJECTED`.
*   **`ReviewedBy`** (FK -> `User.PublicId`, UUID, Nullable): Public identifier of the Shaqqa Admin or Staff member who handled the request.
*   **`RequestNotes`** (TEXT, Nullable): Optional free-text notes filled out by the customer.
*   **`CreatedAt`** (TIMESTAMP): Request creation date.
*   **`UpdatedAt`** (TIMESTAMP): Timestamp of the last status change.
