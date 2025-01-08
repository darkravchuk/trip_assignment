using System.Data;
using System.Globalization;
using CsvHelper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Test_Assesment_DevelopsToday.Entities;
using Test_Assesment_DevelopsToday.Enums;
using Test_Assesment_DevelopsToday.Helpers;
using Test_Assesment_DevelopsToday.Helpers.Constants;

var path = Path.Combine(AppContext.BaseDirectory, FilePaths.SampleCabDataFileName);
if (!File.Exists(path))
{
    Console.WriteLine("File not found.");
    return;
}
List<Trip> uniqueRecords = null; 

using (var reader = new StreamReader(path))
using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
{
    var records = csv.GetRecords<Trip>().ToList();
    uniqueRecords = records
        .GroupBy(r => new { r.tpep_pickup_datetime, r.tpep_dropoff_datetime, r.passenger_count })
        .Select(g => g.First())
        .Where(record =>
        {
            if (record.tpep_pickup_datetime >= record.tpep_dropoff_datetime)
            {
                Console.WriteLine("Invalid value: tpep_pickup_datetime must be less than tpep_dropoff_datetime.");
                return false;
            }

            if (record.passenger_count < 0 || record.trip_distance < 0 || record.fare_amount < 0 || record.tip_amount < 0)
            {
                Console.WriteLine("Invalid numeric value");
                return false;
            }

            return true;
        })
        .ToList();
    
    var duplicates = records
        .GroupBy(r => new { r.tpep_pickup_datetime, r.tpep_dropoff_datetime, r.passenger_count })
        .Where(g => g.Count() > 1)
        .SelectMany(g => g)
        .ToList();

    var duplicatesPath = Path.Combine(Environment.CurrentDirectory, FilePaths.DuplicateDataFileName);

    using (var writer = new StreamWriter(duplicatesPath))
    using (var csvDuplicates = new CsvWriter(writer, CultureInfo.InvariantCulture))
    {
        csvDuplicates.WriteRecords(duplicates);
    }

    foreach (var record in uniqueRecords)
    {
        TrimStringProperties(records);
        if (record.store_and_fwd_flag == StoreAndFwdFlag.N)
            record.store_and_fwd_flag = StoreAndFwdFlag.No;
        else if (record.store_and_fwd_flag == StoreAndFwdFlag.Y)
            record.store_and_fwd_flag = StoreAndFwdFlag.Yes;
    }
}

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(ConfigurationConstants.AppSettingsFileName, 
        ConfigurationConstants.IsAppSettingsOptional, 
        ConfigurationConstants.ReloadAppSettingsOnChange)
    .Build();

var connectionString = configuration.GetConnectionString("DefaultConnection");

using (var connection = new SqlConnection(connectionString))
{
    connection.Open();

    using (var bulkCopy = new SqlBulkCopy(connection))
    {
        bulkCopy.DestinationTableName = "Trips2";

        bulkCopy.ColumnMappings.Add("tpep_pickup_datetime", "tpep_pickup_datetime");
        bulkCopy.ColumnMappings.Add("tpep_dropoff_datetime", "tpep_dropoff_datetime");
        bulkCopy.ColumnMappings.Add("passenger_count", "passenger_count");
        bulkCopy.ColumnMappings.Add("trip_distance", "trip_distance");
        bulkCopy.ColumnMappings.Add("store_and_fwd_flag", "store_and_fwd_flag");
        bulkCopy.ColumnMappings.Add("PULocationID", "PULocationID");
        bulkCopy.ColumnMappings.Add("DOLocationID", "DOLocationID");
        bulkCopy.ColumnMappings.Add("fare_amount", "fare_amount");
        bulkCopy.ColumnMappings.Add("tip_amount", "tip_amount");

        var dataTable = new DataTable();
        dataTable.Columns.Add("tpep_pickup_datetime", typeof(DateTime));
        dataTable.Columns.Add("tpep_dropoff_datetime", typeof(DateTime));
        dataTable.Columns.Add("passenger_count", typeof(int));
        dataTable.Columns.Add("trip_distance", typeof(decimal));
        dataTable.Columns.Add("store_and_fwd_flag", typeof(string));
        dataTable.Columns.Add("PULocationID", typeof(int));
        dataTable.Columns.Add("DOLocationID", typeof(int));
        dataTable.Columns.Add("fare_amount", typeof(decimal));
        dataTable.Columns.Add("tip_amount", typeof(decimal));


        foreach (var record in uniqueRecords)
        {
            var passengerCount = record.passenger_count ?? 0;

            var pickupUtc = TimeZoneInfo.ConvertTimeToUtc(record.tpep_pickup_datetime, TimeZoneHelper.EstTimeZone);
            var dropoffUtc = TimeZoneInfo.ConvertTimeToUtc(record.tpep_dropoff_datetime, TimeZoneHelper.EstTimeZone);
        
            dataTable.Rows.Add(
                pickupUtc,
                dropoffUtc,
                passengerCount,
                record.trip_distance,
                record.store_and_fwd_flag.ToString(),
                record.PULocationID,
                record.DOLocationID,
                record.fare_amount,
                record.tip_amount
            );
        }

        bulkCopy.WriteToServer(dataTable);
    }
}

static void TrimStringProperties<T>(T obj)
{
    var properties = typeof(T).GetProperties()
        .Where(p => p.PropertyType == typeof(string));

    foreach (var property in properties)
    {
        var value = (string)property.GetValue(obj); 
        if (value != null)
        {
            property.SetValue(obj, value.Trim()); 
        }
    }
}
