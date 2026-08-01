# 1. User Stories

## Buyer
- As a buyer, I want to view detailed technical specifications (area, rooms, bathrooms, floor, etc.) so that I can evaluate if the property fits my needs.
- As a buyer, I want to see high-quality images of the property so that I can visually assess its condition.
- As a buyer, I want to view videos or virtual tours so that I can better understand the layout and space.
- As a buyer, I want to read a clear property description so that I can understand its features and advantages.
- As a buyer, I want to know additional details (furnishing status, age of property, amenities) so that I can make an informed decision.

## Seller
- As a seller, I want to add technical specifications (area, rooms, bathrooms, etc.) so that buyers can understand the property details.
- As a seller, I want to upload multiple images for my property so that I can showcase it effectively.
- As a seller, I want to upload videos or virtual tours so that I can attract more buyers.
- As a seller, I want to write and edit a property description so that I can highlight key selling points.
- As a seller, I want to update or delete media (images/videos) so that I can keep my listing accurate.
- As a seller, I want to save my progress as a draft so I can finish adding technical specs later.

## Admin
- As an Admin, I want to flag/hide media that violates community guidelines (e.g., blurred images, contact info in photos) to maintain platform quality.


# 2. Acceptance Criteria & Edge Cases

## Feature: Technical Specs
* **Acceptance Criteria:**
  * Location is returned in API response in valid format (lat, long & structured address). Example: `location`: { `latitude`: number between -90 & 90, `longitude`: number between -180 & 180, `address`: "Aleppo, Syria" }.
  * Area is returned in API response with explicit unit (value > 0 and unit must be valid enum `AreaUnit` `{SqM, Hectare, ...}`).
  * Price is returned in API response in valid format (price > 0).
  * Floor number is returned in API response in valid format (if property is an apartment).
  * Legal status is returned in API response in valid format (enum `LegalStatus` `{Freehold, SharedOwnership, ...}`).
  * Number of rooms is returned in API response in valid format (rooms > 0) (if property is an apartment or villa).
  * Building construction date is returned in API response in valid format (date <= DateTime.Now) (if property is not land).
* **Edge Cases:**
  * Location is missing: API returns `400 Bad Request` `{ "message": "location is required" }`.
  * Location must include valid latitude (-90 to 90) and longitude (-180 to 180): API returns `400 Bad Request` `{ "message": "Invalid location format" }`.
  * Invalid location format: API returns `400 Bad Request` `{ "Invalid location format " }`.
  * Area is missing & area <= 0: API returns `400 Bad Request` `{ "message": "area is required and should be greater than zero" }`.
  * Price is missing & Price <= 0: API returns `400 Bad Request` `{ "price is required and should be greater than zero" }`.
  * Number of rooms is missing for (apartment/villa) & rooms <= 0: API returns `400 Bad Request` `{ "number of rooms is required and should be greater than zero" }`.
  * Legal Status is missing or invalid enum value: API returns `400 Bad Request` `{ "message": "legal status is required and must be a valid enum value" }`.
  * Construction date is missing or invalid date time: API returns `400 Bad Request` `{ "message": "construction date is required and must be a valid date less than or equal to the current date" }`.
  * Invalid construction date (future date): API returns `400 Bad Request` `{ "Construction date cannot be in the future" }`.
  * Property exists but is inactive/deleted: API returns `404 Not Found`.

## Feature: Images and Media
* **Acceptance Criteria:**
  * At least 1 image is required for an active listing.
  * Media supports images and videos (enum `MediaType` `{ Image, Video }`).
  * One media item must be marked as cover (`IsCover` = `TRUE`) and is returned first in the response.
  * Remaining media items are sorted by `DisplayOrder` in ascending order (If not specified, the first image is used as default cover).
  * Max media count defined (Max = 50).
  * Images are compressed upon upload (target size: < 500KB).
  * URL video should be a valid YouTube URL.
* **Edge Cases:**
  * No property images available: API returns `400 Bad Request` `{ "At least one image is required" }`.
  * Large number of images (images > 50): API returns `400 Bad Request` `{ "message": "Maximum number of media files is 50" }`.
  * Invalid images URLs: API returns `400 Bad Request` `{ "Invalid media URL format" }`.

## Feature: Property Description
* **Acceptance Criteria:**
  * Property description is not empty and is displayed.
  * Property description includes roof condition information (if an apartment or villa).
* **Edge Cases:**
  * Property description is missing.


# 3. API Endpoints (Login & Roles)

## 1. Create Property
* **Endpoint:** POST /Properties
* **Description:** Creates a new property listing.
* **Request Body:**
```json
{
  "price": 100000,
  "area": { "value": 120, "unit": "SqM" },
  "location": { "latitude": 36.2021, "longitude": 37.1343, "address": "Aleppo, Syria" },
  "description": "Property description here"
}
```
* **Response (201 Created):**
```json
{
  "message": "Property created successfully"
}
```
* **Error Responses:**
  * `400 Bad Request`: Missing required fields (price, area, location, description), invalid values (price <= 0, area <= 0), or invalid location format.

## 2. Get All Properties
* **Endpoint:** GET /Properties
* **Description:** Retrieves a list of properties.
* **Request Body:**
```json
{}
```
* **Response (200 OK):**
```json
[
  {
    "property_id": 1,
    "price": 100000,
    "description": "Property description here"
  }
]
```
* **Error Responses:**
  * `400 Bad Request`: Invalid request parameters.

## 3. Get Property Details
* **Endpoint:** GET /Properties/{id}
* **Description:** Returns full property details (technical specs, media, description).
* **Path Parameters:**
  * `id`: Property ID
* **Request Body:**
```json
{}
```
* **Response (200 OK):**
```json
{
  "property_id": 1,
  "price": 100000,
  "area": { "value": 120, "unit": "SqM" },
  "location": { "latitude": 36.2021, "longitude": 37.1343, "address": "Aleppo, Syria" },
  "description": "Property description here",
  "media": []
}
```
* **Error Responses:**
  * `400 Bad Request`: Invalid property id (e.g., negative or zero).
  * `404 Not Found`: Property does not exist or is inactive/deleted.

## 4. Update Property
* **Endpoint:** PATCH /Properties/{id}
* **Description:** Updates existing property details.
* **Path Parameters:**
  * `id`: Property ID
* **Request Body:**
```json
{
  "price": 110000
}
```
* **Response (200 OK):**
```json
{
  "message": "Property updated successfully"
}
```
* **Error Responses:**
  * `400 Bad Request`: Invalid values (price <= 0, invalid location, etc.) or Invalid enum values (PropertyType, LegalStatus).
  * `404 Not Found`: Property does not exist.

## 5. Delete Property
* **Endpoint:** DELETE /Properties/{id}
* **Description:** Deletes a property listing.
* **Path Parameters:**
  * `id`: Property ID
* **Request Body:**
```json
{}
```
* **Response (204 No Content):**
```json
{}
```
* **Error Responses:**
  * `404 Not Found`: Property does not exist.

## 6. Add Property Media
* **Endpoint:** POST /Properties/{id}/Media
* **Description:** Adds media to a specific property.
* **Path Parameters:**
  * `id`: Property ID
* **Request Body:**
```json
{
  "type": "Image",
  "url": "http://example.com/image.jpg",
  "is_cover": true
}
```
* **Response (201 Created):**
```json
{
  "message": "Media added successfully"
}
```
* **Error Responses:**
  * `400 Bad Request`: Invalid media URL, unsupported media type, media count exceeds limit (max 50), or no image provided for active property.
  * `404 Not Found`: Property does not exist.

## 7. Delete Property Media
* **Endpoint:** DELETE /Properties/{id}/Media/{mediaId}
* **Description:** Deletes a specific media item from a property.
* **Path Parameters:**
  * `id`: Property ID
  * `mediaId`: Media ID
* **Request Body:**
```json
{}
```
* **Response (204 No Content):**
```json
{}
```
* **Error Responses:**
  * `404 Not Found`: Property does not exist or media does not exist.


# 4. Database Schema (Entities & Attributes)

## 1. Table: `Property`
* **`property_id`** (PK, INT): Property identifier.
* **`property_type`** (ENUM): `APARTMENT`, `LAND`, `SHOP`, `VILLA`, `OFFICE`.
* **`description`** (STRING): Description of the property.
* **`price`** (DECIMAL): Price of the property.
* **`area_value`** (DECIMAL): Area numerical value.
* **`area_unit`** (ENUM): Area unit (`SQM`, `HECTARE`).
* **`number_of_rooms`** (INT): Number of rooms.
* **`floor_number`** (INT): Floor number.
* **`latitude`** (DOUBLE): Location latitude (-90 to 90).
* **`longitude`** (DOUBLE): Location longitude (-180 to 180).
* **`address`** (STRING): Structured address (e.g., "Aleppo, Syria").
* **`city`** (STRING): Location city.
* **`country`** (STRING): Location country.
* **`legal_status`** (ENUM): `LEASEHOLD`, `FREEHOLD`, `COURT_REGISTERED`, `SHARED_OWNERSHIP`.
* **`construction_date`** (DATETIME): Building construction date.

## 2. Table: `User`
* **`user_id`** (PK, INT): User identifier.
* **`name`** (STRING): User name.
* **`phone_number`** (STRING): User phone number.
* **`email`** (STRING): User email address.
* **`user_role`** (ENUM): `SELLER`, `BUYER`.

## 3. Table: `Media`
* **`media_id`** (PK, INT): Media identifier.
* **`property_id`** (FK -> `Property.property_id`, INT): Associated property identifier.
* **`type`** (ENUM): `IMAGE`, `VIDEO`.
* **`url`** (STRING): Media URL.
* **`thumbnail_url`** (STRING, NULLABLE): Required for video thumbnail.
* **`is_cover`** (BOOLEAN): `TRUE` if it is the cover media item.
* **`display_order`** (INT): Sorting order for display.

## 4. Table: `Amenities`
* **`amenity_id`** (PK, INT): Amenity identifier.
* **`name`** (STRING): Name of the amenity.
* **`description`** (STRING, NULLABLE): Description of the amenity.

## 5. Table: `PropertyAmenities`
* **`property_id`** (FK -> `Property.property_id`, INT): Associated property identifier.
* **`amenity_id`** (FK -> `Amenities.amenity_id`, INT): Associated amenity identifier.