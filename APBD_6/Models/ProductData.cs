namespace APBD_6.Models;

public class ProductData
{
    public long IdProduct { get; set; }
    public long IdWarehouse { get; set; }
    public int Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}