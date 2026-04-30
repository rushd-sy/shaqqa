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
    *   `404 Not Found`: Phone number not registered.
    *   `429 Too Many Requests`: User requested OTP too many times in a short period.

## 2. Verify OTP (Final Login)
*   **Endpoint:** `POST /api/auth/verify-otp`
*   **Description:** Verifies the OTP and returns the session token.
*   **Request Body:**
    ```json
    {
      "phone": "+1234567890",
      "otp_code": "123456"
    }
    ```
*   **Response (200 OK):**
    ```json
    {
      "token": "eyJhbGciOiJIUzI1...",
      "user": {
        "id_user": 105,
        "first_name": "John",
        "role": "COMPANY_STAFF",
        "id_company": 12
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
    }
    ```

## 5. Company Admin Adds Staff
*   **Endpoint:** `POST /api/companies/staff`
*   **Description:** Registers a new staff member. (No password required; user will log in via OTP).
*   **Headers:** `Authorization: Bearer <Company_Admin_Token>`
*   **Request Body:**
    ```json
    {
      "phone": "+1987654321",
      "first_name": "Jane",
      "last_name": "Doe"
    }
    ```
*   **Response (201 Created):**
    ```json
    {
      "message": "Staff account created. They can now log in using their phone number.",
      "user_id": 208
    }
    ```
*   **Error Responses:**
    *   `409 Conflict`: This phone number is already registered to another user.
    *   `403 Forbidden`: Only Company Admins can perform this action.