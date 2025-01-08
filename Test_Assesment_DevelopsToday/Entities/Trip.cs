using Test_Assesment_DevelopsToday.Enums;

namespace Test_Assesment_DevelopsToday.Entities;

public class Trip
{
    public DateTime tpep_pickup_datetime { get; set; }
    public DateTime tpep_dropoff_datetime { get; set; }
    public int? passenger_count { get; set; }
    public decimal trip_distance { get; set; }
    public StoreAndFwdFlag? store_and_fwd_flag { get; set; }
    public int PULocationID { get; set; }
    public int DOLocationID { get; set; }
    public decimal fare_amount { get; set; }
    public decimal tip_amount { get; set; }
}
