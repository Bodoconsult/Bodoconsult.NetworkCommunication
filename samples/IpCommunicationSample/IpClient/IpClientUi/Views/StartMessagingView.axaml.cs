using IpClientUi.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace IpClientUi.Views;

/// <summary>
/// Interaktionslogik für FirstView.xaml
/// </summary>
public partial class StartMessagingView : ReactiveUserControl<StartMessagingViewModel>
{
    public StartMessagingView()
    {
        InitializeComponent();
            
        //this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);


        //ViewForMixins.WhenActivated((IActivatableView)this, (Action<IDisposable> d) =>
        //    {
        //        d(
        //            this.Bind<StartMessagingViewModel, StartMessagingView, string, string>(ViewModel, vm => vm.Test, view => view.PathTextBlock.Text!)
        //        );
        //    });
    }
}