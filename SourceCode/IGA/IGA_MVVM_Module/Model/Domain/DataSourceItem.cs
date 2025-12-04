using IGA_GUI_Module.Model.Data;

namespace IGA_GUI_Module.Model.Domain;

/// <summary>
/// 데이터 소스 선택 항목 (ComboBox 바인딩용)
/// </summary>
public class DataSourceItem
{
    /// <summary>
    /// 데이터 모델
    /// </summary>
    public CsvDataModel Data { get; }

    /// <summary>
    /// 기본 데이터 여부
    /// </summary>
    public bool IsMainData { get; }

    /// <summary>
    /// 표시 이름
    /// </summary>
    public string DisplayName { get; }

    public DataSourceItem(CsvDataModel __data, bool __isMainData)
    {
        Data = __data;
        IsMainData = __isMainData;
        DisplayName = __isMainData
            ? $"[기본] {__data.FileName}"
            : $"[병합] {__data.FileName}";
    }
}
