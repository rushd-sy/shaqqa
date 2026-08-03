# 1. User Stories

## Customer
- As a user (Customer - property owner), **I want** to add a property with its property details and verified ownership documents **so that** I can publish verifiable advertisement on the program to potential customers.
- As a user (Customer - property owner), **I want** to update my existing property advertisement **so that** I can correct details, prices, or change contact information.
- As a user (Customer - property owner), **I want** to delete my property advertisement **so that** it is no longer visible once the property is sold or unavailable.
- As a user (Customer), **I want** to view a list of published property advertisements **so that** I can discover properties easily and find a property that suits my needs.

## Agent (Broker)
- As a user (Broker), **I want** to publish properties without ownership documents, **so that** I can publish all properties advertisements that available from my real estate office.
- As a user (Broker), **I want** to update and delete my published property advertisements **so that** I can maintain an accurate portfolio of available properties.

## Shaqqa Admin and Staff
- As a Shaqqa admin OR Shaqqa Staff, **I want** to monitor, edit, or delete user advertisements, **so that** I can maintain content quality and remove spam or violations.

## Company

- Company Admin is the only user that can **NOT** add properties.

### Company Admin
- As a company owner, I want to view, edit, and delete **advertisements** created by any of my staff, so that I can ensure all company listings meet our quality standards.

### Company Staff
- As a company staff, I want to see company staff tools (common with broker) like add, update and delete property and my advertisements, so that I can manage my properties in one place.

---

# 2. Acceptance Criteria & Edge Cases

## Feature: Add Property Advertisement
* **Acceptance Criteria:**
    * The user (property owner, `BROKER`, or `COMPANY_STAFF`) can publish the advertisement, and all interested `CUSTOMER`s can see the property.
* **Edge Cases:**
    * Public infrastructure (such as hospitals, schools, mosques or government sites) as a selected location should be ignored.
    * Outside Syria borders as the selected location should be ignored.
    * The user does not possess a document confirming ownership of the property, if anyone can add a property, then the program will face issues with scammers.
    * Fake property prices, like low prices to attract more people on the program, or high prices to show it as valuable.
    * Add images for another building or location.
    * Fake phone numbers as contact information.
    * Fake property details.

## Feature: Update Property Advertisement
* **Acceptance Criteria:**
    * The author of the advertisement (`CUSTOMER`, `BROKER`, or `COMPANY_STAFF`), their managing `COMPANY_ADMIN`, or authorized `SHAQQA_ADMIN`/`SHAQQA_STAFF` can successfully update property details and images.
* **Edge Cases:**
    * A user attempts to update an advertisement that belongs to another user.
    * Changing the location to an invalid area (e.g., outside Syria borders or public infrastructure).
    * Submitting fake property details or phone numbers during the update.
    * `CUSTOMER` or `BROKER` update advertisements after an update from `COMPANY_ADMIN`, `SHAQQA_ADMIN`/`SHAQQA_STAFF`.

## Feature: Delete Property Advertisement
* **Acceptance Criteria:**
    * The author of the advertisement, their managing `COMPANY_ADMIN`, or authorized program staff (`SHAQQA_ADMIN`/`SHAQQA_STAFF`) can delete or remove the advertisement from public listings.
* **Edge Cases:**
    * Attempting to delete an advertisement that has already been deleted or does not exist.
    * A user attempts to delete an advertisement owned by someone else.

## Feature: List Property Advertisements
* **Acceptance Criteria:**
    * All active users can retrieve and view a list of available advertisements where `is_available` is `TRUE`.
    * Advertisements belonging to a deactivated user (`is_active` is `FALSE`) or a user associated with a deactivated company (`is_active` is `FALSE` in the `Company` table) must be automatically hidden and excluded from public listings.
* **Edge Cases:**
    * Retrieving page numbers that exceed the total number of available pages.
    * Returning an empty list if no advertisements are currently published or match the search parameters.

---

# 3. API Endpoints (Advertisements)

## 1. Add Advertisement
* **Endpoint:** POST `/api/advertisements`
* **Description:** Add a new property advertisement with property details, contact information, and required attachments.
* **Headers:** `Authorization: Bearer <User_Token>`
* **Request Body:**
```json
{
  "property_details": {
    "description": "string",
    "price": 150000000,
    "contract_type": "SALE",
    // all other elements from Saleh
  },
  "contact_information": "+963xxxxxxxxx",
  "attached_files": {
    "images": [
      "file_url_or_base64_string"
    ],
    "ownership_document": "file_url_or_base64_string"
  }
}
```
* **Response (201 Created):**
```json
{
  "id_advertisement": "uuid-or-int",
  "message": "Property added successfully. Redirecting to newly created advertisement page."
}
```
* **Error Responses:**
    * `400 Bad Request`: Syntax, validation, or edge case problem (e.g., location outside Syria).
    * `401 Unauthorized`: Missing or invalid token.
    * `403 Forbidden`: User account is deactivated or lacks permissions to post (`COMPANY_ADMIN`).

## 2. Update Advertisement
* **Endpoint:** PUT `/api/advertisements/{id_advertisement}`
* **Description:** Updates an existing advertisement's details, contact info, or attachments.
* **Path Parameters:**
    * `id_advertisement` (UUID/INT, required): The unique identifier of the advertisement.
* **Headers:** `Authorization: Bearer <User_Token>`
* **Request Body:**
```json
{
  "property_details": {
    "description": "string",
    "price": 160000000,
    // all other elements from PropertyDetails.md
  },
  "contact_information": "+963xxxxxxxxx"
}
```
* **Response (200 OK):**
```json
{
  "id_advertisement": "uuid-or-int",
  "message": "Advertisement updated successfully."
}
```
* **Error Responses:**
    * `400 Bad Request`: Invalid payload or edge case violation.
    * `401 Unauthorized`: Missing or invalid token.
    * `403 Forbidden`: User is not the owner of the advertisement, their managing `COMPANY_ADMIN`, or authorized program staff, or user account is deactivated.
    * `404 Not Found`: Advertisement does not exist.

## 3. Delete Advertisement
* **Endpoint:** DELETE `/api/advertisements/{id_advertisement}`
* **Description:** Deletes or deactivates a specific advertisement.
* **Path Parameters:**
    * `id_advertisement` (UUID/INT, required): The unique identifier of the advertisement.
* **Headers:** `Authorization: Bearer <User_Token>`
* **Response (200 OK):**
```json
{
  "message": "Advertisement deleted successfully."
}
```
* **Error Responses:**
    * `401 Unauthorized`: Missing or invalid token.
    * `403 Forbidden`: User is not the owner of the advertisement, their managing `COMPANY_ADMIN`, or authorized program staff, or user account is deactivated.
    * `404 Not Found`: Advertisement does not exist.

## 4. List Advertisements
* **Endpoint:** GET `/api/advertisements`
* **Description:** Retrieves a list of available property advertisements for the discovery feed.
* **Query Parameters:**
    * `page` (integer, optional): The page index to retrieve (e.g., `?page=1`). Default is `1`.
    * `limit` (integer, optional): The number of advertisements to return per page (e.g., `?limit=10`). Default is `10`.
* **Headers:** `Authorization: Bearer <User_Token>` (Optional for public viewing)
* **Response (200 OK):**
```json
{
  "data": [
    {
      "id_advertisement": "uuid-or-int",
      "id_user": "uuid-or-int",
      "publish_date": "2026-06-10T10:00:00Z",
      "contact_info": "+963xxxxxxxxx",
      "property_details": {
        "description": "string",
        "price": 150000000
      }
    }
  ],
  "pagination": {
    "current_page": 1,
    "total_pages": 10
  }
}
```
* **Error Responses:**
    * `400 Bad Request`: Invalid filtering or pagination query parameters.

---

# 4. Database Schema (Entities & Attributes)

## 1. Table: `Advertisement`
* **`id_advertisement`** (PK, UUID/INT): Unique identifier for the advertisement.
* **`id_user`** (FK -> `User.id_user`): Unique identifier of the user who owns/created the advertisement.
* **`id_property`** (FK -> `Property.id_property`): Unique identifier of the associated property.
* **`publish_date`** (DATETIME/TIMESTAMP): The date when the advertisement was published.
* **`title`** (VARCHAR): The advertisement's title (e.g., "Apartments for Sale in Aleppo New"), used by free-text search.
* **`contract_type`** (ENUM): `RENT`, `SALE`. Whether the property is offered for rent or sale in this advertisement.
* **`contact_info`** (VARCHAR): Contact information listed for the advertisement.
* **`is_available`** (BOOLEAN): Status indicating whether the advertisement is active and visible (`TRUE` or `FALSE`).

## 2. Table: `Property`
* **`id_property`** (PK, UUID/INT): Unique identifier for the property.
* **`property_details`** (TEXT/JSON): Property details (by Saleh).