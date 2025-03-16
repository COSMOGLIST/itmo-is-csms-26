using System.Transactions;
using Task3.Models;
using Task3.Models.Payloads;
using Task3.Repositories;

namespace Task3.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderHistoryRepository _orderHistoryRepository;
    private readonly IOrderItemRepository _orderItemRepository;

    public OrderService(IOrderRepository orderRepository, IOrderHistoryRepository orderHistoryRepository, IOrderItemRepository orderItemRepository)
    {
        _orderRepository = orderRepository;
        _orderHistoryRepository = orderHistoryRepository;
        _orderItemRepository = orderItemRepository;
    }

    public async Task<long> CreateAsync(Order order, CancellationToken cancellationToken)
    {
        using var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);
        long id = await _orderRepository.CreateAsync(order, cancellationToken);
        var orderHistory = new OrderHistory(id, DateTime.Now, OrderHistoryItemKind.Created, new CreatePayload());
        await _orderHistoryRepository.CreateAsync(orderHistory, cancellationToken);
        transaction.Complete();
        return id;
    }

    public async Task<AddOrderItemResultType> AddOrderItemAsync(
        OrderItem orderItem,
        CancellationToken cancellationToken)
    {
        using var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);
        Order order = await _orderRepository.GetByIdAsync(orderItem.OrderId, cancellationToken);
        if (order.OrderState == OrderState.Created)
        {
            long orderItemId = await _orderItemRepository.CreateAsync(orderItem, cancellationToken);
            var orderHistory = new OrderHistory(order.Id, DateTime.Now, OrderHistoryItemKind.ItemAdded, new ItemAddPayload(orderItemId));
            await _orderHistoryRepository.CreateAsync(orderHistory, cancellationToken);
            transaction.Complete();
            return new AddOrderItemResultType.Success(orderItemId);
        }

        transaction.Complete();
        return new AddOrderItemResultType.OrderIsNotCreatedType();
    }

    public async Task RemoveOrderItemAsync(long orderId, long orderItemId, CancellationToken cancellationToken)
    {
        using var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);
        Order order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order.OrderState == OrderState.Created)
        {
            OrderItem orderItem = await _orderItemRepository.GetByIdAsync(orderItemId, cancellationToken);
            orderItem = orderItem with { OrderItemDeleted = true };
            await _orderItemRepository.UpdateAsync(orderItem, cancellationToken);
            var orderHistory = new OrderHistory(orderId, DateTime.Now, OrderHistoryItemKind.ItemRemoved, new ItemRemovePayload(orderItemId));
            await _orderHistoryRepository.CreateAsync(orderHistory, cancellationToken);
        }

        transaction.Complete();
    }

    public async Task TransferToWorkAsync(long id, CancellationToken cancellationToken)
    {
        using var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);
        Order order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order.OrderState == OrderState.Created)
        {
            order = order with { OrderState = OrderState.Processing };
            await _orderRepository.UpdateAsync(order, cancellationToken);
            var orderHistory = new OrderHistory(id, DateTime.Now, OrderHistoryItemKind.StateChanged, new ChangeStatePayload(OrderState.Created, OrderState.Processing));
            await _orderHistoryRepository.CreateAsync(orderHistory, cancellationToken);
        }

        transaction.Complete();
    }

    public async Task CompleteOrderAsync(long id, CancellationToken cancellationToken)
    {
        using var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);
        Order order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order.OrderState == OrderState.Processing)
        {
            order = order with { OrderState = OrderState.Completed };
            await _orderRepository.UpdateAsync(order, cancellationToken);
            var orderHistory = new OrderHistory(id, DateTime.Now, OrderHistoryItemKind.StateChanged, new ChangeStatePayload(OrderState.Processing, OrderState.Completed));
            await _orderHistoryRepository.CreateAsync(orderHistory, cancellationToken);
        }

        transaction.Complete();
    }

    public async Task CancelOrderAsync(long id, CancellationToken cancellationToken)
    {
        using var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);
        Order order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order.OrderState != OrderState.Completed)
        {
            OrderState previousOrderState = order.OrderState;
            order = order with { OrderState = OrderState.Cancelled };
            await _orderRepository.UpdateAsync(order, cancellationToken);
            var orderHistory = new OrderHistory(id, DateTime.Now, OrderHistoryItemKind.StateChanged, new ChangeStatePayload(previousOrderState, OrderState.Cancelled));
            await _orderHistoryRepository.CreateAsync(orderHistory, cancellationToken);
        }

        transaction.Complete();
    }

    public IAsyncEnumerable<OrderHistory> GetHistoryByFilterAsync(int cursor, int pageSize, OrderHistoryFilterQuery orderHistoryFilterQuery, CancellationToken cancellationToken)
    {
        using var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);
        IAsyncEnumerable<OrderHistory> result = _orderHistoryRepository.GetByFilterAsync(cursor, pageSize, orderHistoryFilterQuery, cancellationToken);
        transaction.Complete();
        return result;
    }
}