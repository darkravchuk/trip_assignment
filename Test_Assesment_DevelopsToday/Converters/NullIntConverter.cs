using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Test_Assesment_DevelopsToday.Converters;

public class NullIntConverter : DefaultTypeConverter
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        if (int.TryParse(text, out int result))
        {
            return result;
        }

        throw new ArgumentException($"Invalid value for numeric field: '{text}'.");
    }
}