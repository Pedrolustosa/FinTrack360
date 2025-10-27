using MediatR;

namespace FinTrack360.Application.Features.Dashboard.KPIs.MonthlyCashFlow;

public record GetMonthlyCashFlowQuery(string UserId) : IRequest<CashFlowDto>;
