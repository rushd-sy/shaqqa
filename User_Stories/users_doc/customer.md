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
      "id_location": 205,
      "request_notes": "I worked as a licensed broker in Aleppo for 3 years."
    }
    ```
*   **Response (201 Created):**
    ```json
    {
      "message": "Broker request submitted for review.",
      "request_id": 45
    }
    ```
*   **Error Responses:**
    *   `400 Bad Request`: `id_location` does not exist in the `Location` table or is invalid.
    *   `409 Conflict`: User already has a pending request.

# 4. Database Schema (Entities & Attributes)

## 3. Table: `Broker_Request`
Handles the workflow of a `CUSTOMER` applying to become a `BROKER`.
*   **`id_request`** (PK, UUID/INT): Unique identifier for the broker application.
*   **`id_user`** (FK -> `User.id_user`): The customer submitting the request.
*   **`prior_experience`** (BOOLEAN): Checkbox indicating whether the applicant previously worked as a broker. Default `FALSE`.
*   **`id_location`** (FK -> `Location.id_location`, Nullable): Optional office location picked on the map; references the `Location` table (see property user stories).
*   **`status`** (ENUM): The current state of the request. Values: `PENDING`, `APPROVED`, `REJECTED`.
*   **`reviewed_by`** (FK -> `User.id_user`, Nullable): The Shaqqa Admin or Staff member who handled the request.
*   **`request_notes`** (TEXT, Nullable): Optional free-text notes filled out by the customer.
*   **`created_at`** (TIMESTAMP): Request creation date.
*   **`updated_at`** (TIMESTAMP): Timestamp of the last status change.
