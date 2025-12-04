namespace IGA_GUI_Module.Model.Data;

/// <summary>
/// 차트 유형 열거형
/// </summary>
public enum ChartType
{
    /// <summary>
    /// 꺾은선 그래프
    /// </summary>
    Line,

    /// <summary>
    /// 막대 그래프 (세로)
    /// </summary>
    Column,

    /// <summary>
    /// 막대 그래프 (가로)
    /// </summary>
    Bar,

    /// <summary>
    /// 영역 그래프
    /// </summary>
    Area,

    /// <summary>
    /// 점 그래프 (산점도)
    /// </summary>
    Scatter,

    /// <summary>
    /// 계단형 그래프
    /// </summary>
    StepLine,

    /// <summary>
    /// 파이 그래프
    /// </summary>
    Pie,

    /// <summary>
    /// 3D 점 그래프 (산점도)
    /// </summary>
    Scatter3D,

    /// <summary>
    /// 3D 표면 그래프
    /// </summary>
    Surface3D
}

/// <summary>
/// 차트 유형 선택 항목 (ComboBox 바인딩용)
/// </summary>
public class ChartTypeItem
{
    /// <summary>
    /// 차트 유형
    /// </summary>
    public ChartType Type { get; set; }

    /// <summary>
    /// 표시 이름
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// 3D 차트 여부
    /// </summary>
    public bool Is3D => Type == ChartType.Scatter3D || Type == ChartType.Surface3D;

    public ChartTypeItem(ChartType __type, string __displayName)
    {
        Type = __type;
        DisplayName = __displayName;
    }

    public override string ToString() => DisplayName;
}
