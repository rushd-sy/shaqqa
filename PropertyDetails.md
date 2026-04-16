\# Property Details (Technical specs, media/images, and descriptions)

\# User Stories

1. As a buyer, I want to view detailed technical specifications (area, rooms, bathrooms, floor ……….) so that I can evaluate if the property fits my needs.
2. As a buyer, I want to see high-quality images of the property so that I can visually assess its condition.
3. As a buyer, I want to view videos or virtual tours so that I can better understand the layout and space.
4. As a buyer, I want to read a clear property description so that I can understand its features and advantages.
5. As a buyer, I want to know additional details ( furnishing status, age of property, amenities) so that I can make an informed decision.
6. As a seller, I want to add technical specifications (area, rooms, bathrooms, etc.) so that buyers can understand the property details.
7. As a seller, I want to upload multiple images for my property so that I can showcase it effectively.
8. As a seller, I want to upload videos or virtual tours so that I can attract more buyers.
9. As a seller, I want to write and edit a property description so that I can highlight key selling points.
10. As a seller, I want to update or delete media (images/videos) so that I can keep my listing accurate.

11. As an Admin, I want to flag/hide media that violates community guidelines (e.g., blurred images, contact info in photos) to maintain platform quality.
12. As a seller, I want to save my progress as a draft so I can finish adding technical specs later.



\---

**Acceptance Criteria:**

**A-Technical specs**

* Location is returned in API response in valid format (lat, long \& structured address)

Like =>

"location": {

"latitude": number between -90 \& 90 ,

"longitude": number between -180 \& 180 ,

"address": "Aleppo, Syria"

}



* Area is returned in API response with explicit unit (value > 0 and unit must be valid enum)
{
  "area": {
    "value": 120,     / number
    "unit": "Hectare" / enum AreaUnit {SqM , Hectare .....}
}
* Price is returned in API response in valid format (price > 0)
* Floor number is returned in API response in valid format (if property is apartment)
* Legal status is returned in API response in valid format
* (enum LegalStatus {Freehold,SharedOwnership,.....})
* Number of rooms is returned in API response in valid format (rooms > 0)
* (if property is apartment or Villa)
* Building construction date is returned in API response in valid format
* (date <= DateTime.Now) (if property is not Land)

**B-images and media**

* At least 1 image is required for active listing
* Media supports images and videos(enum MediaType { Image, Video })
* One media item must be marked as cover (IsCover = true) and is returned first in the response.
* Remaining media items are sorted by DisplayOrder in ascending order (If not specified) 
first image is used as default cover
* Max media count defined (Max = 50)
* Images are compressed upon upload (target size: < 500KB).
* Url video should be Valid (youtube url)

**C-property description**

* Property description is not empty and is displayed.
* Property description includes roof condition information(if apartment or villa)



**Edge Cases**

* Location is missing

=>API returns 400 Bad Request  { "message": "location is required" }

* Location must include valid latitude (-90 to 90) and longitude (-180 to 180)”

=> 400 Bad Request { "message": "Invalid location format" }

* Area is missing \& area <= 0

=>API returns 400 Bad Request  { "message": "area is required and should be greater than zero" }

* Price is missing \&Price <= 0

=>API returns 400 Bad Request  {“price is required and should be greater than zero”}

* Number of rooms is missing for (apartment/villa) \& rooms <= 0

=>API returns 400 Bad Request  {“number of rooms is required and should be greater than zero”}

* Legal Status is missing or  invalid enum value.

=>API returns 400 Bad Request  { "message": "legal status is required and must be a valid enum value" }

* construction date is missing or invalid date time.

=>API returns 400 Bad Request  { "message": "construction date is required and must be a valid date less than or equal to the current date" }

* Property description is missing

* Property exists but is inactive/deleted

=>API returns 404 Not found



****
* No property images available

=> 400 Bad Request{ "At least one image is required"}

* Large number of images (images > 50)
=> 400 Bad Request { "message": "Maximum number of media files is 50" }

* Invalid images URLs

=> 400 Bad Request{ "Invalid media URL format"}

****

* Invalid construction date (future date)

=> 400 Bad Request{ "Construction date cannot be in the future"}

* Invalid location format

=> 400 Bad Request{ "Invalid location format "}



**Data Entities \& Attributes:**
**Property

PropertyId
PropertyType enum { “Apartment” , ”Land” , ”Shop” , ”Villa” , ”Office” ….}
Description
Price
Area (class Area)
-  value decimal 
-  unit  enum:{sqm , hectare , ...}

NumberOfRooms
FloorNumber
Location (class location)
Location:
- Latitude: double  (-90 to 90)
- Longitude: double (-180 to 180)
- Address: string   (e.g. "Aleppo, Syria")
- City: string
- Country: string

LegalStatus enum {Leasehold,Freehold,CourtRegistered,SharedOwnership…….}
ConstructionDate

**User

UserId
Name
PhoneNumber
Email
UserRole enum{“Seller” , “Buyer”}

**Media

MediaId (PK) — int
PropertyId (FK) — int
Type — enum (Image, Video)
Url — string
ThumbnailUrl — string (nullable, required for video)
IsCover — bool
DisplayOrder — int

**Amenities

AmenityId (PK) — int
Name — string
Description — string (nullable)

**PropertyAmenities 

PropertyId (FK) — int
AmenityId (FK) — int

**API Endpoints:**
**Path /Properties**
**Http Status Code Request & Response operation**

POST   /Properties          
→ 201 Created   
→ 400 Bad Request 
- Missing required fields (price, area, location, description)
- Invalid values (price <= 0, area <= 0)
- Invalid location format

GET    /Properties
200 OK (list)     

GET    /Properties/{id}     
200 OK          → returns full property details (technical specs, media, description)
404 Not Found   → property does not exist or is inactive/deleted
400 Bad Request → invalid property id (e.g., negative or zero)

PATCH  /Properties/{id}     

200 OK          → property updated successfully
400 Bad Request →
- Invalid values (price <= 0, invalid location, etc.)
- Invalid enum values (PropertyType, LegalStatus)
404 Not Found   → property does not exist

DELETE /Properties/{id}     
204 No Content
404 Not Found  → property does not exist


POST   /Properties/{id}/Media 
201 Created     → media added successfully
400 Bad Request →
- Invalid media URL
- Unsupported media type
- Media count exceeds limit (max 50)
- No image provided for active property
404 Not Found  → property does not exist

DELETE /Properties/{id}/Media/{mediaId} 
204 No Content → media deleted successfully
404 Not Found  →
- property does not exist
- media does not exist