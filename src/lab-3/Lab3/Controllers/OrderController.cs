using Lab3.Models;
using Microsoft.AspNetCore.Mvc;
using Task3.Models;
using Task3.Services;

namespace Lab3.Controllers;

[ApiController]
[Route("orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
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
        [FromBody] Order order,
        CancellationToken cancellationToken)
    {
        long id = await _orderService.CreateAsync(order, cancellationToken);
        return Ok(id);
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
        AddOrderItemResultType addOrderItemResultType = await _orderService.AddOrderItemAsync(
            new OrderItem(orderId, productId, itemQuantity), cancellationToken);
        if (addOrderItemResultType is AddOrderItemResultType.Success success)
        {
            return Ok(success.OrderItemId);
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
        await _orderService.RemoveOrderItemAsync(orderId, orderItemId, cancellationToken);
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
        await _orderService.TransferToWorkAsync(orderId, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Changes Order status to "Completed"
    /// </summary>
    /// <response code="200">Success</response>
    /// <response code="500">Server error</response>
    [HttpPut("{orderId}/status/complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CompleteOrderAsync(
        [FromRoute] long orderId,
        CancellationToken cancellationToken)
    {
        await _orderService.CompleteOrderAsync(orderId, cancellationToken);
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
        await _orderService.CancelOrderAsync(orderId, cancellationToken);
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
        [FromBody] OrderHistoryFilterQuery orderHistoryFilterQuery,
        CancellationToken cancellationToken)
    {
        IAsyncEnumerable<OrderHistory> orderHistories = _orderService.GetHistoryByFilterAsync(cursor, pageSize, orderHistoryFilterQuery, cancellationToken);
        var historyFilterResponse =
            new HistoryFilterResponse(
                await orderHistories.ToListAsync(cancellationToken));
        return Ok(historyFilterResponse);
    }
}