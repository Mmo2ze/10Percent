using _10PercentSys.Components.Pages;
using _10PercentSys.Models;
using SQLite;

namespace _10PercentSys.Models;
[Table("order_products")]
public class OrderProduct
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int OrderId { get; set; }

    [Indexed]
    public int ProductId { get; set; } 

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("price")]
    public decimal Price { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;
    public string NameWithSize => Name + StringSize;
    public string StringSize => Size switch
    {
        Size.Large => " L",
        Size.Medium => " M",
        Size.Small => " S",
        
        _ => ""
    };

    [Column("size")]
    public Size Size { get; set; }

    public decimal Total => Price * Quantity;

    public static List<OrderProduct> Create(List<OrderItem> items, int orderId)
    {
        return items.Select(item => new OrderProduct
            {
                OrderId = orderId,
                ProductId = item.Product.Id,
                Quantity = item.Quantity,
                Price = item.Price,
                Name = item.Product.Name,
                Size = item.Size
            })
            .ToList();
    }
    public ProductSummary Summary =>  new(ProductId, NameWithSize);
    
}

public class OrderItem
{
    public string StringSize => Size switch
    {
        Size.Large => " L",
        Size.Medium => " M",
        Size.Small => " S",
        
        _ => ""
    };
    public Product Product { get; set; }
    public Size Size { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal SubTotal => Price * Quantity;
}