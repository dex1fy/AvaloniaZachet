using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using AvaloniaZachet.Models;
using Microsoft.EntityFrameworkCore;

namespace AvaloniaZachet.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    // TODO: Replace with your real connection string.
    private const string ConnectionString =
        "Host=edu.ngknn.ru;Port=5442;Database=41P_products;Username=21P;Password=123";

    public ObservableCollection<ProductCardViewModel> Products { get; } = new();
    public ObservableCollection<string> ProductTypes { get; } = new();
    public ObservableCollection<string> SortOptions { get; } = new()
    {
        "Без сортировки",
        "По возрастанию",
        "По убыванию"
    };

    private readonly ObservableCollection<ProductCardViewModel> _allProducts = new();

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
                ApplyFilters();
        }
    }

    private string _selectedProductType = "Все";
    public string SelectedProductType
    {
        get => _selectedProductType;
        set
        {
            if (SetProperty(ref _selectedProductType, value))
                ApplyFilters();
        }
    }

    private string _selectedSortOption = "Без сортировки";
    public string SelectedSortOption
    {
        get => _selectedSortOption;
        set
        {
            if (SetProperty(ref _selectedSortOption, value))
                ApplyFilters();
        }
    }

    private bool _hasNoResults;
    public bool HasNoResults
    {
        get => _hasNoResults;
        private set => SetProperty(ref _hasNoResults, value);
    }

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

        var mapped = items.Select(p => new ProductCardViewModel(
                productType: p.IdProductTypeNavigation.ProductType1,
                name: p.Name,
                article: p.Article,
                time: p.ProductWorkshops.Sum(w => w.Time),
                minCostPartner: p.MinCostPartner,
                materialType: p.IdMaterialTypeNavigation.MaterialType1))
            .ToList();

        var types = mapped
            .Select(p => p.ProductType)
            .Distinct()
            .OrderBy(t => t)
            .ToList();

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            _allProducts.Clear();
            foreach (var p in mapped)
                _allProducts.Add(p);

            ProductTypes.Clear();
            ProductTypes.Add("Все");
            foreach (var t in types)
                ProductTypes.Add(t);

            SelectedProductType = "Все";
            SelectedSortOption = "Без сортировки";
            ApplyFilters();
        });
    }

    private void ApplyFilters()
    {
        IEnumerable<ProductCardViewModel> query = _allProducts;

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var text = SearchText.Trim();
            query = query.Where(p => p.Name.Contains(text, System.StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(SelectedProductType) && SelectedProductType != "Все")
            query = query.Where(p => p.ProductType == SelectedProductType);

        query = SelectedSortOption switch
        {
            "По возрастанию" => query.OrderBy(p => p.MinCostPartner),
            "По убыванию" => query.OrderByDescending(p => p.MinCostPartner),
            _ => query
        };

        Products.Clear();
        foreach (var p in query)
            Products.Add(p);

        HasNoResults = Products.Count == 0;
    }
}
