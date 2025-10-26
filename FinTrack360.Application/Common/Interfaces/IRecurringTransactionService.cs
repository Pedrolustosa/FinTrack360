using System.Threading.Tasks;

namespace FinTrack360.Application.Common.Interfaces;

public interface IRecurringTransactionService
{
    Task ProcessRecurringTransactionsAsync();
}
