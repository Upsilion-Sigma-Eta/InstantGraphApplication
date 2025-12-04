using System.Collections.ObjectModel;
using System.Data;

namespace IGA_GUI_Module.Model.Data;

/// <summary>
/// CSV 데이터를 메모리에 저장하기 위한 데이터 모델
/// </summary>
public class CsvDataModel
{
    /// <summary>
    /// CSV 파일 이름 (탭 헤더로 사용)
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// CSV 파일 전체 경로
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// 컬럼 헤더 목록
    /// </summary>
    public ObservableCollection<string> Headers { get; set; } = new();

    /// <summary>
    /// 데이터 행 목록 (각 행은 문자열 배열)
    /// </summary>
    public ObservableCollection<string[]> Rows { get; set; } = new();

    /// <summary>
    /// DataGrid 바인딩용 DataTable 반환
    /// </summary>
    public DataTable ToDataTable()
    {
        var dataTable = new DataTable();

        // 헤더 추가
        foreach (var header in Headers)
        {
            dataTable.Columns.Add(header);
        }

        // 데이터 행 추가
        foreach (var row in Rows)
        {
            var dataRow = dataTable.NewRow();
            for (int i = 0; i < row.Length && i < Headers.Count; i++)
            {
                dataRow[i] = row[i];
            }
            dataTable.Rows.Add(dataRow);
        }

        return dataTable;
    }

    /// <summary>
    /// 지정된 컬럼의 숫자 데이터를 double 배열로 반환
    /// </summary>
    public double[] GetColumnAsDoubles(int __columnIndex)
    {
        var values = new List<double>();

        foreach (var row in Rows)
        {
            if (__columnIndex < row.Length &&
                double.TryParse(row[__columnIndex], out double value))
            {
                values.Add(value);
            }
        }

        return values.ToArray();
    }

    /// <summary>
    /// 지정된 컬럼의 숫자 데이터를 double 배열로 반환 (컬럼 이름으로)
    /// </summary>
    public double[] GetColumnAsDoubles(string __columnName)
    {
        int index = Headers.IndexOf(__columnName);
        if (index < 0)
            return Array.Empty<double>();

        return GetColumnAsDoubles(index);
    }

    /// <summary>
    /// 데이터가 유효한지 확인 (헤더와 데이터 행이 있는지)
    /// </summary>
    public bool IsValid => Headers.Count > 0 && Rows.Count > 0;

    /// <summary>
    /// 총 행 수
    /// </summary>
    public int RowCount => Rows.Count;

    /// <summary>
    /// 총 열 수
    /// </summary>
    public int ColumnCount => Headers.Count;
}
