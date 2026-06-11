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
    * Given the user is logged in and browsing a specific property. When they click the "Add to Favorites" button. Then the property is saved to the Favorites table with the current time recorded in `created_at`, and a confirmation message is displayed indicating successful addition.
    * When the user clicks the "Remove" button from the favorites list. Then the record for `favorite_id` is deleted from the database and the property disappears from the list immediately.
    * When the user opens the "Favorites" page. Then data is fetched in descending order based on the `created_at` field.
* **Edge Cases:**
    * Given the property is already in the user's favorites list. When the user attempts to add it again. Then the system blocks the operation and returns a 409 Conflict error to prevent duplication in the database.

## Feature: Property Booking
* **Acceptance Criteria:**
    * When the user opens the booking page for a specific property. Then the system displays only the available times from the Property_Availability table where `is_booked` = `FALSE`.
    * Given the user selects an available date and clicks "Confirm Booking". When the request is sent to `/api/booking`. Then a new record is created in Booking_Visit, and its status is automatically set to `PENDING`. The appointment status in Property_Availability is changed to `is_booked` = `TRUE` to prevent another user from booking it. The user receives a booking success message showing the booking number and its current status as `PENDING`.
    * Given the user has an upcoming booking with a `PENDING` or `CONFIRMED` status. When the user clicks the "Cancel Booking" button. Then the booking status in the Booking_Visit table changes to status = `CANCELLED`. The Property_Availability table for this appointment is updated to `is_booked` = `FALSE`. The interface is immediately updated to show the user a visual confirmation that the booking is now `CANCELLED`.
    * When the user opens the "My Appointments" page. Then the system displays a list of all upcoming bookings with the status of each booking clearly shown.
* **Edge Cases:**
    * *(No explicit edge cases defined in the provided criteria)*

# 3. API Endpoints (Login & Roles)

## 1. Book a Property Visit
* **Endpoint:** POST /api/booking
* **Description:** Allows the user to view the property in person.
* **Headers:** Content-Type: application/json
* **Request Body:**
```json
{
  "propertyId": 20,
  "availability_id": 25
}
```
* **Response (201 Created):**
```json
{
  "message": "Property booked successfully.",
  "booking_id": 123,
  "status": "pending"
}
```
* **Error Responses:**
    * `404 Not Found`: If the `propertyId` does not exist in the database.
    * `400 Bad Request`: If the sent data is incomplete or incorrect.

## 2. Add Property to Favorites
* **Endpoint:** POST /api/favorites
* **Description:** Allows the user to add a specific property to their favorites list to return to it later.
* **Headers:** Content-Type: application/json
* **Request Body:**
```json
{
  "userId": 105,
  "propertyId": 20
}
```
* **Response (201 Created):**
```json
{
  "message": "Property added to favorites successfully.",
  "favoriteId": 350
}
```
* **Error Responses:**
    * `404 Not Found`: If the `propertyId` does not exist in the database.
    * `409 Conflict`: If the user has already added this property to their favorites.
    * `400 Bad Request`: If the sent data is incomplete or incorrect.

# 4. Database Schema (Entities & Attributes)

## 1. Table: `User`
*(No specific attributes provided in the raw text)*

## 2. Table: `Property`
*(No specific attributes provided in the raw text)*

## 3. Table: `Favorite`
* **`favorite_id`** (PK): Identifier for the favorite record.
* **`user_id`** (FK, COMPOSITE UNIQUE INDEX): Identifier for the associated user.
* **`property_id`** (FK, COMPOSITE UNIQUE INDEX): Identifier for the associated property.
* **`created_at`** (DATETIME): The date and time the favorite record was created.

## 4. Table: `Booking_Visit`
* **`booking_id`** (PK): Identifier for the booking record.
* **`user_id`** (FK): Identifier for the user who made the booking.
* **`property_id`** (FK): Identifier for the property being booked.
* **`created_at`** (DATETIME): The date and time the booking was created.
* **`appointment_datetime`** (DATETIME): The scheduled date and time of the appointment.
* **`status`** (ENUM): The current status of the booking (`PENDING`, `CONFIRMED`, `CANCELLED`).
* **`availability_id`** (FK): Identifier for the specific availability slot selected.

## 5. Table: `Property_Availability`
* **`availability_id`** (PK): Identifier for the availability slot.
* **`property_id`** (FK): Identifier for the associated property.
* **`start_time`** (DATETIME): The start time of the availability slot.
* **`end_time`** (DATETIME): The end time of the availability slot.
* **`is_booked`** (BOOLEAN): Status indicating if the slot has been reserved (`TRUE` or `FALSE`).