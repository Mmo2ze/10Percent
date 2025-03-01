using SQLite;

namespace _10PercentSys.Models;

[Table("Products")]
public class Product
{
    public Product()
    {
        
    }
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Indexed]
    public int CategoryId { get; set; }

    [Column("prices")]
    public string PricesString { get; set; } = string.Empty;

// In Product.cs
    [Ignore]
    public Dictionary<Size, decimal> Prices => 
        HaveSizes ? PricesString.Split(',')
                .Select(p => p.Split(':'))
                .ToDictionary(p => Enum.Parse<Size>(p[0]), p => decimal.Parse(p[1]))
            : new Dictionary<Size, decimal>();
    public void SetPrices(Dictionary<Size, decimal> prices)
    {
        PricesString = string.Join(",", prices.Select(p => $"{p.Key}:{p.Value}"));
    }

    public static Product Create(string name, int categoryId, Dictionary<Size, decimal> prices)
    {
        return new Product
        {
            HaveSizes = true,
            Name = name,
            CategoryId = categoryId,
            PricesString = string.Join(",", prices.Select(p => $"{p.Key}:{p.Value}"))
        };
    }
    public static Product Create(string name, int categoryId, decimal price)
    {
        return new Product
        {
            HaveSizes = false,
            Name = name,
            CategoryId = categoryId,
            NormalPrice = price
        };
    }
    [Column("have_sizes")]
    

    public bool HaveSizes { get; set; } 
    [Column("normal_price")]
    public decimal NormalPrice { get; set; } = 0; 
}
public enum Size { Small, Medium, Large, None }