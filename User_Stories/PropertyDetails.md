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
  * Each image is stored as a shared `File` record (see `FileStorage.md`) — images are served **only** through `GET /api/v1/media/{mediaId}`; the internal `StoredPath` is never exposed, and APIs return the `id_media` **UUID** plus `content_type` (the format: `image/jpeg`, `image/png`, `image/webp`).
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

> **ID strategy:** `Property`, `Amenities`, and `PropertyAmenities` are **never addressed by a public identifier** — they are internal only. They therefore expose **no `PublicId`**; they use only an internal `Id` (INT, PK). Cross-references to `Property`/`Amenities` use their internal `Id`. (This is the explicit exception noted for `Property`.)

## 1. Table: `Property`
* **`Id`** (PK, INT, IDENTITY): Property identifier — **never exposed** (the search feed returns it as the internal `property_id`; there is no public UUID for properties).
* **`PropertyType`** (ENUM): `APARTMENT`, `LAND`, `SHOP`, `VILLA`, `OFFICE`.
* **`Description`** (TEXT): Description of the property.
* **`Price`** (DECIMAL): Price of the property.
* **`AreaValue`** (DECIMAL): Area numerical value.
* **`AreaUnit`** (ENUM): Area unit (`SQM`, `HECTARE`).
* **`NumberOfRooms`** (INT): Number of rooms.
* **`FloorNumber`** (INT): Floor number.
* **`Latitude`** (DOUBLE): Location latitude (-90 to 90).
* **`Longitude`** (DOUBLE): Location longitude (-180 to 180).
* **`Address`** (STRING): Structured address (e.g., "Aleppo, Syria").
* **`City`** (STRING): Location city.
* **`Country`** (STRING): Location country.
* **`LegalStatus`** (ENUM): `LEASEHOLD`, `FREEHOLD`, `COURT_REGISTERED`, `SHARED_OWNERSHIP`.
* **`ConstructionDate`** (DATETIME): Building construction date.

## 2. Table: `Amenities`
* **`Id`** (PK, INT, IDENTITY): Amenity identifier — **never exposed**.
* **`Name`** (STRING): Name of the amenity.
* **`Description`** (STRING, NULLABLE): Description of the amenity.

## 3. Table: `PropertyAmenities`
* **`Id`** (PK, INT, IDENTITY): Internal junction identifier — **never exposed**.
* **`PropertyId`** (FK -> `Property.Id`, INT): Associated property identifier (internal).
* **`AmenityId`** (FK -> `Amenities.Id`, INT): Associated amenity identifier (internal).
