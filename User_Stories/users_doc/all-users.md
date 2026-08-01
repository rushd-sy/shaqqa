# 1. User Stories

![alt text](image.png)

## **Authentication (All Users):**
- As a new user, I want to register with my phone number and verify it via an OTP sent by SMS, so that I can create my account as a `CUSTOMER`.
- As a new user, after verifying my phone, I want to provide my first and last name, so that I can complete my registration and get logged in automatically.
- As a user, I want my role to be recognized upon login so that the frontend can route me to my specific dashboard automatically.
- As a user (any role), I want to log in using my registered phone number and an OTP, so that I can securely access the platform based on my assigned role.
- As a user, I want my logged-in session to remain active for a long time (via automatic refresh token rotation), so that let me login once and improve my user experience.
- As a user, I want to logout from the app, so that let me switch to another account, or make a new profile.

> **Note:** Registration always creates a `CUSTOMER` account. No other role can be assigned during registration — only a Shaqqa Admin can assign roles to users.

# 2. Acceptance Criteria & Edge Cases

## Feature: User Registration (New Users)
*   **Acceptance Criteria:**
    *   Given a new phone number, the system creates a user record with role `CUSTOMER`, `is_verified = 0`, and `is_active = 1`, sends a 6-digit OTP via SMS, and returns "OTP Sent".
    *   Given a valid OTP, the system marks the phone number as `is_verified = 1`.
    *   Given the user's first and last name, the system completes the registration and auto-logs the user in by returning an `accessToken`, `refreshToken`, and `expiresInSeconds`; the frontend routes them to the advertisements page.
    *   Given an already-registered phone number, registration fails with `409 Conflict` and the user is directed to the login flow instead.
    *   The role is always `CUSTOMER`; the registration flow can never assign `BROKER`, `COMPANY_ADMIN`, `COMPANY_STAFF`, `SHAQQA_ADMIN`, or `SHAQQA_STAFF`.
*   **Edge Cases:**
    *   OTP expires before verification (system rejects it and offers a resend).
    *   User requests OTP too many times in a short period (rate-limited with `429 Too Many Requests`).
    *   User verifies the OTP but abandons the name step; the account stays `CUSTOMER` with `is_verified = 1` but missing name, and cannot login until the name step is completed.
    *   User already logged-in tries to register again with a verified phone (system directs them to the login flow).

## Feature: User Login (Returning Users)
*   **Acceptance Criteria:**
    *   Given a registered phone number and a valid OTP code, the system detects the user's role and returns a JWT access token containing `id_user`, `role`, `first_name`, `last_name`, and `id_company` (for `COMPANY_ADMIN`/`COMPANY_STAFF`), plus a refresh token.
    *   Given a non-existent phone number, the system returns 404 Not Found or generic 401 Unauthorized — no account is auto-created (registration is a separate flow).
    *   If `is_active` is FALSE for the user (or their parent company), the login fails with an account suspended error.
    *   OTPs are single-use: they are "burned" (deleted) immediately after successful verification to prevent replay attacks.
    *   Session tokens (access + refresh) are saved locally on mobile app or web browser.
*   **Edge Cases:**
    *   A user is promoted to a new role while they have an active session (JWT needs refresh handling or the frontend must force re-login on 403 errors).
    *   A user attempts to login with a phone number that is not registered (system directs them to the registration flow).
    *   A user attempts to login with a verified account that never completed the name step (system redirects them to complete their profile).
    *   A refresh token is reused after rotation (the old token is already invalidated/revoked).
    *   OTP expiry and wrong-code attempts (system limits attempts before requiring a new OTP).

# 3. API Endpoints (Login & Roles)

## 1. Register
*   **Endpoint:** `POST /api/auth/register`
*   **Description:** New user registration. Checks the phone is not registered, creates a user record with role `CUSTOMER` and `is_verified = 0`, and sends a 6-digit OTP to the user's phone.
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
    *   `400 Bad Request`: Missing `phone`.
    *   `409 Conflict`: Phone number is already registered — use the login flow.
    *   `429 Too Many Requests`: OTP requested too many times in a short period.

## 2. Verify Registration
*   **Endpoint:** `POST /api/auth/verify-registration`
*   **Description:** Verifies the registration OTP and marks the phone number as `is_verified = 1`.
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
      "message": "Phone verified. Please complete your profile."
    }
    ```
*   **Error Responses:**
    *   `401 Unauthorized`: Invalid, expired, or already-used OTP.

## 3. Complete Registration (Auto-Login)
*   **Endpoint:** `POST /api/auth/complete-registration`
*   **Description:** Called after phone verification; saves the user's first and last name and auto-logs them in by issuing a JWT access token and refresh token. The frontend then routes the user to the advertisements page.
*   **Request Body:**
    ```json
    {
      "phone": "+1234567890",
      "first_name": "John",
      "last_name": "Doe"
    }
    ```
*   **Response (200 OK):**
    ```json
    {
      "accessToken": "eyJhbGciOiJIUzI1...",
      "refreshToken": "eyJhbGciOiJIUzI1...",
      "expiresInSeconds": 900,
      "user": {
        "id_user": 106,
        "first_name": "John",
        "last_name": "Doe",
        "role": "CUSTOMER",
        "id_company": null
      }
    }
    ```
*   **Error Responses:**
    *   `400 Bad Request`: Missing `first_name` or `last_name`.
    *   `401 Unauthorized`: Phone number is not verified (`is_verified = 0`).

## 4. Request Login OTP (Returning User)
*   **Endpoint:** `POST /api/auth/login-request`
*   **Description:** Initiates login for an existing user by generating and saving a login OTP, then sending it via SMS. No new account is created.
*   **Request Body:**
    ```json
    {
      "phone": "+1234567890"
    }
    ```
*   **Response (200 OK):**
    ```json
    {
      "message": "Login OTP sent.",
      "expires_in": "300" // seconds
    }
    ```
*   **Error Responses:**
    *   `404 Not Found`: Phone number is not registered — use the registration flow.
    *   `429 Too Many Requests`: User requested OTP too many times in a short period.
    *   `403 Forbidden`: This phone number belongs to an account that has been explicitly deactivated.

## 5. Verify Login OTP (Final Login)
*   **Endpoint:** `POST /api/auth/login-verify`
*   **Description:** Verifies the login OTP (burning it to prevent reuse), detects the user's role, and returns the session tokens with role-based JWT claims.
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
      "accessToken": "eyJhbGciOiJIUzI1...",
      "refreshToken": "eyJhbGciOiJIUzI1...",
      "expiresInSeconds": 900,
      "user": {
        "id_user": 105,
        "first_name": "John",
        "last_name": "Doe",
        "role": "COMPANY_STAFF",
        "id_company": 12
      }
    }
    ```
*   **Error Responses:**
    *   `401 Unauthorized`: Invalid or expired OTP.
    *   `403 Forbidden`: User or Company account is deactivated (`is_active: false`).

## 6. Refresh Tokens (Session Lifecycle)
*   **Endpoint:** `POST /api/auth/refresh`
*   **Description:** Called when the access token expires (e.g., after 15 minutes). Verifies the refresh token is valid and not revoked, then rotates it: a new access token and a new refresh token are issued, and the old refresh token is invalidated in the database.
*   **Request Body:**
    ```json
    {
      "refreshToken": "eyJhbGciOiJIUzI1..."
    }
    ```
*   **Response (200 OK):**
    ```json
    {
      "accessToken": "eyJhbGciOiJIUzI1...",
      "refreshToken": "eyJhbGciOiJIUzI1...",
      "expiresInSeconds": 900
    }
    ```
*   **Error Responses:**
    *   `401 Unauthorized`: Invalid, expired, or revoked refresh token.

## 7. Logout
*   **Endpoint:** `POST /api/auth/logout`
*   **Description:** Revokes/deletes the active refresh token from the database. The client clears all tokens from local storage.
*   **Request Body:**
    ```json
    {
      "refreshToken": "eyJhbGciOiJIUzI1..."
    }
    ```
*   **Response (200 OK):**
    ```json
    {
      "message": "Successfully logged out."
    }
    ```
*   **Error Responses:**
    *   `401 Unauthorized`: Invalid or already-revoked refresh token.

# 4. JWT Token Claims

*   **All users:** `id_user`, `role`, `first_name`, `last_name`.
*   **`COMPANY_ADMIN` / `COMPANY_STAFF` only (additional claims):** `id_company` (and any other company-related details described in their user stories).

# 5. Database Schema (Entities & Attributes)

## 1. Table: `User`
Extended for authentication, standardizing roles, and linking to companies.
*   **`id_user`** (PK, UUID/INT): Unique identifier for the user.
*   **`id_company`** (FK -> `Company.id_company`, Nullable): Populated only for users with the role `COMPANY_ADMIN` and `COMPANY_STAFF`.
*   **`username`** (VARCHAR, Unique): Unique identifier name (optional).
*   **`phone`** (VARCHAR, Unique): Primary login identifier (with country code).
*   **`first_name`** (VARCHAR, Nullable): User's first name. Set during the registration name step.
*   **`last_name`** (VARCHAR, Nullable): User's last name. Set during the registration name step.
*   **`role`** (ENUM): User role on the system, always `CUSTOMER` at registration and only changeable by a Shaqqa Admin. Values: `CUSTOMER`, `BROKER`, `COMPANY_ADMIN`, `COMPANY_STAFF`, `SHAQQA_ADMIN`, `SHAQQA_STAFF`.
*   **`is_active`** (BOOLEAN): Default `TRUE`. Used by admins to ban or suspend users.
*   **`is_verified`** (BOOLEAN): Default `FALSE`. Set to `TRUE` after the user verifies their phone number via registration OTP.
*   **`created_at`** (TIMESTAMP): User registration timestamp.

## 2. Table: `Otp`
Handles single-use OTP codes for both registration and login flows.
Note: Can be replaced with Firebase.
*   **`id_otp`** (PK, UUID/INT): Unique identifier for the OTP record.
*   **`phone`** (VARCHAR): The phone number the OTP was sent to.
*   **`code`** (VARCHAR): The hashed 6-digit OTP code.
*   **`purpose`** (ENUM): Flow the OTP belongs to. Values: `REGISTRATION`, `LOGIN`.
*   **`attempts`** (INT): Default `0`. Incremented on every wrong guess. Max allowed is 3 before invalidation.
*   **`created_at`** (TIMESTAMP): OTP creation timestamp.
*   **`expires_at`** (TIMESTAMP): Expiration time of the OTP.


## 3. Table: `Refresh_Token`
Supports session persistence and refresh token rotation.
*   **`id_token`** (PK, UUID/INT): Unique identifier for the refresh token.
*   **`id_user`** (FK -> `User.id_user`): The user the token belongs to.
*   **`token`** (VARCHAR): The hashed refresh token value.
*   **`expires_at`** (TIMESTAMP): Expiration time of the refresh token.
*   **`is_revoked`** (BOOLEAN): Set to `TRUE` on rotation or logout; old tokens are invalidated when a new pair is issued.
*   **`created_at`** (TIMESTAMP): Token creation timestamp.
