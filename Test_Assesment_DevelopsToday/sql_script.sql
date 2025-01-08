CREATE DATABASE Test_Assesment_DevelopsToday;

USE Test_Assesment_DevelopsToday;

CREATE TABLE Trips (
    tpep_pickup_datetime DATETIME NOT NULL,
    tpep_dropoff_datetime DATETIME NOT NULL,
    passenger_count INT NOT NULL,
    trip_distance DECIMAL(10, 2) NOT NULL,
    store_and_fwd_flag VARCHAR(3) NOT NULL,
    PULocationID INT NOT NULL,
    DOLocationID INT NOT NULL,
    fare_amount DECIMAL(10, 2) NOT NULL,
    tip_amount DECIMAL(10, 2) NOT NULL,
    trip_duration AS DATEDIFF(SECOND, tpep_pickup_datetime, tpep_dropoff_datetime) PERSISTED
);

CREATE INDEX idx_pulocationid ON Trips (PULocationID);
CREATE INDEX idx_trip_distance ON Trips (trip_distance);
CREATE INDEX idx_trip_duration ON Trips (trip_duration);
CREATE INDEX idx_tip_amount ON Trips (tip_amount);