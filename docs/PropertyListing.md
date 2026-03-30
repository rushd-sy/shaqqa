**User Story:**
**As a user** (property owner or broker) **I want** to add a property with its property details **so that** I can publish an advertisement on the program.

**Acceptance Criteria:**  
The user (property owner) and all interested customers can see the property.

**Edge Cases:**
1. Duplicated property with same property details in the program.  
   1. There is a case when there are 2 brokers trying to add the same property, the second will be refused.  
   2. Users can pass this if they made minor edits in the property details (e.g., decrease area by 1).  
2. Public infrastructure (such as hospitals, schools, mosques or government sites) as a selected location should be ignored.  
3. Outside Syria borders as the selected location should be ignored.  
4. The user does not possess a document confirming ownership of the property, if anyone can add a property, then the program will face issues with scammers.  
5. Fake property prices, like low prices to attract more people on the program, or high prices to show it as valuable.  
6. Add images for another building or location.  
7. Fake phone numbers as contact information.  
8. Fake property details.

**API Structure:**
1. Post method.  
2. HTTP status codes:   
   1. 201 Created (property added successfully) \-\> return user to the newly created advertisement page.  
   2. 400 Bad Requests (syntax, validation or edge case problem) \-\> clarify the reason (location outside Syria).  
   3. 409 Conflict (property already exists) \-\> redirect user to advertisement page for that property.  
3. Request body:  
   1. User ID.  
   2. Property details.  
   3. Contact information.  
   4. Attached files for:  
      1. images.  
      2. ownership document.  
4. Response body:  
   1. 201 Created \- 409 Conflict:  
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