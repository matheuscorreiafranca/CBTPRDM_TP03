namespace Rastro.App.Controls;

[ContentProperty(nameof(Body))]
public partial class WorkspaceView : ContentView
{
    public static readonly BindableProperty BodyProperty = BindableProperty.Create(nameof(Body), typeof(View), typeof(WorkspaceView), null,
        propertyChanged: (bindable, _, value) => ((WorkspaceView)bindable).BodyHost.Content = (View)value);
    public View? Body { get => (View?)GetValue(BodyProperty); set => SetValue(BodyProperty, value); }
    public WorkspaceView() => InitializeComponent();
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        var desktop = width >= 1050;
        Sidebar.IsVisible = desktop;
        WorkspaceGrid.ColumnDefinitions[0].Width = desktop ? 216 : 0;
        MobileLogo.IsVisible = !desktop;
        MobileHelp.IsVisible = !desktop;
    }
}
