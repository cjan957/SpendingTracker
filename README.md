# SpendingTracker API


See the API: [SwaggerUI - SpendingTracker](https://spendingtrackerapi.azurewebsites.net/index.html)
<br>
See where the API is used: [SpendingTracker UI](https://github.com/cjan957/SpendingTrackerUI)

2 Main models 
- **TripItem**: for trip related information such as the country, currency, start and end dates etc.
```
  public class TripItem
    {
        public int ID { get; set; } //tripID
        public string Country { get; set; }
        public string Currency { get; set; }
        public string Starts { get; set; }
        public string Ends { get; set; }
        public double Budget { get; set; }
    }
```

- **SpendingItem**: for spending entry information such as tripID, category, title, cost, note, timestamp etc.
```
    public class SpendingItem
    {
        public int ID { get; set; }
        public int TripID { get; set; }
        public string Category { get; set; }
        public string Heading { get; set; }
        public double Cost { get; set; }
        public string Currency { get; set; }
        public string Note { get; set; }
        public string ReceiptID { get; set; }
        public string CreatedAt { get; set; }
    }
```

## API 
**GET**:
- spending - return all spending items regardless of trip
- spending/headings - retrieve all spending item headings regardless of trip
- spending/{id} - return spending item by id
- costbytrip/{id} - return the total expense of the whole trip by trip id
- spendinglistbytrip/{id} - return all spending items by trip id

**PUT**:
- spending/{id} - update a spending item

**POST**: 
- spending - create a new spending item

**DELETE**:
- spending/{id} - delete a specific spending item

## Tools/Library/Services
* .NET Core API
* C#
* Azure Web App
* SQLite
* Swagger
* Postman

