using CsvHelper;
using CsvHelper.Configuration;
using FinTrack360.Application.Common.Interfaces;
using System.Globalization;

namespace FinTrack360.Infrastructure.Services;

public class CsvTransactionParser : ITransactionParser
{
    public Task<IEnumerable<ParsedTransaction>> ParseAsync(Stream fileStream)
    {
        using var reader = new StreamReader(fileStream);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        };
        using var csv = new CsvReader(reader, config);

        var records = csv.GetRecords<ParsedTransaction>().ToList();
        return Task.FromResult<IEnumerable<ParsedTransaction>>(records);
    }
}
