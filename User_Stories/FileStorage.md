# File Storage (Shared)

# 1. Overview

- One shared **`File`** table stores every uploaded file on the platform (advertisement images, company logos, ...).
- Business tables **never store paths or URLs** — they only reference `File.id_file`.
- **File paths are never exposed** in API responses; every file is addressed and served by its UUID through dedicated endpoints.

# 2. Database Schema (Entities & Attributes)

## 1. Table: `File`
* **`id_file`** (PK, UUID): Unique identifier of the stored file — the only file identifier exposed in responses.
* **`file_name`** (VARCHAR): The original file name (kept for download metadata).
* **`content_type`** (VARCHAR): The MIME type of the file — defines its **format** (`image/jpeg`, `image/png`, `image/webp`, ...).
* **`size_bytes`** (BIGINT): File size in bytes.
* **`stored_path`** (VARCHAR): Relative path (e.g., `ab/cd/ef/uuid.ext`), while `BASE_STORAGE_PATH` is stored as a single environment variable. **Never exposed** in API responses.
* **`created_at`** (TIMESTAMP): Upload time.

> The physical file is removed from storage when the last referencing record is deleted.

# 3. API Endpoints

## 1. Get Media File
* **Endpoint:** `GET /api/v1/media/{id_media}`
* **Description:** Serves image binaries (media). Defined in `PropertyListing.md` — media is the only file type referenced by an additional business record (`Media`), so it keeps its own serving endpoint.
* **Headers:** `Authorization: Bearer <User_Token>` (Optional for media of `ACTIVE` advertisements)

## 2. Get File
* **Endpoint:** `GET /api/v1/files/{id_file}`
* **Description:** Generic serving endpoint for all **non-media** files that live in the shared `File` table (company logos, ...). The file is addressed by its `id_file` UUID — **the file path is never exposed**. Access is decided by the entity referencing the file:
    * company logos → public.
* **Path Parameters:**
    * `id_file` (UUID, required): The unique identifier of the stored file.
* **Headers:** `Authorization: Bearer <User_Token>` (Optional for public files)
* **Response (200 OK):** file binary (`Content-Type` from the file's `content_type`).
* **Error Responses:**
    * `401 Unauthorized`: Missing or invalid token (restricted file).
    * `403 Forbidden`: The user is not allowed to access this file.
    * `404 Not Found`: File does not exist.
