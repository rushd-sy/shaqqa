\# Property Details (Technical specs, media/images, and descriptions)



\## User Stories



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



\---



\## Acceptance Criteria



\### A - Technical specs



Location is returned in API response in valid format (lat, long \& structured address)



Like =>



```json

"location": {

&#x20; "latitude": number between -90 \& 90,

&#x20; "longitude": number between -180 \& 180,

&#x20; "address": "Aleppo, Syria"

}





**Acceptance Criteria:**

**A-Technical specs**

* Location is returned in API response in valid format (lat, long \& structured address)

Like =>

"location": {

"latitude": number between -90 \& 90 ,

"longitude": number between -180 \& 180 ,

"address": "Aleppo, Syria"

}



* Area is returned in API response in valid format (area > 0)
* Price is returned in API response in valid format (price > 0)
* Floor number is returned in API response in valid format (if property is apartment)
* Legal status is returned in API response in valid format
* (enum LegalState {Freehold,SharedOwnership,.....})
* Number of rooms is returned in API response in valid format (rooms > 0)
* (if property is apartment or Villa)
* Building construction date is returned in API response in valid format
* (date <= DateTime.Now) (if property is not Land)

**B-images and media**

* At least 1 image is required for active listing
* Media supports images and videos(enum MediaType { Image, Video })
* One image must be marked as cover (IsCover = true)
* If not specified, first image is used as default cover
* Max media count defined (Max = 50)

**C-property description**

* Property description is not empty and is displayed.
* Property description includes roof condition information(if apartment or villa)




**Edge Cases**

* Location is missing

=>API returns 400 Bad Request  {“location is required”}



* Location must include valid latitude (-90 to 90) and longitude (-180 to 180)”

=> 400 Bad Request { "message": "Invalid location format" }



* Area is missing \& area <= 0       

=>API returns 400 Bad Request  {“area is required and should be greater than zero”}



* Price is missing \&Price <= 0

=>API returns 400 Bad Request  {“price is required and should be greater than zero”}



* Number of rooms is missing for (apartment/villa) \& rooms <= 0 

=>API returns 400 Bad Request  {“number of rooms is required and should be greater than zero”}



* Legal Status is missing or  invalid enum value.

=>API returns 400 Bad Request  {“......”}



* construction date is missing or invalid date time.

=>API returns 400 Bad Request  {“......”}



* Property description is missing



* Property exists but is inactive/deleted 

=>API returns 404 Not found  


\*\*\*\*\*


* No property images available

=> 400 Bad Request{ "At least one image is required"}


* Large number of images(images<50)

=> 400 Bad Request{ "Maximum number of media files is 50"}


* Invalid images URLs 

=> 400 Bad Request{ "Invalid media URL format"}


\*\*\*\*\*


* Invalid construction date (future date)

=> 400 Bad Request{ "Construction date cannot be in the future"}


* Invalid location format

=> 400 Bad Request{ "Invalid location format ……."}







**Data Entities \& Attributes:**

Property

\- PropertyId

\- PropertyType enum { “Apartment” , ”Land” , ”Shop” , ”Villa” , ”Office” ….}

\- Description

\- Price

\- Area

\- NumberOfRooms

\- FloorNumber

\- Location (class location)

\- LegalStatus enum {Leasehold,Freehold,CourtRegistered,SharedOwnership…….}

\- ConstructionDate

User

\- UserId

\- Name

\- PhoneNumber

\- Email

\- UserRole enum{“Seller” , “Buyer”}

Media

\- MediaId

\- PropertyId

\- Url

\- Type (image/video)



**API Endpoints:**

**Path (Api EndPoint) /Properties**

**Get Method** 

* /Properties                  => all Properties
* /Properties/{id}           => specific Property
* /Properties/{id}/Media => Media to specific Property


**Http Status Code Request \& Response operation**

* Request: /Properties/{id} => Response: 200 ok (The page was displayed successfully) and json body with property details {........}
* Request: /Properties/{id} => Response: 404 Not found (this Property id is not existed) and json body with message {"message": "Property not found"}
* Request: /Properties/{id} => Response: 400 Bad request (this Property id is negative) and json body with message {  "message": "Invalid property id"}



