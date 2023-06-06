using CashFlow.Application.Commands;
using CashFlow.Application.Queries;
using CashFlow.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashFlow.Application.UseCases;

public class GetByAccountIdAndDateRangeUseCase: IRequestHandler<GetByAccountIdAndDateRangeQuery, CommandResponse<GetByAccountIdAndDateRangeQueryResponse>>
{
    public GetByAccountIdAndDateRangeUseCase(ICashFlowRepository repository, ILogger<GetByAccountIdAndDateRangeUseCase> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    private readonly ICashFlowRepository _repository;
    private readonly ILogger<GetByAccountIdAndDateRangeUseCase> _logger;
    
    public async Task<CommandResponse<GetByAccountIdAndDateRangeQueryResponse>> Handle(GetByAccountIdAndDateRangeQuery request, CancellationToken cancellationToken)
    {
       var (responseList, totalItemCount, totalPages )= await _repository.GetByAccountIdAndDateRange(request.AccountId, request.StartDate, request.EndDate,
            request.PageNumber, request.PageSize);

       if (totalItemCount == 0)
       {
           _logger.LogError("Account Id:{request.AccountId}, TotalItems: {totalItemCount}");
           return CommandResponse<GetByAccountIdAndDateRangeQueryResponse>.CreateFail(ErrorCode.CashFlowNotFound,
               "Account Id:{request.AccountId}, TotalItems: {totalItemCount}");
       }

       var response = new GetByAccountIdAndDateRangeQueryResponse
       {
           Content = responseList.Select(aggregate => new GetDailyBalanceQueryResponse
           {
               CurrentBalance = aggregate!.GetBalance(),
               Transactions = aggregate.Transactions!,
               Date = aggregate.Date,
               AccountId = aggregate.AccountId
           }).ToList(),
           TotalItems = totalItemCount,
           TotalPages = totalPages
       };
       
       _logger.LogInformation($"Account Id:{request.AccountId}, TotalItems: {totalItemCount}");
       return CommandResponse<GetByAccountIdAndDateRangeQueryResponse>.CreateSuccess(response);
    }
}