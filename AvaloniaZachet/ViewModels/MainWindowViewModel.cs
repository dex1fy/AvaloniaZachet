using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaZachet.Models;
using Microsoft.EntityFrameworkCore;

namespace AvaloniaZachet.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    // TODO: Replace with your real connection string.
    private const string ConnectionString =
        "Host=edu.ngknn.ru;Port=5442;Database=41P_products;Username=21P;Password=123";

    public ObservableCollection<ProductCardViewModel> Products { get; } = new();

    public MainWindowViewModel()
    {
        _ = LoadProductsAsync();
    }

    private async Task LoadProductsAsync()
    {
        var options = new DbContextOptionsBuilder<_41pProductsContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var db = new _41pProductsContext(options);

        var items = await db.Products
            .AsNoTracking()
            .Include(p => p.IdProductTypeNavigation)
            .Include(p => p.IdMaterialTypeNavigation)
            .Include(p => p.ProductWorkshops)
            .ToListAsync();

        Products.Clear();
        foreach (var p in items)
        {
            Products.Add(new ProductCardViewModel(
                productType: p.IdProductTypeNavigation.ProductType1,
                name: p.Name,
                article: p.Article,
                time: p.ProductWorkshops.Sum(w => w.Time),
                minCostPartner: p.MinCostPartner,
                materialType: p.IdMaterialTypeNavigation.MaterialType1));
        }
    }
}
