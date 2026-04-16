using IpClientUi.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;
using System.Reactive;
using ReactiveUI.SourceGenerators;

namespace IpClientUi.Views;

/// <summary>
/// Interaktionslogik für FirstView.xaml
/// </summary>
public partial class StopMessagingView : ReactiveUserControl<StopMessagingViewModel>
{
    public StopMessagingView()
    {
        InitializeComponent();
            
        //this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);


        //ViewForMixins.WhenActivated((IActivatableView)this, (Action<IDisposable> d) =>
        //    {
        //        d(
        //            this.Bind<StopMessagingViewModel, StopMessagingView, string, string>(ViewModel, vm => vm.Test, view => view.PathTextBlock.Text!)
        //        );
        //    });
    }
}