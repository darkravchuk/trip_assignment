using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Test_Assesment_DevelopsToday.Enums;

namespace Test_Assesment_DevelopsToday.Converters;

public class StoreAndFwdFlagConverter : DefaultTypeConverter
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return StoreAndFwdFlag.N;
        }

        if (text == "N")
            return StoreAndFwdFlag.N;
        if (text == "Y")
            return StoreAndFwdFlag.Y;

        throw new ArgumentException($"Invalid value for store_and_fwd_flag: '{text}'.");
    }
}