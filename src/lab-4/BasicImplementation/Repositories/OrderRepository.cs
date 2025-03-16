using BasicImplementation.Models;
using Npgsql;
using System.Data;
using System.Runtime.CompilerServices;

namespace BasicImplementation.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly NpgsqlDataSource _npgsqlDataSource;

    public OrderRepository(NpgsqlDataSource npgsqlDataSource)
    {
        _npgsqlDataSource = npgsqlDataSource;
    }

    public async Task<long> CreateAsync(Order order, CancellationToken cancellationToken)
    {
        await using NpgsqlConnection connection = await _npgsqlDataSource.OpenConnectionAsync(cancellationToken);
        const string query = """
                             INSERT INTO orders (
                                                 order_state,
                                                 order_created_at,
                                                 order_created_by
                                                 )
                             VALUES (
                                     @order_state,
                                     @order_created_at,
                                     @order_created_by
                                     )
                             RETURNING order_id;
                             """;

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.Add(new NpgsqlParameter("order_state", order.OrderState));
        command.Parameters.Add(new NpgsqlParameter("order_created_at", order.OrderCreatedAt));
        command.Parameters.Add(new NpgsqlParameter("order_created_by", order.OrderCreatedBy));

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        await reader.ReadAsync(cancellationToken);

        return reader.GetInt64(0);
    }

    public async Task UpdateAsync(Order order, CancellationToken cancellationToken)
    {
        await using NpgsqlConnection connection = await _npgsqlDataSource.OpenConnectionAsync(cancellationToken);
        const string query = """
                             UPDATE orders
                             SET order_state = @order_state,
                                 order_created_at = @order_created_at,
                                 order_created_by = @order_created_by
                             WHERE order_id = @order_id;
                             """;

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.Add(new NpgsqlParameter("order_id", order.Id));
        command.Parameters.Add(new NpgsqlParameter("order_state", order.OrderState));
        command.Parameters.Add(new NpgsqlParameter("order_created_at", order.OrderCreatedAt));
        command.Parameters.Add(new NpgsqlParameter("order_created_by", order.OrderCreatedBy));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async IAsyncEnumerable<Order> GetByFilterAsync(
        int cursor,
        int pageSize,
        OrderFilterQuery orderFilterQuery,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await using NpgsqlConnection connection = await _npgsqlDataSource.OpenConnectionAsync(cancellationToken);
        const string query = """
                             SELECT 
                                 order_id,
                                 order_state,
                                 order_created_at,
                                 order_created_by
                             FROM orders
                             where
                                 (order_id > @cursor)
                                 and (cardinality(@ids) = 0 or order_id = any (@ids))
                                 and (@order_state is null or order_state = @order_state)
                                 and (@order_created_by is null or order_created_by = @order_created_by)
                             ORDER BY order_id
                             limit @page_size;
                             """;

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.Add(new NpgsqlParameter("cursor", cursor));
        command.Parameters.Add(new NpgsqlParameter("page_size", pageSize));
        command.Parameters.Add(new NpgsqlParameter("ids", orderFilterQuery.Ids));
        command.Parameters.Add(new NpgsqlParameter<string?>("order_created_by", orderFilterQuery.Author));
        command.Parameters.Add(new NpgsqlParameter<OrderState?>("order_state", orderFilterQuery.OrderState));

        await using NpgsqlDataReader npgsqlDataReader = await command.ExecuteReaderAsync(cancellationToken);
        while (await npgsqlDataReader.ReadAsync(cancellationToken))
        {
            yield return new Order(
                npgsqlDataReader.GetDateTime("order_created_at"),
                npgsqlDataReader.GetString("order_created_by"),
                await npgsqlDataReader.GetFieldValueAsync<OrderState>("order_state", cancellationToken: cancellationToken),
                npgsqlDataReader.GetInt64("order_id"));
        }
    }

    public async Task<Order> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        await using NpgsqlConnection connection = await _npgsqlDataSource.OpenConnectionAsync(cancellationToken);
        const string query = """
                             SELECT 
                                 order_id,
                                 order_state,
                                 order_created_at,
                                 order_created_by
                             FROM orders
                             where (order_id = @id);
                             """;

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.Add(new NpgsqlParameter("id", id));

        await using NpgsqlDataReader npgsqlDataReader = await command.ExecuteReaderAsync(cancellationToken);
        await npgsqlDataReader.ReadAsync(cancellationToken);

        return new Order(
            npgsqlDataReader.GetDateTime("order_created_at"),
            npgsqlDataReader.GetString("order_created_by"),
            await npgsqlDataReader.GetFieldValueAsync<OrderState>("order_state", cancellationToken: cancellationToken),
            npgsqlDataReader.GetInt64("order_id"));
    }
}