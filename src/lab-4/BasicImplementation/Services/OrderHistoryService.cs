using BasicImplementation.Models;
using BasicImplementation.Repositories;
using System.Transactions;

namespace BasicImplementation.Services;

public class OrderHistoryService : IOrderHistoryService
{
    private readonly IOrderHistoryRepository _orderHistoryRepository;

    public OrderHistoryService(IOrderHistoryRepository orderHistoryRepository)
    {
        _orderHistoryRepository = orderHistoryRepository;
    }

    public IAsyncEnumerable<OrderHistory> GetHistoryByFilterAsync(
        int cursor,
        int pageSize,
        OrderHistoryFilterQuery orderHistoryFilterQuery,
        CancellationToken cancellationToken)
    {
        using var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        IAsyncEnumerable<OrderHistory> result = _orderHistoryRepository.GetByFilterAsync(
            cursor,
            pageSize,
            orderHistoryFilterQuery,
            cancellationToken);

        transaction.Complete();

        return result;
    }

    public async Task CreateAsync(OrderHistory orderHistory, CancellationToken cancellationToken)
    {
        using var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        await _orderHistoryRepository.CreateAsync(orderHistory, cancellationToken);

        transaction.Complete();
    }
}