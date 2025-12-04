using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using IGA_GUI_Module.Model.Service;

namespace IGA_GUI_Module.View;

public partial class DataAnalysisView
{
    public DataAnalysisView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// DataGrid 정렬 이벤트 핸들러
    /// Shift 키를 누르면 다중 컬럼 정렬, 그렇지 않으면 단일 컬럼 정렬
    /// </summary>
    private void DataGridView_Sorting(object __sender, DataGridSortingEventArgs __e)
    {
        if (__sender is not DataGrid dataGrid)
            return;

        __e.Handled = true;

        var column = __e.Column;
        var direction = (column.SortDirection != ListSortDirection.Ascending)
            ? ListSortDirection.Ascending
            : ListSortDirection.Descending;

        var collectionView = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
        if (collectionView == null)
            return;

        // Shift 키가 눌려있지 않으면 기존 정렬 모두 제거
        if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0)
        {
            collectionView.SortDescriptions.Clear();
            // 다른 컬럼들의 정렬 방향 표시 제거
            foreach (var col in dataGrid.Columns)
            {
                if (col != column)
                    col.SortDirection = null;
            }
        }
        else
        {
            // Shift 키가 눌려있으면 해당 컬럼의 기존 정렬만 제거
            var existingSort = collectionView.SortDescriptions
                .FirstOrDefault(__sd => __sd.PropertyName == column.SortMemberPath);
            if (!string.IsNullOrEmpty(existingSort.PropertyName))
            {
                collectionView.SortDescriptions.Remove(existingSort);
            }
        }

        // 새 정렬 추가
        collectionView.SortDescriptions.Add(new SortDescription(column.SortMemberPath, direction));
        column.SortDirection = direction;
    }

    /// <summary>
    /// 차트 저장 버튼 클릭 핸들러
    /// </summary>
    private void SaveChartButton_Click(object __sender, RoutedEventArgs __e)
    {
        var saveDialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = ChartExporter.FileFilter,
            DefaultExt = ".png",
            Title = "차트 이미지 저장",
            FileName = "chart"
        };

        if (saveDialog.ShowDialog() != true)
            return;

        var format = ChartExporter.GetFormatFromExtension(saveDialog.FileName);
        bool success = ChartExporter.SaveToFile(ChartContainer, saveDialog.FileName, format, 144);

        if (success)
        {
            MessageBox.Show($"차트가 저장되었습니다.\n{saveDialog.FileName}", "저장 완료",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show("차트 저장에 실패했습니다.", "오류",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
