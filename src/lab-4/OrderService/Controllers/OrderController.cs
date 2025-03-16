using BasicImplementation.Models;
using BasicImplementation.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Kafka.Models;
using Kafka.Producer;
using Library;
using Orders.Kafka.Contracts;
using OrderService.Mappers;

namespace OrderService.Controllers;

public class OrderController : Library.OrderService.OrderServiceBase
{
    private readonly IOrderService _orderService;
    private readonly IOrderHistoryService _orderHistoryService;
    private readonly IKafkaProducer<OrderCreationKey, OrderCreationValue> _producer;

    public OrderController(
        IOrderService orderService,
        IKafkaProducer<OrderCreationKey,
            OrderCreationValue> producer,
        IOrderHistoryService orderHistoryService)
    {
        _orderService = orderService;
        _producer = producer;
        _orderHistoryService = orderHistoryService;
    }

    public override async Task<CreateOrderReply> Create(CreateOrderRequest request, ServerCallContext context)
    {
        var order = new Order(request.OrderCreatedAt.ToDateTime(), request.OrderCreatedBy);
        var createReply = new CreateOrderReply
        {
            Id = await _orderService.CreateAsync(order, context.CancellationToken),
        };

        await _producer.ProduceAsync(
            new KafkaProducerMessage<OrderCreationKey, OrderCreationValue>(
                new OrderCreationKey
                {
                    OrderId = createReply.Id,
                },
                new OrderCreationValue
                {
                    OrderCreated = new OrderCreationValue.Types.OrderCreated
                    {
                        CreatedAt = order.OrderCreatedAt.ToTimestamp(),
                        OrderId = createReply.Id,
                    },
                }),
            context.CancellationToken);

        return createReply;
    }

    public override async Task<Empty> CancelOrder(ChangeStateRequest request, ServerCallContext context)
    {
        await _orderService.CancelOrderAsync(request.Id, context.CancellationToken);
        return new Empty();
    }

    public override async Task<Empty> TransferToWork(ChangeStateRequest request, ServerCallContext context)
    {
        await _orderService.TransferToWorkAsync(request.Id, context.CancellationToken);
        await _producer.ProduceAsync(
            new KafkaProducerMessage<OrderCreationKey, OrderCreationValue>(
                new OrderCreationKey
                {
                    OrderId = request.Id,
                },
                new OrderCreationValue
                {
                    OrderProcessingStarted = new OrderCreationValue.Types.OrderProcessingStarted
                    {
                        StartedAt = request.OrderChangedStateAt,
                        OrderId = request.Id,
                    },
                }),
            context.CancellationToken);
        return new Empty();
    }

    public override async Task<AddOrderItemReply> AddOrderItem(AddOrderItemRequest request, ServerCallContext context)
    {
        var orderItem = new OrderItem(request.OrderId, request.ProductId, request.OrderItemQuantity);
        var addOrderItemReply = new AddOrderItemReply();
        AddOrderItemResultType addOrderItemResultType =
            await _orderService.AddOrderItemAsync(orderItem, context.CancellationToken);
        if (addOrderItemResultType is AddOrderItemResultType.Success success)
        {
            addOrderItemReply.OrderItemId = success.OrderItemId;
        }

        return addOrderItemReply;
    }

    public override async Task<Empty> RemoveOrderItem(RemoveOrderItemRequest request, ServerCallContext context)
    {
        await _orderService.RemoveOrderItemAsync(request.OrderId, request.OrderItemId, context.CancellationToken);
        return new Empty();
    }

    public override async Task<GetHistoryByFilterReply> GetHistoryByFilter(
        GetHistoryByFilterRequest request,
        ServerCallContext context)
    {
        var getHistoryByFilterReply = new GetHistoryByFilterReply();
        await foreach (BasicImplementation.Models.OrderHistory element in _orderHistoryService.GetHistoryByFilterAsync(
                           request.Cursor,
                           request.PageSize,
                           new OrderHistoryFilterQuery(
                               request.OrderId,
                               OrderHistoryItemKindMapper.Map(request.OrderHistoryItemKind)),
                           context.CancellationToken))
        {
            getHistoryByFilterReply.OrderHistories.Add(OrderHistoryMapper.Map(element));
        }

        return getHistoryByFilterReply;
    }
}