# 1. User Stories

## Shaqqa Admin and Staff

### Shaqqa Admin
- As an admin, I want to have special credintials, login like any other user (using phone number), so that facilitate login process to me.
- As a Shaqqa admin, I want to add, update, and suspend **companies**, so that I can control which businesses operate on the platform.
- As a Shaqqa admin, I want to manage **Shaqqa staff** accounts (add, edit, remove), so that I can delegate administrative tasks effectively.
- As a Shaqqa admin, I want to view, approve, reject, or suspend **broker** accounts, so that I ensure only qualified individuals act as brokers.
- As a Shaqqa admin, I want to monitor, edit, or delete user **advertisements**, so that I can maintain content quality and remove spam or violations.
- As a Shaqqa admin, I want to review and take action on user **reports** (against properties or users), so that I can maintain a safe and trustworthy environment.
- As a Shaqqa admin, I want to create, assign, and manage **badges** for top-performing brokers and companies, so that I can encourage high-quality services.
- As an Shaqqa admin, I want to see statistics about every section in the app, so that help program owners to take future decisions.
- As a Shaqqa admin, I want to create a new Company profile and simultaneously assign an initial user as the `COMPANY_ADMIN`, so they can start managing their company.
- As a Shaqqa admin, I want to be able to disable (`is_active = false`) a user or an entire company, so that I can block login access for policy violations.
- As a Shaqqa admin, I want to create new user accounts assigned the `SHAQQA_STAFF` role, so that my staff members can assist me in managing reports, advertisements, and broker requests.

### Shaqqa Staff
- As a staff member, I want to have special credintials, login like any other user (using phone number), so that facilitate login process to me, and I start my job.
- As a staff member, I want to manage reports, advertisements, and statistics about my work, so that I make program clean from spam and irrelevant advertisments.
- As a staff member, I want to view a list of pending broker requests and approve or reject them, so that I can control who becomes a registered broker.

# 2. Acceptance Criteria & Edge Cases

## **Feature: Adding Shaqqa Staff**
*   **Acceptance Criteria:**
    *   A `SHAQQA_ADMIN` can submit staff details. The system creates a new user and sets their role to `SHAQQA_STAFF`.
*   **Edge Cases:**
    *   The phone number provided for the new Shaqqa staff member is already registered in the system (e.g., as a `CUSTOMER` or independent `BROKER`). The system should allow upgrading or migrating their role to `SHAQQA_STAFF` after confirmation, or block the operation if they are currently linked to an active company as a `COMPANY_ADMIN` or `COMPANY_STAFF`.

# 3. API Endpoints (Login & Roles)

## 1. Review Broker Request (Admin/Staff)
*   **Endpoint:** `PUT /api/admin/broker-requests/{id_request}`
*   **Description:** **Shaqqa Admin OR Shaqqa Staff** reviews and updates request status.
*   **Path Parameters:**
        *   `id_request` (integer, required): The unique ID of the broker request being reviewed (corresponds to the `request_id` returned in the creation step).
*   **Headers:** `Authorization: Bearer <Shaqqa_Admin_or_Staff_Token>`
*   **Request Body:**
    ```json
    {
      "status": "APPROVED", // or "REJECTED"
      "review_notes": "Application details verified."
    }
    ```
*   **Response (200 OK):**
    ```json
    {
      "message": "Status updated. User role is now BROKER."
      "data": {
        "request_id": 45,
        "user_id": 101,
        "status": "APPROVED",
        "reviewed_by": 7, 
        "updated_at": "2026-06-09T11:00:00Z"
      }
    }
    ```
*   **Error Responses:**
    *    `400 Bad Request`: Invalid status value provided.
    *    `401 Unauthorized`: Missing or invalid token.
    *    `403 Forbidden`: User is not a Shaqqa Admin or Staff.
    *    `404 Not Found`: No broker request found with the provided `id_request`.

## 2. Check Phone Number Registration (Shaqqa Admin)
*   **Endpoint:** `POST /api/admin/staff/check`
*   **Description:** Used by a Shaqqa Admin to check if a phone number already exists in the system before registering them as Shaqqa Staff. This determines whether the frontend needs to collect the user's name details.
*   **Headers:** `Authorization: Bearer <Shaqqa_Admin_Token>`
*   **Request Body:**
    ```json
    {
      "phone": "+123456789"
    }
    ```
*   **Response (200 OK):**
    *   **If the user already exists in the system:**
    ```json
    {
      "exists": true,
      "first_name": "John",
      "last_name": "Doe",
      "message": "User exists. First and last name are not required."
    }
    ```
    *   **If the user does not exist:**
    ```json
    {
      "exists": false,
      "message": "User not found."
    }
    ```
*   **Error Responses:**
    *   `401 Unauthorized`: Invalid or expired token.
    *   `403 Forbidden`: Only Shaqqa Admins are authorized to perform this check.

## 3. Shaqqa Admin Adds Staff
*   **Endpoint:** `POST /api/admin/staff`
*   **Description:** Finalizes registering and setting up a Shaqqa Staff member.
*   **Headers:** `Authorization: Bearer <Shaqqa_Admin_Token>`
*   **Request Body:**
    *   **Payload if `exists` was `false` (Names required):**
    ```json
    {
      "phone": "+1234567890",
      "first_name": "John",
      "last_name": "Doe",
    }
    ```
    *   **Payload if `exists` was `true` (Names omitted):**
    ```json
    {
      "phone": "+1234567890"
    }
    ```
*   **Response (201 Created):**
    ```json
    {
      "message": "Shaqqa Staff member successfully registered and role updated to Shaqqa Staff.",
      "user_id": 305
    }
    ```
*   **Error Responses:**
    *   `400 Bad Request`: Missing `first_name` or `last_name` for a phone number that does not exist.
    *   `401 Unauthorized`: Invalid or expired token.
    *   `403 Forbidden`: Only Shaqqa Admins can perform this action.
    *   `409 Conflict`: The target user is already linked to a company (e.g., as `COMPANY_ADMIN` or `COMPANY_STAFF`), which prevents role migration without manual deletion or manual separation from the company first.