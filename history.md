User Story & Acceptance Criteria :

    //As a user, I want to save my search criteria so that I can quickly check for updates later without re-entering filters.
        /There should be a history for each user searching.
    //As a user, I want to save my recent visits so that I can find anything I visited and then wanted to return to, or that I lost.
        /The system should save recent visits for each user 

Edge Cases :
    //Maybe the user could not see all his history 



Data Entities :

    history[user_id,
            property_id,
            visited_searched_at
            ]

API Structure :

    Endpoint : POST  api/v1/recent-visits

    Description : save recent visits or searches

    Request Body :
        {
            "user_id" : ""
            "property_id" : ""
        }

    Success Response (200 OK):

        {
            "message": "Property added to history."
        }

    Endpoint : GET /api/user/viewed-properties?limit=10
    
    Description : select latest 10 visits 

    Success Response (200 OK):

        {
            "data": [
                {
                    "propertyId": "",
                    "viewedAt": ""
                }
            ]
        }

    401 (Unauthorized):
        {
            "error": "Unauthorized",
            "message": "You must be logged in to view history.",
            "code": 401
        }

    400 (Bad Request):
        {
            "error": "Bad Request",
            ""message": "not found property_id",
            "code":"400"
        }
    
    500 (Internal Server Error):
        {
            "error": "Internal Server Error",
            "message": "An unexpected error occurred on the server. Please try again later.",
            "code": 500
        }

    


















