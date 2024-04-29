using System.Transactions;
using APBD_6.Models;
using APBD_6.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_6.Controllers;


[ApiController]
[Route("api/product_warehouses")]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;

    public WarehouseController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }
    
    [HttpPut]
    public async Task<IActionResult> CreateProduct(ProductData productWarehouseData)
    {
        var product = await _warehouseService.GetProduct(productWarehouseData.IdProduct);
        if (product == null)
        {
            return NotFound($"Product with id {productWarehouseData.IdProduct} was not found");
        }

        var warehouse = await _warehouseService.GetWarehouse(productWarehouseData.IdWarehouse);
        if (warehouse == null)
        {
            return NotFound($"Warehouse with id {productWarehouseData.IdWarehouse} was not found");
        }
        
        if (productWarehouseData.Amount <= 0)
        {
            return BadRequest("Amount value must be bigger than 0");
        }

        var order = await _warehouseService.GetOrderByAmountAndProductId(productWarehouseData.Amount, productWarehouseData.IdProduct);
        if(order == null)
        {
            return NotFound($"Order with amount: {productWarehouseData.Amount} and product id: {productWarehouseData.IdProduct} does not exist");
        }
        
        if (order.CreatedAt > productWarehouseData.CreatedAt)
        {
            return BadRequest("Product was added before order was created");
        }

        var productWarehouseEntity = await _warehouseService.GetCompletedOrderById(order.IdOrder);
        if (productWarehouseEntity != null)
        {
            return BadRequest("Order already fulfilled");
        }

        try
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await _warehouseService.UpdateFulfilledAt(order.IdOrder, DateTime.Now);
                await _warehouseService.AddProduct(new ProductWarehouse()
                {
                    IdWarehouse = warehouse.IdWarehouse,
                    IdProduct = product.IdProduct,
                    IdOrder = order.IdOrder,
                    Amount = productWarehouseData.Amount,
                    Price = product.Price * productWarehouseData.Amount,
                    CreatedAt = DateTime.Now
                });
                transaction.Complete();
            }
        }catch (TransactionAbortedException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        return Created();
    }
}