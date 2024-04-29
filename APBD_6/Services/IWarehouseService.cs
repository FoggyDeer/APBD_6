using APBD_6.Models;

namespace APBD_6.Services;

public interface IWarehouseService
{
    
    Task<Product?> GetProduct(long id);
    Task<Warehouse?> GetWarehouse(long id);
    Task<Order?> GetOrderByAmountAndProductId(int amount, long idProduct);
    Task<ProductWarehouse?> GetCompletedOrderById(long orderId);
    Task<Order?> UpdateFulfilledAt(int orderId, DateTime dateTime);
    Task<ProductWarehouse> AddProduct(ProductWarehouse productWarehouse);
}