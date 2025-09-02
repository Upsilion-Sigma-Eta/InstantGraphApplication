using System.ComponentModel;
using System.Runtime.CompilerServices;
using IGA_Common_Module.Interfaces;

namespace IGA_Common_Module.Class;

public class BaseViewModel : IViewModel, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}