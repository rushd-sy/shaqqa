**User Story:**
- **As a user** (Customer - property owner) **I want** to add a property with its property details and verified ownership documents **so that** I can publish verifiable advertisement on the program to potential customers.
- **As a user** (Broker), **I want** to publish propertise without ownership documents, **so that** I can publish all properties advertisements that available from my real estate office.

**Acceptance Criteria:**
- The user (property owner or Broker) and all interested customers can see the property.

**Edge Cases:**
1. Public infrastructure (such as hospitals, schools, mosques or government sites) as a selected location should be ignored.  
2. Outside Syria borders as the selected location should be ignored.
3. The user does not possess a document confirming ownership of the property, if anyone can add a property, then the program will face issues with scammers.
4. Fake property prices, like low prices to attract more people on the program, or high prices to show it as valuable.  
5. Add images for another building or location.
6. Fake phone numbers as contact information.
7. Fake property details.

**API Structure:**
1. Post method.  
2. HTTP status codes:   
   1. 201 Created (property added successfully) \-\> return user to the newly created advertisement page.
   2. 400 Bad Requests (syntax, validation or edge case problem) \-\> clarify the reason (location outside Syria).
3. Request body:  
   1. User ID.
   2. Property details.
   3. Contact information.
   4. Attached files for:
      1. images.
      2. ownership document.
4. Response body:
   1. 201 Created
      1. Advertisement ID.
   2. 400 Bad Requests:
      1. Code.
      2. Detail (the reason).
5. Data Entities:
   1. Advertisement.
   2. User.
   3. Property.
6. Attributes:
   1. Advertisement:
      1. AdvertisementID.
      2. UserID.
      3. PropertyID.
      4. PublishDate.
      5. ContactInfo.
      6. IsAvailable (bool).
   2. User:
      1. UserID.
      2. PhoneNumber.
      3. JoinDate.
      4. PropertyCounter.
   3. Property:
      1. PropertyID.
      2. PropertyDetails (by Saleh).
7. API Endpoints:
   1. /advertisements