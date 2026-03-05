using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using AvaloniaZachet.Models;
using Microsoft.EntityFrameworkCore;

namespace AvaloniaZachet.ViewModels;


//загрузка, фильтрация, поиск, сортировка
public sealed class MainWindowViewModel : ViewModelBase
{
 
    private const string ConnectionString =
        "Host=edu.ngknn.ru;Port=5442;Database=41P_products;Username=21P;Password=123";

    //отображающаяся коллекция после применения фильтров
    public ObservableCollection<ProductCardViewModel> Products { get; } = new();

    //список доступных типов продуктов для выпадающего списка фильтрации
    public ObservableCollection<string> ProductTypes { get; } = new();

    //варианты сортировки
    public ObservableCollection<string> SortOptions { get; } = new()
    {
        "Без сортировки",
        "По возрастанию",
        "По убыванию"
    };


    // полный список всех загруженных продуктов (без фильтрации)
    // используется как источник для операций фильтрации и сортировки
    private readonly ObservableCollection<ProductCardViewModel> _allProducts = new();

    private string _searchText = string.Empty;

    //текст поискового запроса.При изменении автоматически применяются фильтры
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
                ApplyFilters();// применяем фильтрацию при изменении текста

        }
    }

    //выбранный тип продукта для фильтрации. Значение "Все" означает отсутствие фильтра по типу
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

    // выбранный вариант сортировки
    public string SelectedSortOption
    {
        get => _selectedSortOption;
        set
        {
            if (SetProperty(ref _selectedSortOption, value))
                ApplyFilters();
        }
    }

    // флаг, указывающий, что после применения фильтров продукты не найдеы
    // используется для отображения сообщения "Ничего не найдено" в интерфейсе
    private bool _hasNoResults;
    public bool HasNoResults
    {
        get => _hasNoResults;
        private set => SetProperty(ref _hasNoResults, value);
    }

   // Конструктор ViewModel.Запускает асинхронную загрузку данных из ьазы
    public MainWindowViewModel()
    {
        _ = LoadProductsAsync();
    }


    // асинхронно загружает продукты из базы данных, преобразует их в ViewModel-представления
    // заполняет список типов и применяет начальные фильтры
    private async Task LoadProductsAsync()
    {

        // Настройка контекста базы данных для PostgreSQL
        var options = new DbContextOptionsBuilder<_41pProductsContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var db = new _41pProductsContext(options);


        // Запрос к таблице Products с подгрузкой связанных данных(тип продукта, тип материала, цеха)
        // AsNoTracking отключает отслеживание изменений – оптимизация для чтения.
        var items = await db.Products
            .AsNoTracking()
            .Include(p => p.IdProductTypeNavigation)
            .Include(p => p.IdMaterialTypeNavigation)
            .Include(p => p.ProductWorkshops)
            .ToListAsync();


        // Преобразование сущностей БД в ViewModel-объекты ProductCardViewModel
        var mapped = items.Select(p => new ProductCardViewModel(
                productType: p.IdProductTypeNavigation.ProductType1,
                name: p.Name,
                article: p.Article,
                time: p.ProductWorkshops.Sum(w => w.Time),
                minCostPartner: p.MinCostPartner,
                materialType: p.IdMaterialTypeNavigation.MaterialType1))
            .ToList();

        // Извлекаем уникальные типы продуктов для выпадающего списка фильтрации
        var types = mapped
            .Select(p => p.ProductType)
            .Distinct()
            .OrderBy(t => t)
            .ToList();

        await Dispatcher.UIThread.InvokeAsync(() =>
        {

            // Заполняем полную коллекцию всеми продуктами
            _allProducts.Clear();
            foreach (var p in mapped)
                _allProducts.Add(p);

            // Заполняем список типов продуктов для фильтрации
            ProductTypes.Clear();
            ProductTypes.Add("Все");
            foreach (var t in types)
                ProductTypes.Add(t);


            // Сбрасываем фильтры к значениям по умолчанию
            SelectedProductType = "Все";
            SelectedSortOption = "Без сортировки";

            // Применяем фильтры, чтобы отобразить актуальный список
            ApplyFilters();
        });
    }


    // Применяет текущие фильтры (поиск, тип, сортировку) к полному списку продуктов
    // и обновляет отображаемую коллекцию Products.
    private void ApplyFilters()
    {
        IEnumerable<ProductCardViewModel> query = _allProducts;

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var text = SearchText.Trim();
            query = query.Where(p => p.Name.Contains(text, System.StringComparison.OrdinalIgnoreCase));
        }

        // Фильтр по типу продукта, если выбран не "Все"
        if (!string.IsNullOrWhiteSpace(SelectedProductType) && SelectedProductType != "Все")
            query = query.Where(p => p.ProductType == SelectedProductType);

        // Применяем сортировку в зависимости от выбранного варианта
        query = SelectedSortOption switch
        {
            "По возрастанию" => query.OrderBy(p => p.MinCostPartner),
            "По убыванию" => query.OrderByDescending(p => p.MinCostPartner),
            _ => query //"Без сортировки" – оставляем порядок как в _allProducts
        };



        // Обновляем отображаемую коллекцию
        Products.Clear();
        foreach (var p in query)
            Products.Add(p);


        // Устанавливаем флаг отсутствия результатов
        HasNoResults = Products.Count == 0;
    }
}
