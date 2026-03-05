namespace AvaloniaZachet.ViewModels;

public sealed class ProductCardViewModel
{
    public ProductCardViewModel(
        string productType,
        string name,
        string article,
        double time,
        int minCostPartner,
        string materialType)
    {
        ProductType = productType;
        Name = name;
        Article = article;
        Time = time;
        MinCostPartner = minCostPartner;
        MaterialType = materialType;
    }

    public string ProductType { get; }
    public string Name { get; }
    public string Article { get; }
    public double Time { get; }
    public int MinCostPartner { get; }
    public string MaterialType { get; }
}
