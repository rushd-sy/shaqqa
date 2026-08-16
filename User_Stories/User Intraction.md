# 1. User Stories

## User
- As a user, I want to add properties I liked to my favorites list, so I can return to the properties that I liked quickly.
- As a user, I want to remove properties from my favorites list.
- As a user, I want to be able to book a visit to the property I have chosen, to get a real look at the property.
- As a user, I want there to be date options so I can choose a specific date that suits me.
- As a user, I want my favorites sorted by Create_At (The most recently saved item is at the top of the list).
- As a user, I want to see a list of upcoming appointments.
- As a user, I want to see if my booking is successfully or not.
- As a user, I want to cancel an appointment from my appointment list.
- As a user, I want to confirm whether my reservation has been cancelled or not.

# 2. Acceptance Criteria & Edge Cases

## Feature: Favorites
* **Acceptance Criteria:**
    * Given the user is logged in and browsing a specific property. When they click the "Add to Favorites" button. Then the property is saved to the Favorites table with the current time recorded in `CreatedAt`, and a confirmation message is displayed indicating successful addition.
    * When the user clicks the "Remove" button from the favorites list. Then the record for `favorite_id` is deleted from the database and the property disappears from the list immediately.
    * When the user opens the "Favorites" page. Then data is fetched in descending order based on the `CreatedAt` field.
* **Edge Cases:**
    * Given the property is already in the user's favorites list. When the user attempts to add it again. Then the system blocks the operation and returns a 409 Conflict error to prevent duplication in the database.

## Feature: Property Booking
* **Acceptance Criteria:**
    * When the user opens the booking page for a specific property. Then the system displays only the available times from the Property_Availability table where `IsBooked` = `FALSE`.
    * Given the user selects an available date and clicks "Confirm Booking". When the request is sent to `/api/booking`. Then a new record is created in Booking_Visit, and its status is automatically set to `PENDING`. The appointment status in Property_Availability is changed to `IsBooked` = `TRUE` to prevent another user from booking it. The user receives a booking success message showing the booking number and its current status as `PENDING`.
    * Given the user has an upcoming booking with a `PENDING` or `CONFIRMED` status. When the user clicks the "Cancel Booking" button. Then the booking status in the Booking_Visit table changes to status = `CANCELLED`. The Property_Availability table for this appointment is updated to `IsBooked` = `FALSE`. The interface is immediately updated to show the user a visual confirmation that the booking is now `CANCELLED`.
    * When the user opens the "My Appointments" page. Then the system displays a list of all upcoming bookings with the status of each booking clearly shown.
* **Edge Cases:**
    * *(No explicit edge cases defined in the provided criteria)*

# 3. API Endpoints (Login & Roles)

## 1. Book a Property Visit
* **Endpoint:** POST /api/v1/booking
* **Description:** Allows the user to view the property in person.
* **Headers:** `Authorization: Bearer <User_Token>`
* **Request Body:**
```json
{
  "property_id": 7,
  "availability_id": "5f607182-b4c5-d6e7-f809-00000000000b"
}
```
* **Response (201 Created):**
```json
{
  "message": "Property booked successfully.",
  "booking_id": "4e5f6071-a3b4-c5d6-e7f8-00000000000a",
  "status": "PENDING"
}
```
* **Error Responses:**
    * `404 Not Found`: If the `property_id` does not exist in the database.
    * `400 Bad Request`: If the sent data is incomplete or incorrect.

## 2. Add Property to Favorites
* **Endpoint:** POST /api/v1/favorites
* **Description:** Allows the user to add a specific property to their favorites list to return to it later.
* **Headers:** `Authorization: Bearer <User_Token>`
* **Request Body:**
```json
{
  "user_id": "3a1c9e57-1a2b-4c3d-8e9f-000000000001",
  "property_id": 7
}
```
* **Response (201 Created):**
```json
{
  "message": "Property added to favorites successfully.",
  "favorite_id": "3d4e5f60-92a3-b4c5-d6e7-000000000009"
}
```
* **Error Responses:**
    * `404 Not Found`: If the `property_id` does not exist in the database.
    * `409 Conflict`: If the user has already added this property to their favorites.
    * `400 Bad Request`: If the sent data is incomplete or incorrect.

# 4. Database Schema (Entities & Attributes)

> **ID strategy:** `Favorite`, `Booking_Visit`, and `Property` references follow the unified scheme. `Favorite`, `Booking_Visit`, and `Property_Availability` are all exposed (their ids appear in request/response bodies), so each gets `PublicId` (UUID v7, indexed, UNIQUE). `Property` itself has **no `PublicId`** — it is referenced by its internal `Id` (INT), exposed only as the internal `property_id`.

## 1. Table: `User`
*(Referenced by `UserId` → `User.PublicId`. Full definition in `users_doc/all-users.md`.)*

## 2. Table: `Property`
*(Referenced by `PropertyId` → `Property.Id` (INT, internal). Full definition in `PropertyDetails.md`.)*

## 3. Table: `Favorite`
* **`Id`** (PK, INT, IDENTITY): Internal identifier for the favorite record — **never exposed**.
* **`PublicId`** (UUID v7, UNIQUE, INDEXED): Public identifier for the favorite record; exposed as `favorite_id` in the response.
* **`UserId`** (FK -> `User.PublicId`, UUID, COMPOSITE UNIQUE INDEX): Public identifier of the associated user.
* **`PropertyId`** (FK -> `Property.Id`, INT, COMPOSITE UNIQUE INDEX): Internal identifier of the associated property.
* **`CreatedAt`** (DATETIME): The date and time the favorite record was created.

## 4. Table: `Booking_Visit`
* **`Id`** (PK, INT, IDENTITY): Internal identifier for the booking record — **never exposed**.
* **`PublicId`** (UUID v7, UNIQUE, INDEXED): Public identifier for the booking record; exposed as `booking_id` in the response.
* **`UserId`** (FK -> `User.PublicId`, UUID): Public identifier of the user who made the booking.
* **`PropertyId`** (FK -> `Property.Id`, INT): Internal identifier of the property being booked.
* **`CreatedAt`** (DATETIME): The date and time the booking was created.
* **`AppointmentDatetime`** (DATETIME): The scheduled date and time of the appointment.
* **`Status`** (ENUM): The current status of the booking (`PENDING`, `CONFIRMED`, `CANCELLED`).
* **`AvailabilityId`** (FK -> `Property_Availability.PublicId`, UUID): Public identifier of the specific availability slot selected.

## 5. Table: `Property_Availability`
* **`Id`** (PK, INT, IDENTITY): Internal identifier for the availability slot — **never exposed**.
* **`PublicId`** (UUID v7, UNIQUE, INDEXED): Public identifier for the availability slot; exposed as `availability_id` in request/response bodies.
* **`PropertyId`** (FK -> `Property.Id`, INT): Internal identifier of the associated property.
* **`StartTime`** (DATETIME): The start time of the availability slot.
* **`EndTime`** (DATETIME): The end time of the availability slot.
* **`IsBooked`** (BOOLEAN): Status indicating if the slot has been reserved (`TRUE` or `FALSE`).
