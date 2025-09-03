using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using IGA_Common_Module.Class;
using IGA_GUI_Module.Model;

namespace IGA_GUI_Module.ViewModel;

public class MainViewModel : BaseViewModel
{
    ObservableCollection<TabItemModel> _tabItems = new ObservableCollection<TabItemModel>();

    public ObservableCollection<TabItemModel> TabItems
    {
        get => _tabItems;
        set => _tabItems = value ?? throw new ArgumentNullException(nameof(value));
    }

    public ICommand MenuFileSaveCommand { get; private set; }
    public ICommand MenuFileOpenCommand { get; private set; }
    public ICommand MenuFileImportDataCommand { get; private set; }
    public ICommand MenuFileExportDataCommand { get; private set; }
    
    public MainViewModel()
    {
        MenuFileSaveCommand = new RelayCommand(MenuFile_SaveCommand_Execute, RelayCommand.AlwaysExecutable);
        MenuFileOpenCommand = new RelayCommand(MenuFile_OpenCommand_Execute, RelayCommand.AlwaysExecutable);
        MenuFileImportDataCommand = new RelayCommand(MenuFile_ImportDataCommand_Execute, RelayCommand.AlwaysExecutable);
        MenuFileExportDataCommand = new RelayCommand(MenuFile_ExportDataCommand_Execute, RelayCommand.AlwaysExecutable);
    }
    
    private void MenuFile_SaveCommand_Execute(object? __parameter)
    {
        throw new NotImplementedException();
    }

    private void MenuFile_OpenCommand_Execute(object? __parameter)
    {
        throw new NotImplementedException();
    }

    private void MenuFile_ImportDataCommand_Execute(object? __parameter)
    {
        MessageBox.Show("Import Data Selected");
    }

    private void MenuFile_ExportDataCommand_Execute(object? __parameter)
    {
        throw new NotImplementedException();
    }
}