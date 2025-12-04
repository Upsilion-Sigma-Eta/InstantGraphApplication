using System.Data;
using System.IO;
using System.Text;
using IGA_GUI_Module.Model.Data;

namespace IGA_GUI_Module.Model.Service;

/// <summary>
/// CSV 파일 내보내기 서비스
/// - CsvDataModel 또는 DataTable을 CSV 문자열로 변환
/// </summary>
public static class CsvExporter
{
    private static string _delimiter = ",";

    /// <summary>
    /// 구분자 설정
    /// </summary>
    public static void SetDelimiter(string __delimiter)
    {
        _delimiter = __delimiter;
    }

    /// <summary>
    /// 현재 구분자 반환
    /// </summary>
    public static string GetDelimiter()
    {
        return _delimiter;
    }

    /// <summary>
    /// CsvDataModel을 CSV 문자열로 변환
    /// </summary>
    /// <param name="__csvData">변환할 CSV 데이터 모델</param>
    /// <returns>CSV 형식의 문자열</returns>
    public static string ExportToString(CsvDataModel __csvData)
    {
        var sb = new StringBuilder();

        // 헤더 작성
        sb.AppendLine(string.Join(_delimiter, __csvData.Headers.Select(EscapeField)));

        // 데이터 행 작성
        foreach (var row in __csvData.Rows)
        {
            sb.AppendLine(string.Join(_delimiter, row.Select(EscapeField)));
        }

        return sb.ToString();
    }

    /// <summary>
    /// DataTable을 CSV 문자열로 변환
    /// </summary>
    /// <param name="__dataTable">변환할 DataTable</param>
    /// <returns>CSV 형식의 문자열</returns>
    public static string ExportToString(DataTable __dataTable)
    {
        var sb = new StringBuilder();

        // 헤더 작성
        var headers = __dataTable.Columns.Cast<DataColumn>()
            .Select(__col => EscapeField(__col.ColumnName));
        sb.AppendLine(string.Join(_delimiter, headers));

        // 데이터 행 작성
        foreach (DataRow row in __dataTable.Rows)
        {
            var values = row.ItemArray.Select(__item => EscapeField(__item?.ToString() ?? string.Empty));
            sb.AppendLine(string.Join(_delimiter, values));
        }

        return sb.ToString();
    }

    /// <summary>
    /// CSV 문자열을 파일로 저장
    /// </summary>
    /// <param name="__csvData">저장할 CSV 데이터 모델</param>
    /// <param name="__filePath">저장할 파일 경로</param>
    public static void SaveToFile(CsvDataModel __csvData, string __filePath)
    {
        string csvString = ExportToString(__csvData);
        File.WriteAllText(__filePath, csvString, Encoding.UTF8);
    }

    /// <summary>
    /// DataTable을 CSV 파일로 저장
    /// </summary>
    /// <param name="__dataTable">저장할 DataTable</param>
    /// <param name="__filePath">저장할 파일 경로</param>
    public static void SaveToFile(DataTable __dataTable, string __filePath)
    {
        string csvString = ExportToString(__dataTable);
        File.WriteAllText(__filePath, csvString, Encoding.UTF8);
    }

    /// <summary>
    /// CSV 필드 이스케이프 처리
    /// </summary>
    private static string EscapeField(string __field)
    {
        if (string.IsNullOrEmpty(__field))
            return string.Empty;

        // 구분자, 따옴표, 개행이 포함된 경우 따옴표로 감싸기
        if (__field.Contains(_delimiter) || __field.Contains('"') || __field.Contains('\n') || __field.Contains('\r'))
        {
            return $"\"{__field.Replace("\"", "\"\"")}\"";
        }

        return __field;
    }
}
