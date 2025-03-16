using BasicImplementation.Models;
using Npgsql;
using System.Data;
using System.Runtime.CompilerServices;

namespace BasicImplementation.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly NpgsqlDataSource _npgsqlDataSource;

    public ProductRepository(NpgsqlDataSource npgsqlDataSource)
    {
        _npgsqlDataSource = npgsqlDataSource;
    }

    public async Task<long> CreateAsync(Product product, CancellationToken cancellationToken)
    {
        await using NpgsqlConnection connection = await _npgsqlDataSource.OpenConnectionAsync(cancellationToken);
        const string query = """
                             INSERT INTO products (
                                                   product_name,
                                                   product_price
                                                   )
                             VALUES 
                                 (@name,
                                  @price
                                  )
                             RETURNING product_id;
                             """;

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("name", product.ProductName);
        command.Parameters.AddWithValue("price", product.ProductPrice);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        await reader.ReadAsync(cancellationToken);

        return reader.GetInt64(0);
    }

    public async IAsyncEnumerable<Product> GetByFilterAsync(int pageToken, int pageSize, ProductFilterQuery productFilterQuery, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await using NpgsqlConnection connection = await _npgsqlDataSource.OpenConnectionAsync(cancellationToken);
        const string query = """
                             SELECT 
                                 product_id,
                                 product_name,
                                 product_price
                             FROM products
                             where
                                 (product_id > @cursor)
                                 and (cardinality(@ids) = 0 or product_id = any (@ids))
                                 and (@product_name is null or product_name like @product_name)
                                 and (@min_price is null or product_price > @min_price)
                                 and (@max_price is null or product_price < @max_price)
                             ORDER BY product_id
                             limit @page_size;
                             """;

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.Add(new NpgsqlParameter("cursor", pageToken));
        command.Parameters.Add(new NpgsqlParameter("page_size", pageSize));
        command.Parameters.Add(new NpgsqlParameter("ids", productFilterQuery.Ids));
        command.Parameters.Add(new NpgsqlParameter<string?>("product_name", productFilterQuery.Name));
        command.Parameters.Add(new NpgsqlParameter<decimal?>("min_price", productFilterQuery.MinPrice));
        command.Parameters.Add(new NpgsqlParameter<decimal?>("max_price", productFilterQuery.MaxPrice));

        await using NpgsqlDataReader npgsqlDataReader = await command.ExecuteReaderAsync(cancellationToken);
        while (await npgsqlDataReader.ReadAsync(cancellationToken))
        {
            yield return new Product(
                npgsqlDataReader.GetString("product_name"),
                npgsqlDataReader.GetDecimal("product_price"),
                npgsqlDataReader.GetInt64("product_id"));
        }
    }
}