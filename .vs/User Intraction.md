### User Stories
* **As a user**, I want to add properties I liked to my favorites list, so I can return to the properties that I liked quickly.
* **As a user**, I want to remove properties from my favorites list.
* **As a user**, I want to be able to book a visit to the property I have chosen, to get a real look at the property.
* **As a user**, I want there to be date options so I can choose a specific date that suits me.
* **As a user**, I want my favorites sorted by Create_At(The most recently saved item is at the top of the list.).
* **As a user**, I want to see a list of upcoming appointments.
* **As a user**, I want to see if my booking is succesfully or not .
* **As a user**, I want to cancel an appointment from my appointment list.
* **As a user**, I want to confirm whether my reservation has been cancelled or not.

---

### Entities & Attributes
* **User**
* **Property**
* **Favorite**: `favorite_id`, `user_id`, `property_id`, `created_at`,(Composite Unique Index on `user_id` & `property_id`).

* **Booking_Visit**: `booking_id`, `user_id`, `property_id`, `created_at`, `appointment_datetime`, `status ENUM('pending', 'confirmed', 'cancelled')` , `availability_id `.

* **Property_Availability** : `availability_id` , `property_id`, `start_time`, `end_time` , `is_booked`

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
  "propertyId": 20,
  "availability_id": 25
}
```

**Expected Responses**:
* **Success (201 Created)**:
  ```json
  {
    "message": "Property booked successfully."
    "booking_id": 123
    "status": "pending"
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