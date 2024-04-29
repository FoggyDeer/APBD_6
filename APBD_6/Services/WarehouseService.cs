using APBD_6.Models;
using APBD_6.Repositories;

namespace APBD_6.Services;

public class WarehouseService : IWarehouseService
{
    private readonly IWarehouseRepository _warehouseRepository;

    public WarehouseService(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }

    public async Task<Product?> GetProduct(long id)
    {
        return await _warehouseRepository.GetProduct(id);
    }

    public async Task<Warehouse?> GetWarehouse(long id)
    {
        return await _warehouseRepository.GetWarehouse(id);
    }

    public async Task<Order?> GetOrderByAmountAndProductId(int amount, long idProduct)
    {
        return await _warehouseRepository.GetOrderByAmountAndProductId(amount, idProduct);
    }

    public async Task<ProductWarehouse?> GetCompletedOrderById(long orderId)
    {
        return await _warehouseRepository.GetCompletedOrderById(orderId);
    }

    public async Task<Order?> UpdateFulfilledAt(int orderId, DateTime dateTime)
    {
        return await _warehouseRepository.UpdateFulfilledAt(orderId, dateTime);
    }

    public async Task<ProductWarehouse> AddProduct(ProductWarehouse productWarehouse)
    {
        return await _warehouseRepository.AddProduct(new ProductWarehouse()
        {
            IdWarehouse = productWarehouse.IdWarehouse,
            IdProduct = productWarehouse.IdProduct,
            IdOrder = productWarehouse.IdOrder,
            Amount = productWarehouse.Amount,
            Price = productWarehouse.Price * productWarehouse.Amount,
            CreatedAt = productWarehouse.CreatedAt
        });
    }
}