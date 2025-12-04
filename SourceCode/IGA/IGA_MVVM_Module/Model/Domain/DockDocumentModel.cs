using System.Windows.Input;
using IGA_Common_Module.Class;
using IGA_Common_Module.Interfaces;

namespace IGA_GUI_Module.Model.Domain;

/// <summary>
/// AvalonDock 도킹 문서 모델
/// 탭 도킹/언도킹 지원을 위한 모델
/// </summary>
public class DockDocumentModel : BaseViewModel
{
    private string _title = string.Empty;
    private bool _canClose = true;
    private bool _isSelected;
    private IViewModel? _content;

    /// <summary>
    /// 문서 제목
    /// </summary>
    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 문서 내용 (ViewModel)
    /// </summary>
    public IViewModel? Content
    {
        get => _content;
        set
        {
            _content = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 닫기 가능 여부
    /// </summary>
    public bool CanClose
    {
        get => _canClose;
        set
        {
            _canClose = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 선택 여부
    /// </summary>
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 문서 닫기 커맨드
    /// </summary>
    public ICommand CloseCommand { get; }

    /// <summary>
    /// 문서 닫기 요청 이벤트
    /// </summary>
    public event EventHandler? CloseRequested;

    public DockDocumentModel(IViewModel __content, string __title)
    {
        Content = __content;
        Title = __title;
        CloseCommand = new RelayCommand(Close_Execute, RelayCommand.AlwaysExecutable);
    }

    /// <summary>
    /// 닫기 실행
    /// </summary>
    private void Close_Execute(object? __parameter)
    {
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }
}
