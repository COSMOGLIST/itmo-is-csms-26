using BasicImplementation.Models;
using BasicImplementation.Models.Payloads;
using Npgsql;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace BasicImplementation.Repositories;

public class OrderHistoryRepository : IOrderHistoryRepository
{
    private readonly NpgsqlDataSource _npgsqlDataSource;

    public OrderHistoryRepository(NpgsqlDataSource npgsqlDataSource)
    {
        _npgsqlDataSource = npgsqlDataSource;
    }

    public async Task<long> CreateAsync(OrderHistory orderHistory, CancellationToken cancellationToken)
    {
        await using NpgsqlConnection connection = await _npgsqlDataSource.OpenConnectionAsync(cancellationToken);
        const string query = """
                             INSERT INTO order_history (
                                                        order_id,
                                                        order_history_item_created_at,
                                                        order_history_item_kind,
                                                        order_history_item_payload
                                                        )
                             VALUES (
                                     @order_id,
                                     @order_history_item_created_at,
                                     @order_history_item_kind,
                                     CAST(@order_history_item_payload AS jsonb)
                                     )
                             RETURNING order_history_item_id;
                             """;

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.Add(new NpgsqlParameter("order_id", orderHistory.OrderId));
        command.Parameters.Add(new NpgsqlParameter("order_history_item_created_at", orderHistory.OrderHistoryItemCreatedAt));
        command.Parameters.Add(new NpgsqlParameter("order_history_item_kind", orderHistory.OrderHistoryItemKind));
        command.Parameters.Add(new NpgsqlParameter("order_history_item_payload", JsonSerializer.Serialize(orderHistory.OrderHistoryItemPayload)));

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        await reader.ReadAsync(cancellationToken);

        return reader.GetInt64(0);
    }

    public async IAsyncEnumerable<OrderHistory> GetByFilterAsync(int cursor, int pageSize, OrderHistoryFilterQuery orderHistoryFilterQuery, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await using NpgsqlConnection connection = await _npgsqlDataSource.OpenConnectionAsync(cancellationToken);
        const string query = """
                             SELECT 
                                 order_history_item_id,
                                 order_id,
                                 order_history_item_created_at,
                                 order_history_item_kind,
                                 order_history_item_payload
                             FROM order_history
                             where
                                 order_history_item_id > @cursor
                                 and (@order_id is null or order_id = @order_id)
                                 and (@order_history_item_kind is null or order_history_item_kind = @order_history_item_kind)
                             ORDER BY order_history_item_id
                             limit @page_size;
                             """;

        await using NpgsqlCommand command = _npgsqlDataSource.CreateCommand(query);
        command.Parameters.Add(new NpgsqlParameter("cursor", cursor));
        command.Parameters.Add(new NpgsqlParameter("page_size", pageSize));
        command.Parameters.Add(new NpgsqlParameter<long?>("order_id", orderHistoryFilterQuery.OrderId));
        command.Parameters.Add(new NpgsqlParameter<OrderHistoryItemKind?>("order_history_item_kind", orderHistoryFilterQuery.OrderHistoryItemKind));

        await using NpgsqlDataReader npgsqlDataReader = await command.ExecuteReaderAsync(cancellationToken);
        while (await npgsqlDataReader.ReadAsync(cancellationToken))
        {
            yield return new OrderHistory(
                npgsqlDataReader.GetInt64("order_id"),
                npgsqlDataReader.GetDateTime("order_history_item_created_at"),
                await npgsqlDataReader.GetFieldValueAsync<OrderHistoryItemKind>("order_history_item_kind", cancellationToken: cancellationToken),
                JsonSerializer.Deserialize<Payload>(npgsqlDataReader.GetString("order_history_item_payload")),
                npgsqlDataReader.GetInt64("order_history_item_id"));
        }
    }
}