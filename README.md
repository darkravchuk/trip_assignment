### Number of Rows in the Table After Running the Program

After processing the data from the CSV file and inserting it into the database:

1. **Number of rows after removing duplicates**:  
   After removing duplicates based on the combination of `tpep_pickup_datetime`, `tpep_dropoff_datetime`, and `passenger_count`, there were **29,889** unique records remaining.

2. **Number of rows after validation checks**:  
   After applying additional validation checks (e.g., ensuring `tpep_pickup_datetime` is less than `tpep_dropoff_datetime` and that numeric values are non-negative), **29,772** records were inserted into the database.

---

### Validation Logic

The following validation rules were applied to ensure data integrity:

1. **`tpep_pickup_datetime` must be less than `tpep_dropoff_datetime`**:  
   This rule ensures that the pickup time is always before the dropoff time. Records where `tpep_pickup_datetime` is greater than or equal to `tpep_dropoff_datetime` were considered invalid and excluded from the database.

2. **Numeric values must be non-negative**:  
   The following fields were checked to ensure they contain valid, non-negative values:
   - `passenger_count` (number of passengers)
   - `trip_distance` (distance of the trip)
   - `fare_amount` (fare amount)
   - `tip_amount` (tip amount)  
   Records with negative values in any of these fields were considered invalid and excluded from the database.

---

### Optimizations for Handling a 10GB CSV File

If I knew the program would be used for a 10GB CSV file, I believe the following changes would significantly improve its performance and efficiency:
#### Bulk Insert in Batches  

Inserting data in smaller batches (e.g., 10,000 records at a time) can significantly improve performance when handling a 10GB CSV file. This approach is crucial because loading the entire file into memory at once would quickly exhaust system resources, leading to out-of-memory errors or crashes. By processing smaller batches, we keep memory consumption low and ensure the program runs smoothly. Furthermore, large insert operations can take a long time to complete, which may cause the database connection to time out. Smaller batches also make error handling easier—if an error occurs, we can retry just that batch instead of restarting the entire operation, reducing downtime. 
#### Temporary Index Disabling  

I believe temporarily disabling database indexes during bulk inserts would significantly improve performance. Indexes are great for speeding up queries, but they can slow down inserts, especially with large datasets. By disabling them during the bulk insert process and re-enabling them afterward, we can achieve faster data ingestion.

#### Triggers for Real-Time Calculations 
I think using triggers to automatically calculate and update metrics like the average tip_amount for each PULocationId would be a smart move. This eliminates the need for expensive recalculations every time the query is run.
However, if we create a separate table (PULocationAvgTip) to store pre-calculated metrics like the average tip_amount for each PULocationId, we need to ensure that this table stays consistent with the main Trips table. 
