using _10PercentSys.Services;
using SQLite;

namespace _10PercentSys.Models;
[Table("Categories")]
public class Category
{
    public Category()
    {
    }
    
    [PrimaryKey,AutoIncrement]
    public int Id { get; set; }


    [Column("color")]
    public string Color { get; set; } = "#000000";
    
    public string ColorRgp(int opacity = 100)
    {
        var (r, g, b) = ColorConverter.HexToRgb(Color);
        return $"rgba({r},{g},{b},{opacity/100})";
    }
    
    [Column("name")]
    public string Name { get; set; } = string.Empty;
    [Ignore]
    public List<Product> Products { get; set; } = [];
}