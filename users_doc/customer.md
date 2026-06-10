# 1. User Stories

## Customer
- As a customer, I want to login on the system then see customer tabs like discover advertisments and brokers, search, history, favorite, add property, my advertisements and customer settings, so that I can discover properties easily and deal with intrested features only.
- As a customer, I want to submit a broker application form and attach my verification documents, so that I can prove my qualifications and initiate the process of upgrading my role to a Broker.

# 2. Acceptance Criteria & Edge Cases

## **Feature: Request Broker Status**
*   **Acceptance Criteria:**
    *   A user with role `CUSTOMER` can successfully create a PENDING `Broker_Request`.
    *   When an Admin changes the request status to `APPROVED`, the user's role in the `User` table updates to `BROKER`.
*   **Edge Cases:**
    *   A user submits a request while they already have a `PENDING` request (System should block and return 409 Conflict).
    *   A user who is a `COMPANY_STAFF` tries to apply as an independent `BROKER` (System should block; roles are mutually exclusive).

# 3. API Endpoints (Login & Roles)

## 3. Apply to become a Broker
*   **Endpoint:** `POST /api/users/broker-requests`
*   **Description:** Submits a request with a required document (identity/license).
*   **Headers:** `Authorization: Bearer <Customer_Token>`
*   **Request Body:**
    ```json
    {
      "request_notes": "I am a licensed broker in Cairo.",
      "document_id": "uuid-or-int-of-uploaded-file" 
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
    *   `400 Bad Request`: `document_id` is missing or invalid.
    *   `409 Conflict`: User already has a pending request.

# 4. Database Schema (Entities & Attributes)

## 3. Table: `Broker_Request`
Handles the workflow of a `CUSTOMER` applying to become a `BROKER`.
*   **`id_request`** (PK, UUID/INT): Unique identifier for the broker application.
*   **`id_user`** (FK -> `User.id_user`): The customer submitting the request.
*   **`status`** (ENUM): The current state of the request. Values: `PENDING`, `APPROVED`, `REJECTED`.
*   **`reviewed_by`** (FK -> `User.id_user`, Nullable): The Shaqqa Admin or Staff member who handled the request.
*   **`request_notes`** (TEXT, Nullable): Form details filled out by the customer.
*   **`document_id`** (FK -> `Document.id_document`): Required identification/license document reference to verify the broker.
*   **`created_at`** (TIMESTAMP): Request creation date.
*   **`updated_at`** (TIMESTAMP): Timestamp of the last status change.