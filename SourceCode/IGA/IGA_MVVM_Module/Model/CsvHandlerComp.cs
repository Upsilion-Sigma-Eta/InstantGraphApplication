using System.Text;
using System.Windows.Controls;

namespace IGA_GUI_Module.Model;

public class CsvHandlerComp
{
    public static string _delimiter = ",";

    /// <summary>
    /// csv 파일 내용을 파싱해서 문자열 배열로 만드는 함수
    /// </summary>
    /// <param name="__csv">csv 파일의 전체 내용을 담은 string</param>
    /// <returns>파싱된 csv 파일의 데이터들. string 배열로 반환됨(0번 인덱스 => 1번째 열)</returns>
    public static string[] Parse(string __csv)
    {
        return __csv.Split(_delimiter);
    }

    /// <summary>
    /// DataGrid를 csv로 변환하는 함수
    /// </summary>
    /// <param name="__dataGrid">csv로 내보낼 데이터 그리드 객체</param>
    /// <returns>데이터 그리드 객체의 csv 변환 결과 문자열</returns>
    public static string ExportToCsv(DataGrid __dataGrid)
    {
        StringBuilder csvStringBuilder = new StringBuilder();

        List<string> headers = new List<string>();
        
        headers.AddRange(__dataGrid.Columns.Select(__column => __column != null ? __column.Header?.ToString() : "")!);

        csvStringBuilder.AppendJoin(_delimiter, headers);
        csvStringBuilder.AppendLine();

        foreach (ItemCollection item in __dataGrid.Items)
        {
            if (__dataGrid.ItemContainerGenerator.ContainerFromItem(item) is not DataGridRow row)
            {
                throw new Exception("Row is null");
            }

            csvStringBuilder.AppendJoin(_delimiter, row.Item?.ToString());
        }

        return csvStringBuilder.ToString();
    }
}