using Microsoft.AspNetCore.Mvc;
using Orders.ProcessingService.Contracts;

namespace Gateway.Controllers;

[ApiController]
[Route("orders/{orderId}/status")]
public class OrderProcessingController : ControllerBase
{
    private readonly OrderService.OrderServiceClient _client;

    public OrderProcessingController(OrderService.OrderServiceClient client)
    {
        _client = client;
    }

    [HttpPut("approval")]
    public async Task<ActionResult> ApproveOrderAsync(
        [FromRoute] long orderId,
        [FromQuery] bool isApproved,
        [FromQuery] string approvedBy,
        [FromQuery] string? failureReason = null,
        CancellationToken cancellationToken = default)
    {
        await _client.ApproveOrderAsync(
            new ApproveOrderRequest
            {
                OrderId = orderId,
                IsApproved = isApproved,
                ApprovedBy = approvedBy,
                FailureReason = failureReason,
            },
            cancellationToken: cancellationToken);
        return Ok();
    }

    [HttpPut("start-packing")]
    public async Task<ActionResult> StartOrderPackingAsync(
        [FromRoute] long orderId,
        [FromQuery] string packingBy,
        CancellationToken cancellationToken = default)
    {
        await _client.StartOrderPackingAsync(
            new StartOrderPackingRequest
            {
                OrderId = orderId,
                PackingBy = packingBy,
            },
            cancellationToken: cancellationToken);
        return Ok();
    }

    [HttpPut("finish-packing")]
    public async Task<ActionResult> FinishOrderPackingAsync(
        [FromRoute] long orderId,
        [FromQuery] bool isSuccessful,
        [FromQuery] string? failureReason = null,
        CancellationToken cancellationToken = default)
    {
        await _client.FinishOrderPackingAsync(
            new FinishOrderPackingRequest
            {
                OrderId = orderId,
                IsSuccessful = isSuccessful,
                FailureReason = failureReason,
            },
            cancellationToken: cancellationToken);
        return Ok();
    }

    [HttpPut("start-delivery")]
    public async Task<ActionResult> StartOrderDeliveryAsync(
        [FromRoute] long orderId,
        [FromQuery] string deliveredBy,
        CancellationToken cancellationToken = default)
    {
        await _client.StartOrderDeliveryAsync(
            new StartOrderDeliveryRequest
            {
                OrderId = orderId,
                DeliveredBy = deliveredBy,
            },
            cancellationToken: cancellationToken);
        return Ok();
    }

    [HttpPut("finish-delivery")]
    public async Task<ActionResult> FinishOrderDeliveryAsync(
        [FromRoute] long orderId,
        [FromQuery] bool isSuccessful,
        [FromQuery] string? failureReason = null,
        CancellationToken cancellationToken = default)
    {
        await _client.FinishOrderDeliveryAsync(
            new FinishOrderDeliveryRequest
            {
                OrderId = orderId,
                IsSuccessful = isSuccessful,
                FailureReason = failureReason,
            },
            cancellationToken: cancellationToken);
        return Ok();
    }
}