using CsvHelper.Configuration;
using Test_Assesment_DevelopsToday.Converters;
using Test_Assesment_DevelopsToday.Entities;

namespace Test_Assesment_DevelopsToday.Mapping;

public class TripMap : ClassMap<Trip>
{
    public TripMap()
    {
        Map(m => m.tpep_pickup_datetime).Name("tpep_pickup_datetime");
        Map(m => m.tpep_dropoff_datetime).Name("tpep_dropoff_datetime");
        Map(m => m.passenger_count)
            .Name("passenger_count")
            .TypeConverter<NullIntConverter>();
        Map(m => m.store_and_fwd_flag).Name("store_and_fwd_flag").TypeConverter<StoreAndFwdFlagConverter>();
        Map(m => m.PULocationID).Name("PULocationID");
        Map(m => m.DOLocationID).Name("DOLocationID");
        Map(m => m.fare_amount).Name("fare_amount");
        Map(m => m.tip_amount).Name("tip_amount");
    }
}