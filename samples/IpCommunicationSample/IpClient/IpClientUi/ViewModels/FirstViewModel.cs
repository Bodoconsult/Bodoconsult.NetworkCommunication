using Bodoconsult.App.ReactiveUI.Interfaces;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace IpClientUi.ViewModels;

public partial class FirstViewModel : ReactiveObject, IUiRegionViewModel
{
    /// <summary>
    /// Gets a string token representing the current ViewModel, such as 'login' or 'user'.
    /// </summary>
    public string UrlPathSegment => "first";

    //private string _name;
    //public string Test
    //{
    //    get => _name;
    //    set => this.RaiseAndSetIfChanged(ref _name, value);
    //}

    /// <summary>
    /// Test text
    /// </summary>
    [Reactive] public partial string Test { get; set; }

    /// <summary>
    /// Gets the IScreen that this ViewModel is currently being shown in. This
    /// is usually passed into the ViewModel in the Constructor and saved
    /// as a ReadOnly Property.
    /// </summary>
    public IScreen HostScreen { get; private set; } = new DummyScreen();

    public FirstViewModel()
    {
        _test = "Blubb";
    }

    public FirstViewModel(IScreen screen)
    {
        HostScreen = screen;
        _test = "Blubb";
    }

    /// <summary>
    /// Method based late injection of <see cref="ReactiveUI.IScreen"/> instance for navigation
    /// </summary>
    /// <param name="screen"></param>
    public void InjectScreen(IScreen screen)
    {
        HostScreen = screen;
    }
}