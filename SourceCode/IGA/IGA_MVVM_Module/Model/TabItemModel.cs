using IGA_Common_Module.Interfaces;

namespace IGA_GUI_Module.Model;

public class TabItemModel
{
    public IViewModel View { get; private set; }
    public string Header { get; private set; }
    
    public TabItemModel(IViewModel __view, string __header)
    {
        View = __view;
        Header = __header;
    }
}