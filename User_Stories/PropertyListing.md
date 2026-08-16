# 1. User Stories

## Customer
- As a user (Customer - property owner), **I want** to add a property with its property details **so that** I can publish verifiable advertisement on the program to potential customers.
- As a user (Customer - property owner), **I want** to update my existing property advertisement **so that** I can correct details, prices, or change contact information.
- As a user (Customer - property owner), **I want** to delete my property advertisement **so that** it is no longer visible once the property is sold or unavailable.
- As a user (Customer), **I want** to view a list of published property advertisements **so that** I can discover properties easily and find a property that suits my needs.

## Agent (Broker)
- As a user (Broker), **I want** to publish properties **so that** I can publish all properties advertisements that available from my real estate office.
- As a user (Broker), **I want** my verification requests to be marked as high priority **so that** my advertisements are fast-tracked and published quickly.
- As a user (Broker), **I want** to update and delete my published property advertisements **so that** I can maintain an accurate portfolio of available properties.

## Shaqqa Admin and Staff
- Shaqqa Admin and Staff can **NOT** add properties.
- As a Shaqqa admin OR Shaqqa Staff, **I want** to review verification requests and approve, request edits, or reject them **so that** only valid advertisements become public (see `VerificationRequest.md`).
- As a Shaqqa admin OR Shaqqa Staff, **I want** to monitor, edit, or delete user advertisements **so that** I can maintain content quality and remove spam or violations.

## Company
- Company Admin can **NOT** add properties.

### Company Admin
- As a company owner, I want to view, edit, and delete **advertisements** created by any of my staff, so that I can ensure all company listings meet our quality standards.

### Company Staff
- As a company staff, I want to see company staff tools (common with broker) like add, update and delete property and my advertisements, so that I can manage my properties in one place.

---

# 2. Acceptance Criteria & Edge Cases

## Feature: Add Property Advertisement
* **Acceptance Criteria:**
    * The user (`CUSTOMER`, `BROKER`, or `COMPANY_STAFF`) creates the advertisement with status **`PENDING`** and it is **automatically submitted for verification** (a `PUBLISH` `VerificationRequest` is created in the same transaction).
    * The advertisement becomes visible to all `CUSTOMER`s only when its status is `ACTIVE` (approved by Shaqqa Admin/Staff — see `VerificationRequest.md`).
    * `BROKER` and `COMPANY_STAFF` verification requests are marked `HIGH` priority so they are reviewed first.
    * At least 1 image is required before the advertisement can be created (and therefore before submission).
* **Edge Cases:**
    * Public infrastructure (such as hospitals, schools, mosques or government sites) as a selected location should be ignored.
    * Outside Syria borders as the selected location should be ignored.
    * The user does not actually own the property: if anyone can add a property, the program will face issues with scammers.
    * Fake property prices, like low prices to attract more people on the program, or high prices to show it as valuable.
    * Add images for another building or location.
    * Fake phone numbers as contact information.
    * Fake property details.

## Feature: Update Property Advertisement
* **Acceptance Criteria:** 3 scenarios:
    1. **`PENDING` advertisement** — saved **directly on the same row** (no new version):
        * The existing `PENDING` verification request is **hard-deleted** and a **new `PUBLISH` verification request** is created (replacement — renews the verification cycle).
        * `updated_at` is set to now (it was `NULL` at creation); `publish_date` stays `NULL` (not yet approved).
    2. **`ACTIVE` advertisement** — creates a **replacement version**:
        * A **new `Advertisement` row** is created (status `PENDING`) with `id_superseded_advertisement` pointing to the live version, plus an **`UPDATE` verification request**.
        * The live version stays `ACTIVE` until the request is `APPROVED`, then the live version becomes `DELETED` and the replacement version becomes `ACTIVE`.
        * The replacement **keeps the original `publish_date`** of the first version (feed order stays stable) and sets `updated_at` to now.
    3. **`DRAFT` advertisement** (assigned **only** by a `NEEDS_EDIT` review) — saved **directly on the same row**; the save **auto-resubmits** the advertisement (a new verification request is created and the status becomes `PENDING`). `updated_at` is set to now.
* **Media behavior (details in Feature: Media Management):**
    * Media can be added directly through the update request itself (the `images` field of the `Update Advertisement` endpoint) or through the dedicated media endpoints — same rules apply either way.
    * Scenarios (1) and (3): media records are **not affected** unless the user explicitly changes media — they keep pointing to the same advertisement and `File` records; deleted/changed media is cleaned up immediately.
    * Scenario (2): media records are **copied** to the replacement version (new `id_media` UUIDs, same `id_file` references, cover, and order). The `File` records and their physical files are **shared** — nothing is duplicated on disk. After the cycle resolves: on `APPROVED` the superseded version's media records are deleted and any `File` no longer referenced by the replacement is removed from storage; on `REJECTED` the replacement version's media records are deleted and its `File` records are removed.
* **Permissions:** the author of the advertisement, their managing `COMPANY_ADMIN`, or authorized `SHAQQA_ADMIN`/`SHAQQA_STAFF` can update the advertisement.
* **Edge Cases:**
    * A user attempts to update an advertisement that belongs to another user.
    * Changing the location to an invalid area (e.g., outside Syria borders or public infrastructure).
    * Submitting fake property details or phone numbers during the update.
    * `CUSTOMER` or `BROKER` update advertisements after an update from `COMPANY_ADMIN`, `SHAQQA_ADMIN`/`SHAQQA_STAFF`.

## Feature: Delete Property Advertisement
* **Acceptance Criteria:**
    * The author of the advertisement, their managing `COMPANY_ADMIN`, or authorized program staff (`SHAQQA_ADMIN`/`SHAQQA_STAFF`) can delete the advertisement **directly — no verification required**.
    * Deletion is a soft delete (`status` = `DELETED`); the advertisement disappears from public listings.
    * When an `UPDATE` request is `APPROVED`, the superseded version is also set to `DELETED` (replacement).
* **Edge Cases:**
    * Attempting to delete an advertisement that has already been deleted or does not exist.
    * A user attempts to delete an advertisement owned by someone else.

## Feature: List Property Advertisements (Feed)
* **Acceptance Criteria:**
    * All active users can retrieve and view a list of advertisements where `status` is `ACTIVE`.
    * Results are ordered by **`publish_date` descending** (latest published first).
    * Advertisements belonging to a deactivated user (`is_active` is `FALSE`) or a user associated with a deactivated company (`is_active` is `FALSE` in the `Company` table) must be automatically hidden and excluded from public listings.
* **Edge Cases:**
    * Retrieving page numbers that exceed the total number of available pages.
    * Returning an empty list if no advertisements are currently published or match the search parameters.

## Feature: Advertisement Details & Recent Visits
* **Acceptance Criteria:**
    * `GET /api/v1/advertisements/{advertisementId}` returns full details (property specs, media, description, contact info).
    * As a **side effect**, the backend implicitly records the visit in the viewer's recent visits history — no separate client request is required (see `history/RecentVisits.md`).
* **Edge Cases:**
    * Advertisement does not exist or is inactive/deleted: `404 Not Found`, nothing is recorded.
    * Unauthenticated viewer: the advertisement still loads, but no visit is recorded.

## Feature: Media Management
* **Acceptance Criteria:** 3 scenarios (matching the update scenarios):
    1. **`PENDING` advertisement** — media changes are **direct** (no verification): applied immediately to the same version. Adding media **renews the verification cycle** (the existing `PENDING` request is hard-deleted and a new one is created), exactly like any other update to a `PENDING` advertisement.
    2. **`ACTIVE` advertisement:**
        * **Adding media is verified**: a replacement version is created (data + media records copied, files shared) with the new image, plus an **`UPDATE` verification request**; the live version stays `ACTIVE` until `APPROVED`. If an `UPDATE` cycle is **already pending**, the image is added directly to the pending version (the pending request already covers it).
        * **Deleting media is direct — no verification required** (applies to the version referenced by the request), but at least 1 image should be kept.
        * Setting the cover / `display_order` is **direct** (no verification).
    3. **`DRAFT` advertisement** — media changes are **direct** (no verification, no request involvement).
* **ID and security (unified file storage):**
    * Every uploaded file (media, company logos, ...) is stored as one record in the shared **`File`** table, referenced by its `PublicId` (UUID v7) — see `FileStorage.md`. **No business table stores file paths or URLs.**
    * `id_media` is a **UUID** returned in media responses together with `content_type` (the image format).
    * **File paths are never exposed** in responses. Images are served through `GET /api/v1/media/{mediaId}`; all other files through the generic file endpoint (see `FileStorage.md`).
* Adding a new cover image replaces the current cover. Deleting the cover image: the first remaining image by `display_order` becomes the new cover.
* **Edge Cases:**
    * Adding an image to an advertisement that does not exist: `404 Not Found`.
    * Deleting a media item that does not exist: `404 Not Found`.
    * Exceeding 50 images, unsupported format, or size above the limit: `400 Bad Request`.
    * Deleting the cover image: the first remaining image by `display_order` becomes the new cover.

---

# 3. API Endpoints (Advertisements)

> All endpoints are versioned under `/api/v1/`. Authentication (`Authorization: Bearer <User_Token>`) is required for all endpoints except public viewing (`GET` feed, details, and media file of `ACTIVE` advertisements). Endpoints that upload files (advertisement creation/update and media) use `multipart/form-data`; everything else uses JSON.

> **ID strategy:** `advertisement_id` and `id_media` in request/response bodies are **UUID v7** values equal to `Advertisement.PublicId` / `Media.PublicId`. Path parameters are `camelCase` (`advertisementId`, `mediaId`). The internal `Id` (INT) is never exposed.

## 1. Create Advertisement
* **Endpoint:** `POST /api/v1/advertisements`
* **Description:** Creates a new advertisement with its property details as **`PENDING`** and **automatically submits it for verification** (creates a `PUBLISH` `VerificationRequest` in the same transaction). No DRAFT step. The same request uploads the initial images.
* **Headers:** `Authorization: Bearer <User_Token>`
* **Request Body:** `multipart/form-data`
    * `title` (string, required)
    * `contract_type` (enum, required): `SALE`, `RENT`.
    * `contact_info` (string, required)
    * `property_details` (JSON string, required): the payload defined in `PropertyDetails.md`.
    * `images` (file[], required): at least 1 image (JPEG, PNG, or WebP, compressed < 500KB).
* **Response (201 Created):**
```json
{
  "advertisement_id": "b7f0c8a2-3c4d-5e6f-7081-000000000003",
  "verification_request_id": "9a31d4e6-4d5e-6f70-8192-000000000004",
  "message": "Advertisement created and submitted for verification."
}
```
* **Error Responses:**
    * `400 Bad Request`: Missing or invalid required fields, or no image attached (see `PropertyDetails.md` validation rules).
    * `401 Unauthorized`: Missing or invalid token.
    * `403 Forbidden`: User account is deactivated or lacks permissions to post (`COMPANY_ADMIN`, `SHAQQA_ADMIN`/`SHAQQA_STAFF` cannot create).

## 2. Update Advertisement
* **Endpoint:** `PUT /api/v1/advertisements/{advertisementId}`
* **Description:** Updates the advertisement. The behavior depends on the current status (3 scenarios):
    * **`PENDING`:** saved directly on the same row; the existing `PENDING` verification request is **hard-deleted** and a new `PUBLISH` request is created (replacement — renews the verification cycle). `updated_at` is set to now.
    * **`ACTIVE`:** a **new `Advertisement` row** (replacement version, status `PENDING`) is created with an **`UPDATE` verification request**; the old version stays live until `APPROVED`, then the old version becomes `DELETED` and the new version becomes `ACTIVE`. The replacement keeps the original `publish_date` and sets `updated_at` to now.
    * **`DRAFT`** (assigned only by a `NEEDS_EDIT` review): saved directly on the same row and **auto-resubmitted** (a new verification request is created, status becomes `PENDING`). `updated_at` is set to now.
    * When `images` are attached, media follows the Media Management rules (see Media Management): `PENDING` → added directly and the verification cycle is renewed; `ACTIVE` → added to a replacement version (staged for verification); `DRAFT` → added directly.
* **Path Parameters:**
    * `advertisementId` (UUID v7, required): The `PublicId` of the advertisement.
* **Headers:** `Authorization: Bearer <User_Token>`
* **Request Body:** `multipart/form-data`
    * `title` (string, required)
    * `contract_type` (enum, required): `SALE`, `RENT`.
    * `contact_info` (string, required)
    * `property_details` (JSON object, required): the payload defined in `PropertyDetails.md`.
    * `images` (file[], optional): images to add directly through this endpoint (JPEG, PNG, or WebP, compressed < 500KB, max 50 images). Missing means no media changes.
* **Response (200 OK) — PENDING:**
```json
{
  "advertisement_id": "b7f0c8a2-3c4d-5e6f-7081-000000000003",
  "verification_request_id": "6d2b4e8f-5e6f-7081-8192-00000000000f",
  "message": "Changes saved. A new verification request was created."
}
```
* **Response (200 OK) — ACTIVE (replacement version):**
```json
{
  "advertisement_id": "f3e1a9c7-6f70-8192-a3b4-000000000010",
  "verification_request_id": "6d2b4e8f-5e6f-7081-8192-00000000000f",
  "message": "Changes staged for verification. The current version stays live until approved."
}
```
* **Response (200 OK) — DRAFT (saved and resubmitted):**
```json
{
  "advertisement_id": "b7f0c8a2-3c4d-5e6f-7081-000000000003",
  "verification_request_id": "6d2b4e8f-5e6f-7081-8192-00000000000f",
  "message": "Draft saved and resubmitted for verification."
}
```
* **Error Responses:**
    * `400 Bad Request`: Invalid payload or edge case violation.
    * `401 Unauthorized`: Missing or invalid token.
    * `403 Forbidden`: User is not the owner of the advertisement, their managing `COMPANY_ADMIN`, or authorized program staff, or user account is deactivated.
    * `404 Not Found`: Advertisement does not exist.

## 3. Delete Advertisement
* **Endpoint:** `DELETE /api/v1/advertisements/{advertisementId}`
* **Description:** Deletes the advertisement **directly — no verification required** (soft delete: `status` = `DELETED`).
* **Path Parameters:**
    * `advertisementId` (UUID v7, required): The `PublicId` of the advertisement.
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

## 4. List Advertisements (Feed)
* **Endpoint:** `GET /api/v1/advertisements`
* **Description:** Retrieves the discovery feed: only advertisements with `status` = `ACTIVE`, **ordered by `updated_at` descending then by `publish_date` descending** (latest first). Advertisements of deactivated users/companies are excluded.
* **Query Parameters:**
    * `page` (integer, optional): The page index to retrieve (e.g., `?page=1`). Default is `1`.
    * `limit` (integer, optional): The number of advertisements to return per page (e.g., `?limit=10`). Default is `10`.
* **Headers:** `Authorization: Bearer <User_Token>` (Optional for public viewing)
* **Response (200 OK):**
```json
{
  "data": [
    {
      "advertisement_id": "b7f0c8a2-3c4d-5e6f-7081-000000000003",
      "user_id": "3a1c9e57-1a2b-4c3d-8e9f-000000000001",
      "publish_date": "2026-08-10T10:00:00Z",
      "updated_at": null,
      "contact_info": "+963xxxxxxxxx",
      "property_details": {
        "description": "string",
        "price": 150000000
      },
      "media": [
      { "id_media": "1a2b3c4d-5e6f-7081-923a-000000000005", "is_cover": true, "display_order": 0 }
      ]
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

## 5. Get Advertisement Details
* **Endpoint:** `GET /api/v1/advertisements/{advertisementId}`
* **Description:** Returns full advertisement details (technical specs, media, description, contact info). **Side effect:** the backend implicitly records the visit in the viewer's recent visits history (see `history/RecentVisits.md`).
* **Path Parameters:**
    * `advertisementId` (UUID v7, required): The `PublicId` of the advertisement.
* **Headers:** `Authorization: Bearer <User_Token>` (Optional for public viewing)
* **Response (200 OK):**
```json
{
  "advertisement_id": "b7f0c8a2-3c4d-5e6f-7081-000000000003",
  "user_id": "3a1c9e57-1a2b-4c3d-8e9f-000000000001",
  "publish_date": "2026-08-10T10:00:00Z",
  "updated_at": null,
  "contact_info": "+963xxxxxxxxx",
  "title": "Apartments for Sale in Aleppo New",
  "contract_type": "SALE",
  "property_details": {
    "id_property": 7,
    "price": 150000000,
    "area": { "value": 120, "unit": "SqM" },
    "location": { "latitude": 36.2021, "longitude": 37.1343, "address": "Aleppo, Syria" },
    "description": "Property description here",
    "media": [
      { "id_media": "1a2b3c4d-5e6f-7081-923a-000000000005", "content_type": "image/jpeg", "is_cover": true, "display_order": 0 }
    ]
  }
}
```
* **Error Responses:**
    * `400 Bad Request`: Invalid advertisement id (e.g., malformed UUID).
    * `404 Not Found`: Advertisement does not exist or is inactive/deleted. Nothing is recorded.

## 6. Add Media (Images)
* **Endpoint:** `POST /api/v1/advertisements/{advertisementId}/media`
* **Description:** Uploads an image for the advertisement. The behavior depends on the current status:
    * **`PENDING`:** the image is added **directly** and the verification cycle is **renewed** (the existing `PENDING` request is hard-deleted and a new one is created).
    * **`DRAFT`:** the image is added **directly** — no verification, no request involvement.
    * **`ACTIVE`:** **verified** — a new replacement version is created (data + media copied, files shared) with the new image, plus an **`UPDATE` verification request**; the live version stays `ACTIVE` until `APPROVED`. If an `UPDATE` cycle is already pending, the image is added directly to the pending version.
* **Path Parameters:**
    * `advertisementId` (UUID v7, required): The `PublicId` of the advertisement.
* **Headers:** `Authorization: Bearer <User_Token>`
* **Request Body:** `multipart/form-data`
    * `file` (file, required): The image file (JPEG, PNG, or WebP, compressed < 500KB).
    * `is_cover` (boolean, optional): Set `TRUE` to make this image the cover.
    * `display_order` (integer, optional): Sorting position. Defaults to the end of the list.
* **Response (201 Created) — added directly (PENDING / DRAFT / ACTIVE with pending cycle):**
```json
{
  "id_media": "1a2b3c4d-5e6f-7081-923a-000000000005",
  "content_type": "image/jpeg",
  "is_cover": false,
  "display_order": 3,
  "message": "Media added successfully."
}
```
* **Response (202 Accepted) — staged for verification (ACTIVE, new cycle):**
```json
{
  "advertisement_id": "f3e1a9c7-6f70-8192-a3b4-000000000010",
  "id_media": "1a2b3c4d-5e6f-7081-923a-000000000005",
  "content_type": "image/jpeg",
  "verification_request_id": "6d2b4e8f-5e6f-7081-8192-00000000000f",
  "message": "Media staged for verification. The current version stays live until approved."
}
```
* **Error Responses:**
    * `400 Bad Request`: Unsupported image format, size above the limit, or media count exceeds 50.
    * `401 Unauthorized`: Missing or invalid token.
    * `403 Forbidden`: User is not the owner of the advertisement (or their managing `COMPANY_ADMIN`).
    * `404 Not Found`: Advertisement does not exist.

## 7. Update Media (Set Cover / Reorder)
* **Endpoint:** `PATCH /api/v1/advertisements/{advertisementId}/media/{mediaId}`
* **Description:** Updates the cover flag and/or the display order of an existing image. **Direct — no verification required** (any status).
* **Path Parameters:**
    * `advertisementId` (UUID v7, required): The `PublicId` of the advertisement.
    * `mediaId` (UUID v7, required): The `PublicId` of the media item.
* **Headers:** `Authorization: Bearer <User_Token>`
* **Request Body:** JSON (at least one field)
```json
{
  "is_cover": true,
  "display_order": 1
}
```
* **Response (200 OK):**
```json
{
  "id_media": "1a2b3c4d-5e6f-7081-923a-000000000005",
  "is_cover": true,
  "display_order": 1,
  "message": "Media updated successfully."
}
```
* **Error Responses:**
    * `400 Bad Request`: Invalid payload.
    * `401 Unauthorized`: Missing or invalid token.
    * `403 Forbidden`: User is not the owner of the advertisement (or their managing `COMPANY_ADMIN`).
    * `404 Not Found`: Advertisement or media item does not exist.

## 8. Delete Media (Image)
* **Endpoint:** `DELETE /api/v1/advertisements/{advertisementId}/media/{mediaId}`
* **Description:** Deletes a specific image. **Direct — no verification required** (on any status, including `ACTIVE`). If the cover image is deleted, the first remaining image by `display_order` becomes the new cover, keeping at least 1 image (prevent deletion case).
* **Path Parameters:**
    * `advertisementId` (UUID v7, required): The `PublicId` of the advertisement.
    * `mediaId` (UUID v7, required): The `PublicId` of the media item.
* **Headers:** `Authorization: Bearer <User_Token>`
* **Response (200 OK):**
```json
{
  "message": "Media deleted successfully."
}
```
* **Error Responses:**
    * `400 Bad Request`: Can not delete the last image.
    * `401 Unauthorized`: Missing or invalid token.
    * `403 Forbidden`: User is not the owner of the advertisement (or their managing `COMPANY_ADMIN`).
    * `404 Not Found`: Advertisement or media item does not exist.

## 9. Get Media File
* **Endpoint:** `GET /api/v1/media/{mediaId}`
* **Description:** Serves the image binary file. The file is addressed by its `PublicId` UUID — **the file path is never exposed** in any response. The response `Content-Type` is taken from the file's `content_type` (format). Public access is allowed only for media belonging to an `ACTIVE` advertisement; the owner (or their managing `COMPANY_ADMIN` / `SHAQQA_ADMIN` / `SHAQQA_STAFF`) can also access media of their own non-active versions.
* **Path Parameters:**
    * `mediaId` (UUID v7, required): The `PublicId` of the media item.
* **Headers:** `Authorization: Bearer <User_Token>` (Optional for media of `ACTIVE` advertisements)
* **Response (200 OK):** image binary (`Content-Type: image/jpeg` | `image/png` | `image/webp`, with `Cache-Control`).
* **Error Responses:**
    * `401 Unauthorized`: Missing or invalid token (media of a non-active version).
    * `403 Forbidden`: The advertisement is not `ACTIVE` and the user is not its owner.
    * `404 Not Found`: Media item does not exist.

---

# 4. Database Schema (Entities & Attributes)

> **ID strategy:** `Advertisement` and `Media` expose a `PublicId` (UUID v7, indexed, UNIQUE) used in all endpoints and as FK targets. The internal `Id` (INT, PK) is never exposed.

## 1. Table: `Advertisement`
* **`Id`** (PK, INT, IDENTITY): Internal identifier for the advertisement **version** — **never exposed**.
* **`PublicId`** (UUID v7, UNIQUE, INDEXED): Public identifier for the advertisement **version**; exposed as `advertisement_id` in all endpoints and used as the FK target.
* **`UserId`** (FK -> `User.PublicId`, UUID): Public identifier of the user who owns/created the advertisement.
* **`PropertyId`** (FK -> `Property.Id`, INT): Internal identifier of the associated property (see `PropertyDetails.md`). `Property` has no public id.
* **`SupersededAdvertisementId`** (FK -> `Advertisement.PublicId`, UUID, NULLABLE): The version this version replaces (set only on replacement versions created by `UPDATE`; on approval the superseded version becomes `DELETED`).
* **`PublishDate`** (DATETIME/TIMESTAMP, NULLABLE): The date when the **first version** of the advertisement became `ACTIVE`. Used to order the feed (latest first). **Preserved across replacements** — a replacement version inherits the original `publish_date`, it is never refreshed.
* **`UpdatedAt`** (DATETIME/TIMESTAMP, NULLABLE): `NULL` at creation; set to now on every update (PENDING/DRAFT in-place edits and creation of replacement versions).
* **`Title`** (VARCHAR): The advertisement's title (e.g., "Apartments for Sale in Aleppo New"), used by free-text search.
* **`ContractType`** (ENUM): `RENT`, `SALE`. Whether the property is offered for rent or sale in this advertisement.
* **`ContactInfo`** (VARCHAR): Contact information listed for the advertisement.
* **`Status`** (ENUM): Lifecycle status of the advertisement version. Values: `PENDING`, `ACTIVE`, `DRAFT`, `REJECTED`, `DELETED`.
    * Public visibility is derived from `status`: an advertisement version is publicly visible **only** when `status` = `ACTIVE`.
    * `DRAFT` is assigned **only** by Shaqqa Admin/Staff after a review (`NEEDS_EDIT`) — users never create `DRAFT` advertisements.
    * Transitions are driven by `VerificationRequest` (see `VerificationRequest.md`).

## 2. Table: `Media`
* **`Id`** (PK, INT, IDENTITY): Internal identifier of the media item — **never exposed**.
* **`PublicId`** (UUID v7, UNIQUE, INDEXED): Public identifier of the media item; exposed as `id_media` in media responses and used as the FK target.
* **`AdvertisementId`** (FK -> `Advertisement.PublicId`, UUID): Public identifier of the associated advertisement version.
* **`FileId`** (FK -> `File.PublicId`, UUID): The stored image. The shared `File` table and the generic file serving endpoint are defined in `FileStorage.md`. The image format is `File.ContentType` — only image MIME types are allowed (`image/jpeg`, `image/png`, `image/webp`).
* **`IsCover`** (BOOLEAN): `TRUE` if it is the cover media item.
* **`DisplayOrder`** (INT): Sorting order for display.

---

# Role Permission Matrix

| Action | CUSTOMER | BROKER | COMPANY_STAFF | COMPANY_ADMIN | SHAQQA_ADMIN/STAFF |
|---|---|---|---|---|---|
| Create advertisement (status `PENDING`, auto-submitted) | Yes | Yes | Yes | No | No |
| Submit for verification | Auto at creation (`NORMAL` priority) | Auto at creation (`HIGH` priority) | Auto at creation (`HIGH` priority) | No | No |
| Update `PENDING` advertisement (replaces verification request) | Yes | Yes | Yes | Yes (staff ads) | Yes (moderation) |
| Update `ACTIVE` advertisement (new version + verification) | Yes | Yes | Yes | Yes (staff ads) | Yes (moderation, direct) |
| Update `DRAFT` advertisement (saved and auto-resubmitted) | Yes | Yes | Yes | Yes (staff ads) | Yes (moderation) |
| Delete advertisement | Yes (direct) | Yes (direct) | Yes (direct) | Yes (staff ads) | Yes (moderation, direct) |
| Add media — `ACTIVE` (staged + verification) | Yes | Yes | Yes | Yes (staff ads) | Yes (moderation, direct) |
| Add media — `PENDING`/`DRAFT` (direct) | Yes | Yes | Yes | Yes (staff ads) | Yes (moderation, direct) |
| Update media (set cover / reorder) — direct | Yes | Yes | Yes | Yes (staff ads) | Yes (moderation, direct) |
| Delete media (direct) | Yes | Yes | Yes | Yes (staff ads) | Yes (moderation, direct) |
| Review verification requests | No | No | No | No | Yes |

> Deletion of advertisements and media never requires verification. Publishing (`PUBLISH`) and updates (`UPDATE`) go through verification. `DRAFT` is never created by users — it is assigned only by Shaqqa Admin/Staff after a `NEEDS_EDIT` review, and the resubmission happens through the update endpoint.
