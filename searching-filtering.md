User Story & Acceptance Criteria :

    //As a market watcher, I want to sort results so that I can see the latest deals first.
        /The system must record the history of each property.
    //As a user, I want to save my search criteria so that I can quickly check for updates later without re-entering filters.
        /There should be a history for each user.
    //As a user(customer) , I want to filter by ( price,location,latest,estate type,area,room count, contract type) so that I can find my interests .
        /The system must allow users to apply multiple filters simultaneously
        /Users should be able to set a minimum and maximum price range
        /The system must filter properties based on the selected city or neighborhood
        /Users should have an option to sort results by "Latest Added"
        /The system must filter results by "Estate Type" ( Apartment, Villa), "Room Count," and "Area"
        /Users must be able to switch between "Rent" and "Sale"
        /A "Nothing" message should appear if the user searches for something that does not exist.
        /There should be an option to clear all filters.

Edge Cases :
    /If the user searches for a location that does not exist => a "nothing" message should appear.
    /If the user enters negative values => a "Invalid Input" message should appear.
    /If the user enters an inappropriate value for the field, such as String instead of Int =>a "Invalid Input" message should appear.


Data Entities :

    User (...)
    Property(...)
    history(...)

API Structure :
    Method : GET
    Path : /api/v1/properties/filter
    Description : Filter by ( price, area, location ,latest ,estate type ,contract type , room count )
    Query Parameters :
        minPrice (decimal)
        maxPrice (decimal)
        location (string)
        roomCount (int)
        area (decimal)
        estateType (string)
        contractType (string)

    status code :
        Success Response : 200 (ok)
            {
                "totalItems": 45,
                "totalPages": 5,
                "currentPage": 1,
                "pageSize": 10,
                "data": [
                            { "id": ,... },
                            { "id": ,... }
                        ]
            }
        Error Responses :
            400 : (Bad Request)=>If the input data is missing required fields
                {
                    "error": "Invalid Input",
                    "message": "Minimum price cannot be less than zero.",
                    "code": 400
                }
            500 (Internal Server Error)=>If a database connection failure occurs


