using IGA_Common_Module.Class;

namespace IGA_GUI_Module.Model.Domain;

/// <summary>
/// Y축 컬럼 선택 항목 (CheckBox 바인딩용)
/// </summary>
public class YColumnSelectionItem : BaseViewModel
{
    private bool _isSelected;

    /// <summary>
    /// 컬럼 인덱스
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// 컬럼 헤더 이름
    /// </summary>
    public string Header { get; set; } = string.Empty;

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
}
