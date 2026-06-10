1. User Stories
User
As a user, I want to save my search criteria so that I can quickly check for updates later without re-entering filters.

As a user, I want to save my recent visits so that I can find anything I visited and then wanted to return to, or that I lost.

2. Acceptance Criteria & Edge Cases
Feature: Recent Visits and Search History
Acceptance Criteria:

There should be a history for each user searching.

The system should save recent visits for each user.

Edge Cases:

Maybe the user could not see all his history.

3. API Endpoints (Login & Roles)
1. Save Recent Visits or Searches
Endpoint: POST /api/v1/recent-visits

Description: save recent visits or searches

Request Body:

JSON
{
  "user_id": "",
  "property_id": ""
}
Response (200 OK Status):

JSON
{
  "message": "Property added to history."
}
2. Get Recent Visits
Endpoint: GET /api/user/viewed-properties?limit=10

Description: select latest 10 visits

Response (200 OK Status):

JSON
{
  "data": [
    {
      "propertyId": "",
      "viewedAt": ""
    }
  ]
}
Error Responses:

400 Bad Request: not found property_id

JSON
{
  "error": "Bad Request",
  "message": "not found property_id",
  "code": "400"
}
401 Unauthorized: You must be logged in to view history.

JSON
{
  "error": "Unauthorized",
  "message": "You must be logged in to view history.",
  "code": 401
}
500 Internal Server Error: An unexpected error occurred on the server. Please try again later.

JSON
{
  "error": "Internal Server Error",
  "message": "An unexpected error occurred on the server. Please try again later.",
  "code": 500
}
4. Database Schema (Entities & Attributes)
1. Table: History
user_id ():

property_id ():

visited_searched_at ():