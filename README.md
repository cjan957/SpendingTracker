# SpendingTracker API


See the API: [SwaggerUI - SpendingTracker](https://spendingtrackerapi.azurewebsites.net/index.html)
<br>
See where the API is used: [SpendingTracker UI](https://github.com/cjan957/SpendingTrackerUI)

2 Main models 
- **TripItem**: contains trip information such as the country, currency, start and end dates
- **SpendingItem**: contains spending entry information such as tripID, category, title, cost, note, timestamp


API 
GET:
- spending - return all spending items regardless of trip
- spending/headings - retrieve all spending item headings regardless of trip
- spending/{id} - return spending item by id
- costbytrip/{id} - return the total expense of the whole trip by trip id
- spendinglistbytrip/{id} - return all spending items by trip id

PUT:
- spending/{id} - update a spending item

POST: 
- spending - create a new spending item

DELETE:
- spending/{id} - delete a specific spending item
