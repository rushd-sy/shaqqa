# 1. User Stories

## Buyer
- As a buyer, I want to view detailed technical specifications (area, rooms, bathrooms, floor, etc.) so that I can evaluate if the property fits my needs.
- As a buyer, I want to see high-quality images of the property so that I can visually assess its condition.
- As a buyer, I want to read a clear property description so that I can understand its features and advantages.
- As a buyer, I want to know additional details (furnishing status, age of property, amenities) so that I can make an informed decision.

## Seller
- As a seller, I want to add technical specifications (area, rooms, bathrooms, etc.) so that buyers can understand the property details.
- As a seller, I want to upload multiple images for my property so that I can showcase it effectively.
- As a seller, I want to write and edit a property description so that I can highlight key selling points.
- As a seller, I want to provide all required details and at least one image in one step, so that my advertisement is submitted for verification immediately.

> **Note:** Videos are **not** part of the program — no video storage, preview, or management anywhere (including third-party platforms like YouTube).


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
  * At least 1 image is required before an advertisement can be submitted for verification (and therefore for an active listing).
  * Only images are supported — the `Media` entity holds images only.
  * One media item must be marked as cover (`is_cover` = `TRUE`) and is returned first in the response.
  * Remaining media items are sorted by `display_order` in ascending order (If not specified, the first image is used as default cover).
  * Max media count defined (Max = 50).
  * Images are compressed upon upload (target size: < 500KB).
  * Allowed image formats: `JPEG`, `PNG`, `WebP`.
  * Images are stored on the platform file system under `wwwroot/uploads/advertisements/{id_advertisement}/` and served **only** through `GET /api/v1/media/{id_media}` — the internal `file_path` is never exposed in responses; APIs return the `id_media` **UUID** (see `PropertyListing.md`).
* **Edge Cases:**
  * No property images available at submission time: API returns `400 Bad Request` `{ "At least one image is required" }`.
  * Large number of images (images > 50): API returns `400 Bad Request` `{ "message": "Maximum number of media files is 50" }`.
  * Unsupported image format or file size above the limit: API returns `400 Bad Request` `{ "message": "Invalid image file" }`.
  * Deleting the cover image: the first remaining image by `display_order` becomes the new cover.

## Feature: Property Description
* **Acceptance Criteria:**
  * Property description is not empty and is displayed.
  * Property description includes roof condition information (if an apartment or villa).
* **Edge Cases:**
  * Property description is missing.


# 3. Database Schema (Entities & Attributes)

> **Scope note:** This document defines the `Property` entity only. The `Media` and `Advertisement` tables are defined in `PropertyListing.md`. The `User` table is defined in `users_doc/all-users.md`. The `Property` entity has **no lifecycle status** — public visibility is governed entirely by the parent `Advertisement.status` (see `PropertyListing.md` and `VerificationRequest.md`). All API endpoints for property/advertisement operations live in `PropertyListing.md`.

## 1. Table: `Property`
* **`id_property`** (PK, INT): Property identifier.
* **`property_type`** (ENUM): `APARTMENT`, `LAND`, `SHOP`, `VILLA`, `OFFICE`.
* **`description`** (TEXT): Description of the property.
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

## 2. Table: `Amenities`
* **`amenity_id`** (PK, INT): Amenity identifier.
* **`name`** (STRING): Name of the amenity.
* **`description`** (STRING, NULLABLE): Description of the amenity.

## 3. Table: `PropertyAmenities`
* **`id_property`** (FK -> `Property.id_property`, INT): Associated property identifier.
* **`amenity_id`** (FK -> `Amenities.amenity_id`, INT): Associated amenity identifier.
