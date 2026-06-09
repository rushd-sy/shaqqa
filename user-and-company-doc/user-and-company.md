# 1. User Stories

![alt text](image.png)

## **Authentication (All Users):**
- As a user (any role), I want to log in using my registered phone number and OTP, so that I can securely access the platform based on my assigned role.
- As a user, I want my session to recognize my exact role upon login so that the frontend can route me to my specific dashboard automatically.

## Customer
- As a customer, I want to login on the system then see customer tabs like discover advertisments and brokers, search, history, favorite, add property, my advertisements and customer settings, so that I can discover properties easily and deal with intrested features only.
- As a customer, I want to submit a broker application form and attach my verification documents, so that I can prove my qualifications and initiate the process of upgrading my role to a Broker.

## Agent (Broker)
Normal customer become broker after approvement by program admin OR they added manually after normal contact on real life.
- As a broker, I want to login on the system then see broker tabs like add property, my advertisements, statistics, reports, badges, my profile, and broker settings.

## Company
- Program admins can add company after special contact between company and program owner.
- Company can manage its employees and have special managements tools, profits statistics, and more, so that help company to have one tool to control everything.
- the big different between company and normal broker is that company have profits statistics.
- Company can stop any feature (like profit statistics) or any feature, reflect changes to all company staff, so that protect and respect company privacy, and enable the features that company desires.

### Company Admin
- As a company owner, I want to have special credintials and login like any other user (using phone number), so that facilitate login process to me.
- As a company owner, I want to see company admin tabs like manage company staff (CRUD), manage advertisemnts (CRUD), statistics (advertisements - staff - profits), reports, badges, and admin settings, so that I have full access to the company from one place.
- As a company owner, I want to track the profits from statistics, so that reduce external accounting software usage (like "الأمين"), or at least, reduce accounting efforts.
- As a Company Admin, I want to create new user accounts assigned the `COMPANY_STAFF` role and automatically linked to my `id_company`, so that my employees can access our company features.

### Company Staff
- As a company staff, I want to have company staff credintials, login on the system easily like any other user (using phone number), and start work with the company as employee, so that I start my job easily with the company on one place.
- As a company staff, I want to see company staff tabs (common with broker) like add property, my advertisements, statistics (advertisements - profits), reports, badges, my profile, and company staff settings.

## Shaqqa Admin and Staff

### Shaqqa Admin
- As an admin, I want to have special credintials, login like any other user (using phone number), so that facilitate login process to me.
- As an Shaqqa admin, I want to manage everything on the app, so that I keep the app clean, bring more customers, and add companies.
- As an Shaqqa admin, I want manage companies, staff, brokers, advertisements, reports, and badges, so that help program owners to control program content and serve customers.
- As an Shaqqa admin, I want to see statistics about every section in the app, so that help program owners to take future decisions.
- As a Shaqqa admin, I want to create a new Company profile and simultaneously assign an initial user as the `COMPANY_ADMIN`, so they can start managing their company.
- As a Shaqqa admin, I want to be able to disable (`is_active = false`) a user or an entire company, so that I can block login access for policy violations.
- As a Shaqqa admin, I want to create new user accounts assigned the `SHAQQA_STAFF` role, so that my staff members can assist me in managing reports, advertisements, and broker requests.

### Shaqqa Staff
- As a staff member, I want to have special credintials, login like any other user (using phone number), so that facilitate login process to me, and I start my job.
- As a staff member, I want to manage reports, advertisements, and statistics about my work, so that I make program clean from spam and irrelevant advertisments.
- As a staff member, I want to view a list of pending broker requests and approve or reject them, so that I can control who becomes a registered broker.

# 2. Acceptance Criteria & Edge Cases

## Feature: User Login
*   **Acceptance Criteria:**
    *   Given a valid phone number and a valid OTP code, the system returns a JWT token containing `id_user`, `role`, and `id_company` (if applicable).
    *   Given a non-existent phone number, the system returns a 404 Not Found or generic 401 Unauthorized.
    *   If `is_active` is FALSE for the user (or their parent company), the login fails with an account suspended error.
*   **Edge Cases:**
    *   A user is promoted to a new role while they have an active session (JWT needs refresh handling or the frontend must force re-login on 403 errors).

## **Feature: Request Broker Status**
*   **Acceptance Criteria:**
    *   A user with role `CUSTOMER` can successfully create a PENDING `Broker_Request`.
    *   When an Admin changes the request status to `APPROVED`, the user's role in the `User` table updates to `BROKER`.
*   **Edge Cases:**
    *   A user submits a request while they already have a `PENDING` request (System should block and return 409 Conflict).
    *   A user who is a `COMPANY_STAFF` tries to apply as an independent `BROKER` (System should block; roles are mutually exclusive).

## **Feature: Adding Company Staff**
*   **Acceptance Criteria:**
    *   A `COMPANY_ADMIN` can submit staff details. The system creates a new user, sets role to `COMPANY_STAFF`, and assigns the admin's `id_company`.
*   **Edge Cases:**
    *   The phone number provided for the new staff member is already registered as a `CUSTOMER` in the app. (System should either reject with "Phone number already exists" or offer a flow to invite/migrate the existing user to the company).

## **Feature: Adding Shaqqa Staff**
*   **Acceptance Criteria:**
    *   A `SHAQQA_ADMIN` can submit staff details. The system creates a new user and sets their role to `SHAQQA_STAFF`.
*   **Edge Cases:**
    *   The phone number provided for the new Shaqqa staff member is already registered in the system (e.g., as a `CUSTOMER` or independent `BROKER`). The system should allow upgrading or migrating their role to `SHAQQA_STAFF` after confirmation, or block the operation if they are currently linked to an active company as a `COMPANY_ADMIN` or `COMPANY_STAFF`.

---

# 3. API Endpoints (Login & Roles)

## 1. Request Login OTP
*   **Endpoint:** `POST /api/auth/request-otp`
*   **Description:** Initiates the login process by sending a 6-digit code to the user's phone.
*   **Request Body:**
    ```json
    {
      "phone": "+1234567890"
    }
    ```
*   **Response (200 OK):**
    ```json
    {
      "message": "OTP sent successfully.",
      "expires_in": "300" // seconds
    }
    ```
*   **Error Responses:**
    *   `429 Too Many Requests`: User requested OTP too many times in a short period.
    *   `403 Forbidden`: This phone number belongs to an account that has been explicitly deactivated.

## 2. Verify OTP (Final Login)
*   **Endpoint:** `POST /api/auth/verify-otp`
*   **Description:** Verifies the OTP and returns the session token. If the phone number does not exist in the database, a new user record is automatically created with the role `CUSTOMER`.
*   **Request Body:**
    ```json
    {
      "phone": "+1234567890",
      "otp_code": "123456"
    }
    ```
*   **Response (200 OK):**
    *   Option A: Existing User (e.g., Company Staff)
    ```json
    {
      "token": "eyJhbGciOiJIUzI1...",
      "user": {
        "id_user": 105,
        "first_name": "John",
            "Last_name": "Doe",
        "role": "COMPANY_STAFF",
        "id_company": 12
      }
    }
    ```
    *   Option B: New User (Registration)
    ```json
    {
        "token": "eyJhbGciOiJIUzI1...",
        "is_new_user": true,
        "user": {
            "id_user": 106,
            "first_name": null,
            "Last_name": null,
            "role": "CUSTOMER",
            "id_company": null
        }
    }
    ```
*   **Error Responses:**
    *   `401 Unauthorized`: Invalid or expired OTP.
    *   `403 Forbidden`: User or Company account is deactivated (`is_active: false`).

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

## 4. Review Broker Request (Admin/Staff)
*   **Endpoint:** `PUT /api/admin/broker-requests/{id_request}`
*   **Description:** **Shaqqa Admin OR Shaqqa Staff** reviews and updates request status.
*   **Path Parameters:**
        *   `id_request` (integer, required): The unique ID of the broker request being reviewed (corresponds to the `request_id` returned in the creation step).
*   **Headers:** `Authorization: Bearer <Shaqqa_Admin_or_Staff_Token>`
*   **Request Body:**
    ```json
    {
      "status": "APPROVED", // or "REJECTED"
      "review_notes": "Documents verified."
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

## 5. Check Phone Number Registration
*   **Endpoint:** `POST /api/companies/staff/check`
*   **Description:** Used by the Admin to check if a phone number already exists in the system before adding them as staff. This determines whether the frontend needs to collect the user's name.
*   **Headers:** `Authorization: Bearer <Company_Admin_Token>`
*   **Request Body:**
    ```json
    {
      "phone": "+1234567890"
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
    *   `403 Forbidden`: Only Company Admins are authorized to check phone numbers.

## 6. Company Admin Adds Staff
*   **Endpoint:** `POST /api/companies/staff`
*   **Description:** Finalizes adding the staff member to the company based on the results of the check endpoint.
*   **Headers:** `Authorization: Bearer <Company_Admin_Token>`
*   **Request Body:**
    *   **Payload if `exists` was `false` (Names required):**
    ```json
    {
      "phone": "+1234567890",
      "first_name": "John",
      "last_name": "Doe"
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
      "message": "Staff member successfully processed and linked to the company.",
      "user_id": 208
    }
    ```
*   **Error Responses:**
    *   `400 Bad Request`: Missing `first_name` or `last_name` for a phone number that does not exist.
    *   `401 Unauthorized`: Invalid or expired token.
    *   `403 Forbidden`: Only Company Admins can perform this action.
    *   `409 Conflict`: This phone number is already registered as staff for a company.

## 7. Check Phone Number Registration (Shaqqa Admin)
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

## 8. Shaqqa Admin Adds Staff
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

# 4. Database Schema (Entities & Attributes)

## 1. Table: `Company`
Extended to handle basic status for login/access control.
*   **`id_company`** (PK, UUID/INT): Unique identifier for the company.
*   **`name`** (VARCHAR): Main company contact name.
*   **`phone_number`** (VARCHAR, Unique): Main company contact phone.
*   **`id_location`** (FK -> `Location.id_location`, Nullable): Corporate office location reference.
*   **`is_active`** (BOOLEAN): Default `TRUE`. If `FALSE`, all company staff under this company are prevented from logging in.
*   **`created_at`** (TIMESTAMP): Creation date.

## 2. Table: `User`
Extended for authentication, standardizing roles, and linking to companies.
*   **`id_user`** (PK, UUID/INT): Unique identifier for the user.
*   **`id_company`** (FK -> `Company.id_company`, Nullable): Populated only for users with the role `COMPANY_ADMIN` and `COMPANY_STAFF`.
*   **`username`** (VARCHAR, Unique): Unique identifier name (optional).
*   **`phone`** (VARCHAR, Unique): Primary login identifier (with country code).
*   **`first_name`** (VARCHAR): User's first name.
*   **`last_name`** (VARCHAR): User's last name.
*   **`role`** (ENUM): User role on the system. Values: `CUSTOMER`, `BROKER`, `COMPANY_ADMIN`, `COMPANY_STAFF`, `SHAQQA_ADMIN`, `SHAQQA_STAFF`.
*   **`is_active`** (BOOLEAN): Default `TRUE`. Used by admins to ban or suspend users.
*   **`created_at`** (TIMESTAMP): User registration timestamp.

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