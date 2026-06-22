using IpClientUi.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace IpClientUi.Views;

/// <summary>
/// Interaktionslogik für FirstView.xaml
/// </summary>
public partial class FirstView: ReactiveUserControl<FirstViewModel>
{
    public FirstView()
    {
        InitializeComponent();
            
        //this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);


        ViewForMixins.WhenActivated((IActivatableView)this, (Action<IDisposable> d) =>
            {
                d(
                    this.Bind<FirstViewModel, FirstView, string, string>(ViewModel, vm => vm.Test, view => view.PathTextBlock.Text!)
                );
            });
    }
}