using Gateway.Mappers;
using Gateway.Models;
using Google.Protobuf.WellKnownTypes;
using Library;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers;

[ApiController]
[Route("orders")]
public class OrderController : ControllerBase
{
    private readonly OrderService.OrderServiceClient _client;

    public OrderController(OrderService.OrderServiceClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Creates new Order
    /// </summary>
    /// <returns>Id of a new Order</returns>
    /// <response code="200">Return Id of a new Order</response>
    /// <response code="500">Server error</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(long))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<long>> CreateAsync(
        [FromBody] OrderDto order,
        CancellationToken cancellationToken)
    {
        CreateOrderReply createOrderReply = await _client.CreateAsync(
            new CreateOrderRequest
            {
                OrderCreatedAt = order.OrderCreatedAt.ToTimestamp(),
                OrderCreatedBy = order.OrderCreatedBy,
            },
            cancellationToken: cancellationToken);
        return Ok(createOrderReply.Id);
    }

    /// <summary>
    /// Creates new OrderItem
    /// </summary>
    /// <returns>Id of a new OrderItem</returns>
    /// <response code="200">Return Id of a new OrderItem</response>
    /// <response code="400">Order status is not "Created"</response>
    /// <response code="500">Server error</response>
    [HttpPut("{orderId}/orderItems")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(long))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<long>> AddOrderItemAsync(
        [FromRoute] long orderId,
        [FromQuery] long productId,
        [FromQuery] int itemQuantity,
        CancellationToken cancellationToken)
    {
        AddOrderItemReply addOrderItemReply = await _client.AddOrderItemAsync(
            new AddOrderItemRequest
                {
                    OrderId = orderId,
                    OrderItemQuantity = itemQuantity,
                    ProductId = productId,
                },
            cancellationToken: cancellationToken);
        if (addOrderItemReply.OrderItemId is not null)
        {
            return Ok(addOrderItemReply.OrderItemId);
        }
        else
        {
            return BadRequest();
        }
    }

    /// <summary>
    /// Removes OrderItem
    /// </summary>
    /// <response code="200">Success</response>
    /// <response code="500">Server error</response>
    [HttpDelete("{orderId}/orderItems")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> RemoveOrderItemAsync(
        [FromRoute] long orderId,
        [FromQuery] long orderItemId,
        CancellationToken cancellationToken)
    {
        await _client.RemoveOrderItemAsync(
            new RemoveOrderItemRequest
            {
                OrderId = orderId,
                OrderItemId = orderItemId,
            },
            cancellationToken: cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Changes Order status to "Processing"
    /// </summary>
    /// <response code="200">Success</response>
    /// <response code="500">Server error</response>
    [HttpPut("{orderId}/status/transfer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> TransferToWorkAsync(
        [FromRoute] long orderId,
        CancellationToken cancellationToken)
    {
        await _client.TransferToWorkAsync(
            new ChangeStateRequest
            {
                Id = orderId,
                OrderChangedStateAt = DateTime.UtcNow.ToTimestamp(),
            },
            cancellationToken: cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Changes Order state to "Cancelled"
    /// </summary>
    /// <response code="200">Success</response>
    /// <response code="500">Server error</response>
    [HttpPut("{orderId}/status/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CancelOrderAsync(
        [FromRoute] long orderId,
        CancellationToken cancellationToken)
    {
        await _client.CancelOrderAsync(
            new ChangeStateRequest
            {
                Id = orderId,
                OrderChangedStateAt = DateTime.UtcNow.ToTimestamp(),
            },
            cancellationToken: cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Filters and returns History items by conditions
    /// </summary>
    /// <returns>History items by conditions</returns>
    /// <response code="200">Returns History items by conditions</response>
    /// <response code="500">Server error</response>
    [HttpGet("history")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HistoryFilterResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HistoryFilterResponse>> GetHistoryByFilterAsync(
        [FromQuery] int cursor,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken,
        [FromQuery] long? orderId = null,
        [FromQuery] string? orderHistoryItemKind = null)
    {
        GetHistoryByFilterReply orderHistories = await _client.GetHistoryByFilterAsync(
            new GetHistoryByFilterRequest
            {
                Cursor = cursor,
                PageSize = pageSize,
                OrderId = orderId,
                OrderHistoryItemKind = OrderHistoryItemKindMapper.MapToLibrary(orderHistoryItemKind),
            },
            cancellationToken: cancellationToken);
        var historyFilterResponse =
            new HistoryFilterResponse(orderHistories.OrderHistories.Select(OrderHistoryMapper.Map));
        return Ok(historyFilterResponse);
    }
}