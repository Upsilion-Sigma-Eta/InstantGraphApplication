using IGA_Common_Module.Class;
using IGA_Common_Module.Interfaces;

namespace IGA_GUI_Module.ViewModel;

public class MainWindowViewModel : BaseViewModel
{
    private IViewModel _mainView = new MainViewModel();

    public IViewModel MainView
    {
        get => _mainView;
        set
        {
            _mainView = value;
            OnPropertyChanged();
        }
    }
}