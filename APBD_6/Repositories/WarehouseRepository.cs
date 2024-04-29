using System.Data.SqlClient;
using APBD_6.Models;

namespace APBD_6.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private IConfiguration _configuration;
    
    public WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<Product?> GetProduct(long id)
    {
        await using var con =  new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT IdProduct, Name, Description, Price FROM Product WHERE IdProduct = @Id";
        cmd.Parameters.AddWithValue("@Id", id);
        
        var dr = await cmd.ExecuteReaderAsync();
        
        if (await dr.ReadAsync())
        {
            return new Product
            {
                IdProduct = (int)dr["IdProduct"],
                Name = dr["Name"].ToString(),
                Description = dr["Description"].ToString(),
                Price = Convert.ToDouble(dr["Price"])
            };
        }
        
        return null;
    }

    public async Task<Warehouse?> GetWarehouse(long id)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        con.Open();
        
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT IdWarehouse, Name, Address FROM Warehouse WHERE IdWarehouse = @Id";
        cmd.Parameters.AddWithValue("@Id", id);
        
        var dr = await cmd.ExecuteReaderAsync();
        
        if (await dr.ReadAsync())
        {
            return new Warehouse
            {
                IdWarehouse = (int)dr["IdWarehouse"],
                Name = dr["Name"].ToString(),
                Address = dr["Address"].ToString()
            };
        }
        
        return null;
    }

    public async Task<Order?> GetOrderByAmountAndProductId(int amount, long idProduct)
    {
        await using var con =  new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        
        cmd.CommandText = "SELECT IdOrder, IdProduct, Amount, CreatedAt, FulfilledAt FROM \"Order\" WHERE IdProduct = @Id AND Amount = @Amount";
        cmd.Parameters.AddWithValue("@Id", idProduct);
        cmd.Parameters.AddWithValue("@Amount", amount);
        
        var dr = await cmd.ExecuteReaderAsync();
        
        if (await dr.ReadAsync())
        {
            return new Order()
            {
                IdOrder = (int)dr["IdOrder"],
                IdProduct = (int)dr["IdProduct"],
                Amount = (int)dr["Amount"],
                CreatedAt = (DateTime)dr["CreatedAt"],
                FulfilledAt = dr["FulfilledAt"] == DBNull.Value ? null : (DateTime)dr["FulfilledAt"]
            };
        }
        
        return null;
    }

    public async Task<ProductWarehouse?> GetCompletedOrderById(long orderId)
    {
        await using var con =  new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT IdProductWarehouse, IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt " +
                          "FROM Product_Warehouse " +
                          "WHERE IdOrder = @Id";
        cmd.Parameters.AddWithValue("@Id", orderId);
        
        var dr = await cmd.ExecuteReaderAsync();
        
        if (await dr.ReadAsync())
        {
            return new ProductWarehouse
            {
                IdProductWarehouse = (int)dr["IdProductWarehouse"],
                IdWarehouse = (int)dr["IdWarehouse"],
                IdProduct = (int)dr["IdProduct"],
                IdOrder = (int)dr["IdOrder"],
                Amount = (int)dr["Amount"],
                Price = Convert.ToDouble(dr["Price"]),
                CreatedAt = (DateTime)dr["CreatedAt"]
            };
        }
        
        return null;
    }
    
    public async Task<Order?> UpdateFulfilledAt(int orderId, DateTime dateTime)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();

        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "UPDATE \"Order\" SET FulfilledAt = @time WHERE IdOrder = @Id";
        
        cmd.Parameters.AddWithValue("@time", dateTime);
        cmd.Parameters.AddWithValue("@Id", orderId);

        await cmd.ExecuteNonQueryAsync();
        
        cmd.CommandText = "SELECT IdOrder, IdProduct, Amount, CreatedAt, FulfilledAt  FROM \"Order\" WHERE IdOrder = @Id";
        
        var dr = await cmd.ExecuteReaderAsync();
        
        if (await dr.ReadAsync())
        {
            return new Order
            {
                IdOrder = (int)dr["IdOrder"],
                IdProduct = (int)dr["IdProduct"],
                Amount = (int)dr["Amount"],
                CreatedAt = (DateTime)dr["CreatedAt"],
                FulfilledAt = (DateTime)dr["FulfilledAt"]
            };
        }
        
        return null;
    }
    
    public async Task<ProductWarehouse> AddProduct(ProductWarehouse productWarehouse)
    {
        await using var con =  new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) " +
                          "VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt)";
        cmd.Parameters.AddWithValue("@IdWarehouse", productWarehouse.IdWarehouse);
        cmd.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
        cmd.Parameters.AddWithValue("@IdOrder", productWarehouse.IdOrder);
        cmd.Parameters.AddWithValue("@Amount", productWarehouse.Amount);
        cmd.Parameters.AddWithValue("@Price", productWarehouse.Price);
        cmd.Parameters.AddWithValue("@CreatedAt", productWarehouse.CreatedAt);

        await cmd.ExecuteNonQueryAsync();
        
        cmd.CommandText = "SELECT MAX(IdProductWarehouse) FROM Product_Warehouse";
        
        var dr = await cmd.ExecuteReaderAsync();
        
        if (await dr.ReadAsync())
        {
            return new ProductWarehouse
            {
                IdProductWarehouse = (int)dr[0],
                IdWarehouse = productWarehouse.IdWarehouse,
                IdProduct = productWarehouse.IdProduct,
                IdOrder = productWarehouse.IdOrder,
                Amount = productWarehouse.Amount,
                Price = productWarehouse.Price,
                CreatedAt = productWarehouse.CreatedAt
            };
        }

        return null!;
    }
}