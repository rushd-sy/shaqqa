# File Storage (Shared)

# 1. Overview

- One shared **`File`** table stores every uploaded file on the platform (advertisement images, company logos, ...).
- Business tables **never store paths or URLs** — they only reference `File.PublicId`.
- **File paths are never exposed** in API responses; every file is addressed and served by its `PublicId` (UUID v7) through dedicated endpoints.

> **ID strategy:** `File` exposes `PublicId` (UUID v7) in all endpoints and as the FK target for referencing tables. The internal `Id` (INT, PK) is never exposed. `PublicId` is indexed (UNIQUE).

# 2. Database Schema (Entities & Attributes)

## 1. Table: `File`
* **`Id`** (PK, INT, IDENTITY): Internal identifier of the stored file — **never exposed**.
* **`PublicId`** (UUID v7, UNIQUE, INDEXED): The only file identifier exposed in responses and used in FKs. Serves the role previously held by `id_file`.
* **`FileName`** (VARCHAR): The original file name (kept for download metadata).
* **`ContentType`** (VARCHAR): The MIME type of the file — defines its **format** (`image/jpeg`, `image/png`, `image/webp`, ...).
* **`SizeBytes`** (BIGINT): File size in bytes.
* **`StoredPath`** (VARCHAR): Relative path (e.g., `ab/cd/ef/{uuid}.ext`), while `BASE_STORAGE_PATH` is stored as a single environment variable. **Never exposed** in API responses.
* **`CreatedAt`** (TIMESTAMP): Upload time.

> The physical file is removed from storage when the last referencing record is deleted.

# 3. API Endpoints

## 1. Get Media File
* **Endpoint:** `GET /api/v1/media/{mediaId}`
* **Description:** Serves image binaries (media). Defined in `PropertyListing.md` — media is the only file type referenced by an additional business record (`Media`), so it keeps its own serving endpoint.
* **Headers:** `Authorization: Bearer <User_Token>` (Optional for media of `ACTIVE` advertisements)

## 2. Get File
* **Endpoint:** `GET /api/v1/files/{fileId}`
* **Description:** Generic serving endpoint for all **non-media** files that live in the shared `File` table (company logos, ...). The file is addressed by its `PublicId` UUID — **the file path is never exposed**. Access is decided by the entity referencing the file:
    * company logos → public.
* **Path Parameters:**
    * `fileId` (UUID v7, required): The `PublicId` of the stored file.
* **Headers:** `Authorization: Bearer <User_Token>` (Optional for public files)
* **Response (200 OK):** file binary (`Content-Type` from the file's `ContentType`).
* **Error Responses:**
    * `401 Unauthorized`: Missing or invalid token (restricted file).
    * `403 Forbidden`: The user is not allowed to access this file.
    * `404 Not Found`: File does not exist.
