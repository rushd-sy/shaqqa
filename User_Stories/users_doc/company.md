# 1. User Stories

## Company
- Program admins can add company after special contact between company and program owner.
- Company can manage its employees and have special managements tools, profits statistics, and more, so that help company to have one tool to control everything.
- the big different between company and normal broker is that company have profits statistics.
- Company can stop any feature (like profit statistics) or any feature, reflect changes to all company staff, so that protect and respect company privacy, and enable the features that company desires.

### Company Admin
- As a company owner, I want to have special credintials and login like any other user (using phone number), so that facilitate login process to me.
- As a company owner, I want to add, update, and remove **company staff** members, so that I can control exactly who represents my company on the app.
- As a company owner, I want to view, edit, and delete **advertisements** created by any of my staff, so that I can ensure all company listings meet our quality standards.
- As a company owner, I want to view detailed **statistics** covering our active advertisements, staff performance, and overall profits, so that I can make data-driven business decisions.
- As a company owner, I want to monitor **reports** and feedback related to my company and staff, so that I can address customer concerns and improve service quality.
- As a company owner, I want to view the **badges** earned by my company and staff, so that I can track our reputation and credibility on the platform.
- As a company owner, I want to access **admin settings** to toggle specific features (like profit tracking) on or off, so that I can customize the platform to fit my company's privacy and operational needs.
- As a company owner, I want to track the profits from statistics, so that reduce external accounting software usage (like "الأمين"), or at least, reduce accounting efforts.
- As a Company Admin, I want to create new user accounts assigned the `COMPANY_STAFF` role and automatically linked to my `company_id`, so that my employees can access our company features.

### Company Staff
- As a company staff, I want to have company staff credintials, login on the system easily like any other user (using phone number), and start work with the company as employee, so that I start my job easily with the company on one place.
- As a company staff, I want to see company staff tabs (common with broker) like add property, my advertisements, statistics (advertisements - profits), reports, badges, my profile, and company staff settings.

# 2. Acceptance Criteria & Edge Cases

## **Feature: Adding Company Staff**
*   **Acceptance Criteria:**
    *   A `COMPANY_ADMIN` can submit staff details. The system creates a new user, sets role to `COMPANY_STAFF`, and assigns the admin's `company_id` (the `Company.PublicId`).
*   **Edge Cases:**
    *   The phone number provided for the new staff member is already registered as a `CUSTOMER` in the app. (System should either reject with "Phone number already exists" or offer a flow to invite/migrate the existing user to the company).

# 3. API Endpoints (Login & Roles)

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
      "user_id": "3a1c9e57-1a2b-4c3d-8e9f-000000000001"
    }
    ```
*   **Error Responses:**
    *   `400 Bad Request`: Missing `first_name` or `last_name` for a phone number that does not exist.
    *   `401 Unauthorized`: Invalid or expired token.
    *   `403 Forbidden`: Only Company Admins can perform this action.
    *   `409 Conflict`: This phone number is already registered as staff for a company.

# 4. Database Schema (Entities & Attributes)

> **ID strategy:** `Company` exposes `PublicId` (UUID v7, indexed, UNIQUE) as `company_id` in JWT claims / responses and as the FK target. The internal `Id` (INT, PK) is never exposed. `Location` is referenced but not defined in these documents; its public identifier (`Location.PublicId`) is used as the FK target for `IdLocation`.

## 1. Table: `Company`
Extended to handle basic status for login/access control.
*   **`Id`** (PK, INT, IDENTITY): Internal identifier for the company — **never exposed**.
*   **`PublicId`** (UUID v7, UNIQUE, INDEXED): Public identifier for the company; exposed as `company_id` in JWT claims / responses and used as the FK target.
*   **`Name`** (VARCHAR): Main company contact name.
*   **`PhoneNumber`** (VARCHAR, Unique): Main company contact phone.
*   **`IdLocation`** (FK -> `Location.PublicId`, UUID, Nullable): Public identifier of the corporate office location reference.
*   **`IsActive`** (BOOLEAN): Default `TRUE`. If `FALSE`, all company staff under this company are prevented from logging in.
*   **`CreatedAt`** (TIMESTAMP): Creation date.
