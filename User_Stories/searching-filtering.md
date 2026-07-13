1. User Stories
Market Watcher
As a market watcher, I want to sort results so that I can see the latest deals first.

Customer
As a user (customer), I want to filter by (price, location, latest, estate type, area, room count, contract type) so that I can find my interests.

2. Acceptance Criteria \& Edge Cases
Feature: Property Filtering and Sorting
Acceptance Criteria:

The system must record the creation date of each property.

The system must allow users to apply multiple filters simultaneously.

Users should be able to set a minimum and maximum price range.

The system must filter properties based on the selected city or neighborhood.

Users should have an option to sort results by "Latest Added".

The system must filter results by "Estate Type" (APARTMENT, VILLA), "Room Count," and "Area".

Users must be able to switch between RENT and SALE.

A NOTHING message should appear if the user searches for something that does not exist.

There should be an option to clear all filters.

Edge Cases:

If the user searches for a location that does not exist => returns a 200 OK with an empty array: { "data": \[] }.

If the user enters negative values => an INVALID INPUT message should appear.

If the user enters an inappropriate value for the field, such as String instead of Int => an INVALID INPUT message should appear.

3. API Endpoints (Login \& Roles)
4. Filter and Sort Properties
Endpoint: GET /api/v1/properties

Description: Filter by (price, area, location, latest, estate type, contract type, room count)

Query Parameters:

minPrice (decimal)

maxPrice (decimal)

location (string)

minRooms (int)

minArea (decimal)

maxArea (decimal)

estateType (string)

contractType (string)

Page (int)

PageSize (int)

sortBy (string, values: price, area, latest)

sortOrder (string, values: asc, desc)

Response (200 OK Status):

JSON
{
"totalItems": 45,
"totalPages": 5,
"currentPage": 1,
"pageSize": 10,
"data": \[
{
"id": "..."
},
{
"id": "..."
}
]
}
Error Responses:

400 Bad Request: If the input data is missing required fields or is invalid.

JSON
{
"error": "Invalid Input",
"message": "Minimum price cannot be less than zero.",
"code": 400
}
500 Internal Server Error: If a database connection failure occurs.

4. Database Schema (Entities \& Attributes)
5. Table: User
6. Table: Property

