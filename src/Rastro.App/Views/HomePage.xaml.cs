using Rastro.Core.ViewModels;

namespace Rastro.App.Views;

public partial class HomePage : ContentPage
{
    private readonly TrackingViewModel _viewModel;
    public HomePage(TrackingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        PackageCards.SizeChanged += (_, _) => ResizeCards();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try { await _viewModel.InitializeAsync(); }
        catch (Exception) { await DisplayAlertAsync("Armazenamento indisponível", "Você pode continuar usando os códigos de demonstração.", "Continuar"); }
    }
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        var mobile = width < 650;
        PageContent.Padding = mobile ? new Thickness(20, 24, 20, 20) : new Thickness(36, 30, 36, 24);
        Hero.Padding = mobile ? 20 : new Thickness(30, 26);
        HeroIllustration.IsVisible = width >= 820;
        HeroGrid.ColumnDefinitions[1].Width = width >= 820 ? GridLength.Star : new GridLength(0);
        HeroHeading.FontSize = mobile ? 25 : 31;
        GreetingBadge.IsVisible = width >= 900;
        Grid.SetRow(FiltersBorder, mobile ? 1 : 0);
        Grid.SetColumn(FiltersBorder, mobile ? 0 : 1);
        Grid.SetColumnSpan(FiltersBorder, mobile ? 2 : 1);
        FiltersBorder.HorizontalOptions = mobile ? LayoutOptions.Start : LayoutOptions.End;
        foreach (var stat in new[] { StatTotal, StatActive, StatDelivered }) stat.Padding = mobile ? new Thickness(10, 14) : new Thickness(18, 16);
        ResizeCards();
    }
    private void OnCardLoaded(object? sender, EventArgs e) => ResizeCards();
    private void ResizeCards()
    {
        var available = PackageCards.Width;
        if (available <= 0) return;
        var columns = available >= 840 ? 3 : available >= 550 ? 2 : 1;
        foreach (var card in PackageCards.Children.OfType<View>())
        {
            card.WidthRequest = (available - (columns - 1) * 12) / columns;
            var index = PackageCards.Children.IndexOf(card);
            card.Margin = new Thickness(0, 0, index % columns == columns - 1 ? 0 : 12, 12);
        }
    }
}
