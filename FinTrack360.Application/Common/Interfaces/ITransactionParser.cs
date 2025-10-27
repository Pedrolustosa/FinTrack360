using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FinTrack360.Application.Common.Interfaces;

public record ParsedTransaction(DateTime Date, string Description, decimal Amount);

public interface ITransactionParser
{
    Task<IEnumerable<ParsedTransaction>> ParseAsync(Stream fileStream);
}
