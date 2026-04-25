### User Stories
* **As a user**, I want to add properties I liked to my favorites list, so I can return to the properties that I liked quickly.
* **As a user**, I want to remove properties from my favorites list.
* **As a user**, I want to be able to book a visit to the property I have chosen, to get a real look at the property.
* **As a user**, I want there to be date options so I can choose a specific date that suits me.
* **As a user**, I want my favorites sorted by date.
* **As a user**, I want to see a list of upcoming appointments.
* **As a user**, I want to cancel an appointment from my appointment list.

---

### Entities & Attributes
* **User**
* **Property**
* **Favorite**: `favorite_id`, `user_id`, `property_id`, `created_at`.
* **Booking_Visit**: `booking_id`, `user_id`, `property_id`, `created_at`, `available_appointment`, `status`.

---

### API Documentation

#### 1. Feature: Book a Property Visit
**Description**: Allows the user to view the property in person.
**Endpoint**:
* **Method**: `POST`
* **Path**: `/api/booking`
* **Content-Type**: `application/json`

**Request Body**:
```json
{
  "userId": 105,
  "propertyId": 20,
  "appointmentDate": "2026-05-15"
}
```

**Expected Responses**:
* **Success (201 Created)**:
  ```json
  {
    "message": "Property booked successfully."
  }
  ```
* **Edge Cases**:
    * **404 Not Found**: If the `propertyId` does not exist in the database.
    * **400 Bad Request**: If the sent data is incomplete or incorrect.

---

#### 2. Feature: Add Property to Favorites
**Description**: Allows the user to add a specific property to their favorites list to return to it later.
**Endpoint**:
* **Method**: `POST`
* **Path**: `/api/favorites`
* **Content-Type**: `application/json`

**Request Body**:
```json
{
  "userId": 105,
  "propertyId": 20
}
```

**Expected Responses**:
* **Success (201 Created)**:
  ```json
  {
    "message": "Property added to favorites successfully.",
    "favoriteId": 350
  }
  ```
* **Edge Cases**:
    * **404 Not Found**: If the `propertyId` does not exist in the database.
    * **409 Conflict**: If the user has already added this property to their favorites.
    * **400 Bad Request**: If the sent data is incomplete or incorrect.