using Rastro.Core.ViewModels;

namespace Rastro.App.Views;

public partial class ResultsPage : ContentPage
{
    public ResultsPage(TrackingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        var mobile = width < 700;
        var stack = width < 900;
        PageContent.Padding = mobile ? new Thickness(20) : new Thickness(36, 24);
        PageHeading.FontSize = mobile ? 23 : 28;
        StatusHeadline.FontSize = mobile ? 22 : 24;
        StatusCard.Padding = mobile ? 20 : 26;
        Grid.SetRow(Eta, mobile ? 1 : 0);
        Grid.SetColumn(Eta, mobile ? 0 : 1);
        Grid.SetColumnSpan(Eta, mobile ? 2 : 1);
        Grid.SetRow(RightColumn, stack ? 1 : 0);
        Grid.SetColumn(RightColumn, stack ? 0 : 1);
        Grid.SetColumnSpan(TimelineCard, stack ? 2 : 1);
        Grid.SetColumnSpan(RightColumn, stack ? 2 : 1);
    }
}
