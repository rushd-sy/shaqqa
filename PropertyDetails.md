**Property details (Technical specs, media/images, and descriptions) =>** 

As a buyer, I want to see the property specifications so that I can evaluate the property.

As a seller, I want to publish my property with images and videos so that buyers can view it and contact me.

As a buyer, I want to see the property description so that I can understand all the details about it.



**Acceptance Criteria:**

**A-Technical specs**

1. Location is displayed
2. Area is displayed
3. Price is displayed
4. Floor number is displayed (if property is apartment)
5. Legal status is displayed
6. Number of rooms is displayed (if property is apartment or Villa)
7. Building construction date is displayed (if property is not Land)



**B-images and media**

1. Images are displayed.



**C-property description**

1. Property description is not empty and is displayed.
2. Property description includes roof condition information(if apartment)



**Edge Cases**

Location is missing

Area is missing

Price is missing

Number of rooms is missing 

Property legal status is missing

Construction date is missing

Property description is missing

Property exists but is inactive/deleted

\*\*

No property images available

Large number of images

Invalid images URLs 

\*\*

Negative values for price or area

Invalid construction date (future date)

Number of rooms is zero or negative

Invalid location format









**Data Entities \& Attributes:**

Property

\- PropertyId

\- PropertyType enum { “Apartment” , ”Land” , ”Shop” , ”Villa” , ”Office” ….}

\- Description

\- Price

\- Area

\- NumberOfRooms

\- FloorNumber

\- LegalStatus

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



**API Endpoints/Path**  /Properties

Get Method 

1. /Properties                  => all Properties
2. /Properties/{id}           => specific Property
3. /Properties/{id}/Media => Media to specific Property



**Http Status Code Request \& Response operation**

* Request: /Properties/{id} => Response: 200 ok (The page was displayed successfully) and json body with property details {........}
* Request: /Properties/{id} => Response: 404 Not found (this Property id is not existed) and json body with message {"message": "Property not found"}
* Request: /Properties/{id} => Response: 400 Bad request (this Property id is negative) and json body with message {  "message": "Invalid property id"}



