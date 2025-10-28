using System.Threading.Tasks;

namespace FinTrack360.Application.Common.Interfaces;

public interface IDailyAlertJob
{
    Task RunAsync();
}
