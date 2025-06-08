using _10PercentSys.Components.Pages;
using _10PercentSys.Models;
using SQLite;

namespace _10PercentSys.Services;

public class LocalDbService
{
    private const string DbName = "10PercentDb.db";
    private static string BackUpPath => Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

    private static string DbPath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DbName);

    private readonly SQLiteAsyncConnection _connection;


    public LocalDbService()
    {
        _connection = new SQLiteAsyncConnection(DbPath);
        _connection.ExecuteAsync("PRAGMA foreign_keys = ON;");
// Add this after creating the connection
        _connection.ExecuteAsync("PRAGMA journal_mode = WAL;");
        _connection.ExecuteAsync("PRAGMA cache_size = 10000;");
        _connection.BackupAsync(BackUpPath, DbName);
        // Initialize tables synchronously to avoid race conditions
        InitializeDatabase();
    }

    private async void InitializeDatabase()
    {
        try
        {
            // Explicitly create tables if they don't exist
            await _connection.CreateTableAsync<Product>();
            await _connection.CreateTableAsync<Order>();
            await _connection.CreateTableAsync<OrderProduct>();
            await _connection.CreateTableAsync<Category>();
            await _connection.DeleteAsync<Order>(GetOrdersAsync());
            await _connection.DeleteAsync<OrderProduct>(GetOrderItemsAsync());
            await _connection.DeleteAsync<Models.GamingRoom>();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task<List<OrderProduct>> GetOrderItemsAsync()
    {
        return await _connection.Table<OrderProduct>().ToListAsync();
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        return await _connection.Table<Product>().ToListAsync();
    }

    public async Task<Product> GetProductAsync(int id)
    {
        return await _connection.Table<Product>().Where(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateProductAsync(Product product)
    {
        await _connection.InsertAsync(product);
    }

    public async Task UpdateProductAsync(Product product)
    {
        await _connection.UpdateAsync(product);
    }

    public async Task DeleteProductAsync(Product product)
    {
        await _connection.DeleteAsync(product);
    }

    public async Task DeleteProductAsync(int id)
    {
        await _connection.DeleteAsync<Product>(id);
    }

    public async Task<List<Order>> GetOrdersAsync()
    {
        return await _connection.Table<Order>().ToListAsync();
    }

    public async Task<Order> GetOrderAsync(int id)
    {
        return await _connection.Table<Order>().Where(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateOrderAsync(Order order)
    {
        await _connection.InsertAsync(order);
    }

    public async Task CreateOrderProductsAsync(List<OrderProduct> orderProducts)
    {
        await _connection.InsertAllAsync(orderProducts);
    }

    public async Task<List<OrderProduct>> GetOrderItemsAsyncByOrderId(int orderId)
    {
        return await _connection.Table<OrderProduct>().Where(p => p.OrderId == orderId).ToListAsync();
    }

// Service method remains the same but now handles exact timestamps
    public async Task<List<Order>> GetOrdersByDateAsync(DateTime startDate, DateTime endDate)
    {
        return await _connection.Table<Order>()
            .Where(o => o.Date >= startDate && o.Date <= endDate)
            .OrderByDescending(o => o.Date)
            .ToListAsync();
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        return await _connection.Table<Category>().ToListAsync();
    }

    public async Task DeleteCategoryAsync(int categoryId)
    {
        await _connection.DeleteAsync<Category>(categoryId);
    }

    public async Task UpdateCategoryAsync(Category currentCategory)
    {
        await _connection.UpdateAsync(currentCategory);
    }

    public async Task CreateCategoryAsync(Category category)
    {
        await _connection.InsertAsync(category);
    }

    public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        return await _connection.Table<Product>().Where(p => p.CategoryId == categoryId).ToListAsync();
    }

    public async Task<Category> GetCategoryAsync(int categoryId)
    {
        return await _connection.Table<Category>().Where(c => c.Id == categoryId).FirstOrDefaultAsync();
    }
    public async Task<List<Models.GamingRoom>> GetGamingRoomsAsync()
    {
        return await _connection.Table<Models.GamingRoom>().ToListAsync();
    }
    public async Task<Models.GamingRoom> GetGamingRoomAsync(int id)
    {
        return await _connection.Table<Models.GamingRoom>().Where(p => p.id == id).FirstOrDefaultAsync();
    }
}