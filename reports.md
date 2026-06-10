### User Stories

- **As a user**, I want to select a specific reason for my report from a predefined list (e.g., Fake Listing, Spam, Inappropriate Content), so that my report is clear and categorized.
    
- **As a user**, I want to optionally add a text description to my report, so that I can provide more context or proof about the violation.
    
- **As a user**, I want to receive a success message confirming that my report has been submitted.
    
- **As an admin**, I want to see a list of all submitted reports sorted by `created_at`, so I can review the oldest  issues first.
    
- **As an admin**, I want to update the status of a report (from 'pending' to 'resolved' or 'rejected'), so that I can track which issues have been handled.
    

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
