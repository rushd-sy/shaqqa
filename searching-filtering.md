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

Data Entities :

    User 
    Property
    history

API Structure:
    Get method
    HTTP status code:
    200 (OK)
    400 (Bad request)
    404 (Not found)
    500 (Server Error)

