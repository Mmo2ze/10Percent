using _10PercentSys.Models;
using SQLite;

[Table("orders")]
public class Order
{
    [PrimaryKey, AutoIncrement]
    [Column("id")]
    public int Id { get; set; }

    [Column("date")]
    public DateTime Date { get; set; } = DateTime.Now;

    [Column("vat14")]
    public bool Vat14 { get; set; }

    [Column("vat10")]
    public bool Vat10 { get; set; }

    [Column("discount")]
    public int Discount { get;  set; }

    [Ignore] public List<OrderProduct> OrderProducts { get; set; } = [];


    public Order() { }

    private decimal Multiplier => (Vat14 && Vat10 ? 1.24m : Vat14 ? 1.14m : Vat10 ? 1.10m : 1m) - Discount / 100m;
    public decimal Total => OrderProducts.Sum(p => p.Total) * Multiplier;
    public bool HaveDiscount => Discount > 0;
    public decimal DiscountAmount => Total * Discount / 100m;
    public decimal TaxRate => Multiplier*100 - 100;
    public decimal TaxAmount => Total * (TaxRate / 100);

    public static Order Create(bool placeTax, bool orgTax)
    {
        return new Order
        {
            Vat10 = placeTax,
            Vat14 = orgTax,
        };
    }
}