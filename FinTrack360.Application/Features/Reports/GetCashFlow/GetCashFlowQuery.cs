using MediatR;
using System.Collections.Generic;

namespace FinTrack360.Application.Features.Reports.GetCashFlow;

public record GetCashFlowQuery(int Year) : IRequest<IEnumerable<CashFlowDto>>;
