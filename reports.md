### User Stories

- **As a user**, I want to select a specific reason for my report from a predefined list (e.g., Fake Listing, Spam, Inappropriate Content), so that my report is clear and categorized.
    
- **As a user**, I want to optionally add a text description to my report, so that I can provide more context or proof about the violation.
    
- **As a user**, I want to receive a success message confirming that my report has been submitted.
    
- **As an admin**, I want to see a list of all submitted reports sorted by `created_at`, so I can review the oldest  issues first.
    
- **As an admin**, I want to update the status of a report (from 'pending' to 'resolved' or 'rejected'), so that I can track which issues have been handled.
    
### Acceptance Criteria

- Given a user is viewing an existing property
When the user submits a report with a valid reason from the predefined list
Then the system should store the report with a 'pending' status
And return a 201 Created response with a success message

- Given a user is attempting to report a property
When the user submits the report without providing a 'reason'
Then the system should reject the request
And return a 400 Bad Request response with a validation error message

- Given a user has already reported a specific property
And the previous report's status is still 'pending' or 'under_review'
When the user attempts to submit another report for the same property
Then the system should prevent the duplication
And return a 409 Conflict response

- Given a user is attempting to report a property
When the user submits a report for a propertyId that does not exist in the database
Then the system should reject the request
And return a 404 Not Found response

### Entities & Attributes

- **User**
    
- **Property**

- **Report**: `report_id`, `user_id`, `property_id`, `reason ENUM('fake_listing', 'spam', 'inappropriate', 'other')`, `description`, `status ENUM('pending', 'under_review', 'resolved', 'rejected')`, `created_at`, (Composite Unique Index on `user_id` & `property_id`).
    

### API Documentation

#### 1. Feature: Submit a Property Report

**Endpoint**:

- **Method**: `POST`
    
- **Path**: `/api/reports`
    
- **Content-Type**: `application/json`
    

**Request Body**:

JSON

```
{
  "reporterId": 105,
  "propertyId": 20,
  "reason": "fake_listing",
  "description": "The images used in this property listing are fake and taken from another website."
}
```

**Expected Responses**:

- **Success (201 Created)**:
    
    JSON
    
    ```
    {
      "message": "Report submitted successfully. Thank you for keeping our platform safe.",
      "report_id": 512,
      "status": "pending"
    }
    ```
    

```
* **Edge Cases**:
    * **404 Not Found**: If the `propertyId` or `reasonId` does not exist in the database.
    * **409 Conflict**: If the user has already reported this specific property and the report is still pending.
    * **400 Bad Request**: If the sent data is incomplete (missing `reasonId`).

---
