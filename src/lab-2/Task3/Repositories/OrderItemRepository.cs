using Npgsql;
using System.Data;
using System.Runtime.CompilerServices;
using Task3.Models;

namespace Task3.Repositories;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly NpgsqlDataSource _npgsqlDataSource;

    public OrderItemRepository(NpgsqlDataSource npgsqlDataSource)
    {
        _npgsqlDataSource = npgsqlDataSource;
    }

    public async Task<long> CreateAsync(OrderItem orderItem, CancellationToken cancellationToken)
    {
        await using NpgsqlConnection connection = await _npgsqlDataSource.OpenConnectionAsync(cancellationToken);
        const string query = """
                             INSERT INTO order_items (
                                                      order_id,
                                                      product_id,
                                                      order_item_quantity,
                                                      order_item_deleted
                                                      )
                             VALUES (
                                     @order_id,
                                     @product_id,
                                     @order_item_quantity,
                                     @order_item_deleted
                                     )
                             RETURNING order_item_id;
                             """;
        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.Add(new NpgsqlParameter("order_id", orderItem.OrderId));
        command.Parameters.Add(new NpgsqlParameter("product_id", orderItem.ProductId));
        command.Parameters.Add(new NpgsqlParameter("order_item_quantity", orderItem.OrderItemQuantity));
        command.Parameters.Add(new NpgsqlParameter("order_item_deleted", orderItem.OrderItemDeleted));
        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        await reader.ReadAsync(cancellationToken);
        return reader.GetInt64(0);
    }

    public async Task UpdateAsync(OrderItem orderItem, CancellationToken cancellationToken)
    {
        await using NpgsqlConnection connection = await _npgsqlDataSource.OpenConnectionAsync(cancellationToken);
        const string query = """
                             UPDATE order_items
                             SET order_id = @order_id,
                                 product_id = @product_id,
                                 order_item_quantity = @order_item_quantity,
                                 order_item_deleted = @order_item_deleted
                             WHERE order_item_id = @order_item_id;
                             """;
        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.Add(new NpgsqlParameter("order_item_id", orderItem.Id));
        command.Parameters.Add(new NpgsqlParameter("order_id", orderItem.OrderId));
        command.Parameters.Add(new NpgsqlParameter("product_id", orderItem.ProductId));
        command.Parameters.Add(new NpgsqlParameter("order_item_quantity", orderItem.OrderItemQuantity));
        command.Parameters.Add(new NpgsqlParameter("order_item_deleted", orderItem.OrderItemDeleted));
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<OrderItem> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        await using NpgsqlConnection connection = await _npgsqlDataSource.OpenConnectionAsync(cancellationToken);
        const string query = """
                             SELECT 
                                 order_item_id,
                                 order_id,
                                 product_id,
                                 order_item_quantity,
                                 order_item_deleted
                             FROM order_items
                             where (order_item_id = @id);
                             """;
        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.Add(new NpgsqlParameter("id", id));
        await using NpgsqlDataReader npgsqlDataReader = await command.ExecuteReaderAsync(cancellationToken);
        await npgsqlDataReader.ReadAsync(cancellationToken);
        return new OrderItem(
            npgsqlDataReader.GetInt64("order_id"),
            npgsqlDataReader.GetInt64("product_id"),
            npgsqlDataReader.GetInt32("order_item_quantity"),
            npgsqlDataReader.GetBoolean("order_item_deleted"),
            npgsqlDataReader.GetInt64("order_item_id"));
    }

    public async IAsyncEnumerable<OrderItem> GetByFilterAsync(int cursor, int pageSize, OrderItemsFilterQuery orderItemsFilterQuery, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await using NpgsqlConnection connection = await _npgsqlDataSource.OpenConnectionAsync(cancellationToken);
        const string query = """
                             SELECT 
                                 order_item_id,
                                 order_id,
                                 product_id,
                                 order_item_quantity,
                                 order_item_deleted
                             FROM order_items
                             where
                                 (order_item_id > @cursor)
                                 and (cardinality(@order_ids) = 0 or order_id = any (@order_ids))
                                 and (cardinality(@product_ids) = 0 or product_id = any (@product_ids))
                                 and (@order_item_deleted is null or order_item_deleted = @order_item_deleted)
                             ORDER BY order_item_id
                             limit @page_size;
                             """;
        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.Add(new NpgsqlParameter("cursor", cursor));
        command.Parameters.Add(new NpgsqlParameter("page_size", pageSize));
        command.Parameters.Add(new NpgsqlParameter("order_ids", orderItemsFilterQuery.OrderIds));
        command.Parameters.Add(new NpgsqlParameter("product_ids", orderItemsFilterQuery.ProductIds));
        command.Parameters.Add(new NpgsqlParameter<bool?>("order_item_deleted", orderItemsFilterQuery.Deleted));
        await using NpgsqlDataReader npgsqlDataReader = await command.ExecuteReaderAsync(cancellationToken);
        while (await npgsqlDataReader.ReadAsync(cancellationToken))
        {
            yield return new OrderItem(
                npgsqlDataReader.GetInt64("order_id"),
                npgsqlDataReader.GetInt64("product_id"),
                npgsqlDataReader.GetInt32("order_item_quantity"),
                npgsqlDataReader.GetBoolean("order_item_deleted"),
                npgsqlDataReader.GetInt64("order_item_id"));
        }
    }
}