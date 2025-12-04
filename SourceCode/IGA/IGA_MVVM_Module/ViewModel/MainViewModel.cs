using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using IGA_Common_Module.Class;
using IGA_GUI_Module.Model.Data;
using IGA_GUI_Module.Model.Domain;
using IGA_GUI_Module.Model.Service;
using Microsoft.Win32;

namespace IGA_GUI_Module.ViewModel;

/// <summary>
/// 메인 화면의 ViewModel
/// 도킹 문서 관리 및 파일 메뉴 커맨드 처리
/// </summary>
public class MainViewModel : BaseViewModel
{
    private ObservableCollection<DockDocumentModel> _documents = new();
    private DockDocumentModel? _activeDocument;

    /// <summary>
    /// 도킹 문서 컬렉션
    /// </summary>
    public ObservableCollection<DockDocumentModel> Documents
    {
        get => _documents;
        set
        {
            _documents = value ?? throw new ArgumentNullException(nameof(value));
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 현재 활성 문서
    /// </summary>
    public DockDocumentModel? ActiveDocument
    {
        get => _activeDocument;
        set
        {
            _activeDocument = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasLoadedData));
            OnPropertyChanged(nameof(IsGraphDrawn));
        }
    }

    /// <summary>
    /// 현재 선택된 탭의 ViewModel
    /// </summary>
    public DataAnalysisViewModel? CurrentDataAnalysisViewModel
    {
        get
        {
            return ActiveDocument?.Content as DataAnalysisViewModel;
        }
    }

    /// <summary>
    /// 데이터가 로드되었는지 여부 (분석 메뉴 활성화 용도)
    /// </summary>
    public bool HasLoadedData => CurrentDataAnalysisViewModel?.HasData ?? false;

    /// <summary>
    /// 그래프가 그려졌는지 여부 (극값 찾기 메뉴 활성화 용도)
    /// </summary>
    public bool IsGraphDrawn => CurrentDataAnalysisViewModel?.IsGraphDrawn ?? false;

    // 파일 메뉴 커맨드
    public ICommand MenuFileSaveCommand { get; }
    public ICommand MenuFileOpenCommand { get; }
    public ICommand MenuFileImportDataCommand { get; }
    public ICommand MenuFileExportDataCommand { get; }

    // 분석 메뉴 커맨드
    public ICommand MenuAnalysisDrawGraphCommand { get; }
    public ICommand MenuAnalysisFindMaxCommand { get; }
    public ICommand MenuAnalysisFindMinCommand { get; }
    public ICommand MenuAnalysisRegressionCommand { get; }
    public ICommand MenuAnalysisStatisticsCommand { get; }

    public MainViewModel()
    {
        // 파일 메뉴 커맨드 초기화
        MenuFileSaveCommand = new RelayCommand(MenuFile_SaveCommand_Execute, CanSaveOrExport);
        MenuFileOpenCommand = new RelayCommand(MenuFile_OpenCommand_Execute, RelayCommand.AlwaysExecutable);
        MenuFileImportDataCommand = new RelayCommand(MenuFile_ImportDataCommand_Execute, RelayCommand.AlwaysExecutable);
        MenuFileExportDataCommand = new RelayCommand(MenuFile_ExportDataCommand_Execute, CanSaveOrExport);

        // 분석 메뉴 커맨드 초기화
        MenuAnalysisDrawGraphCommand = new RelayCommand(MenuAnalysis_DrawGraphCommand_Execute, CanDrawGraph);
        MenuAnalysisFindMaxCommand = new RelayCommand(MenuAnalysis_FindMaxCommand_Execute, CanFindExtrema);
        MenuAnalysisFindMinCommand = new RelayCommand(MenuAnalysis_FindMinCommand_Execute, CanFindExtrema);
        MenuAnalysisRegressionCommand = new RelayCommand(MenuAnalysis_RegressionCommand_Execute, CanDrawGraph);
        MenuAnalysisStatisticsCommand = new RelayCommand(MenuAnalysis_StatisticsCommand_Execute, CanDrawGraph);
    }

    #region 파일 메뉴 커맨드

    private bool CanSaveOrExport(object? __parameter) => HasLoadedData;

    private void MenuFile_SaveCommand_Execute(object? __parameter)
    {
        var viewModel = CurrentDataAnalysisViewModel;
        if (viewModel?.DataTable == null)
        {
            MessageBox.Show("저장할 데이터가 없습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var saveDialog = new SaveFileDialog
        {
            Filter = "CSV 파일 (*.csv)|*.csv",
            DefaultExt = ".csv",
            Title = "CSV 파일 저장",
            FileName = viewModel.CsvData?.FileName ?? "data"
        };

        if (saveDialog.ShowDialog() == true)
        {
            try
            {
                CsvExporter.SaveToFile(viewModel.DataTable, saveDialog.FileName);
                MessageBox.Show("저장 성공", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"파일 저장 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void MenuFile_OpenCommand_Execute(object? __parameter) => LoadDataFile();

    private void MenuFile_ImportDataCommand_Execute(object? __parameter) => LoadDataFile();

    private void MenuFile_ExportDataCommand_Execute(object? __parameter) => MenuFile_SaveCommand_Execute(__parameter);

    private void LoadDataFile()
    {
        var openDialog = new OpenFileDialog
        {
            Filter = "데이터 파일 (*.csv;*.xlsx;*.xls)|*.csv;*.xlsx;*.xls|CSV 파일 (*.csv)|*.csv|Excel 파일 (*.xlsx;*.xls)|*.xlsx;*.xls|모든 파일 (*.*)|*.*",
            DefaultExt = ".csv",
            Title = "데이터 파일 열기"
        };

        if (openDialog.ShowDialog() != true)
            return;

        string filePath = openDialog.FileName;

        try
        {
            if (ExcelHandler.IsExcelFile(filePath))
            {
                LoadExcelFileAllSheets(filePath);
            }
            else
            {
                LoadCsvFile(filePath);
            }

            OnPropertyChanged(nameof(HasLoadedData));
            OnPropertyChanged(nameof(IsGraphDrawn));
        }
        catch (FileNotFoundException ex)
        {
            MessageBox.Show($"파일을 찾을 수 없습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (InvalidDataException ex)
        {
            MessageBox.Show($"파일 형식 오류: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"파일을 불러오는 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LoadCsvFile(string __filePath)
    {
        if (!CsvParser.IsUtf8Encoding(__filePath))
        {
            var result = MessageBox.Show(
                "파일이 UTF-8 인코딩이 아닐 수 있습니다. 내용이 깨질 수 있습니다.\n계속하시겠습니까?",
                "경고",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;
        }

        var dataModel = CsvParser.LoadFromFile(__filePath);
        CreateDocumentFromData(dataModel);
    }

    private void LoadExcelFileAllSheets(string __filePath)
    {
        var sheetNames = ExcelHandler.GetSheetNames(__filePath);

        if (sheetNames.Count == 0)
        {
            MessageBox.Show("Excel 파일에 시트가 없습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        string baseFileName = Path.GetFileNameWithoutExtension(__filePath);

        for (int i = 0; i < sheetNames.Count; i++)
        {
            var dataModel = ExcelHandler.LoadFromFile(__filePath, i);
            string tabTitle = sheetNames.Count > 1
                ? $"{baseFileName} - {sheetNames[i]}"
                : baseFileName;

            dataModel.FileName = tabTitle;
            CreateDocumentFromData(dataModel);
        }

        if (sheetNames.Count > 1)
        {
            MessageBox.Show($"{sheetNames.Count}개의 시트를 각각 탭으로 불러왔습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void CreateDocumentFromData(CsvDataModel __dataModel)
    {
        var dataAnalysisViewModel = new DataAnalysisViewModel
        {
            CsvData = __dataModel
        };

        var document = new DockDocumentModel(dataAnalysisViewModel, __dataModel.FileName);
        document.CloseRequested += OnDocumentCloseRequested;
        Documents.Add(document);

        ActiveDocument = document;
    }

    private void OnDocumentCloseRequested(object? __sender, EventArgs __e)
    {
        if (__sender is DockDocumentModel document)
        {
            document.CloseRequested -= OnDocumentCloseRequested;
            Documents.Remove(document);

            if (Documents.Count > 0 && ActiveDocument == document)
            {
                ActiveDocument = Documents[^1];
            }
            else if (Documents.Count == 0)
            {
                ActiveDocument = null;
            }

            OnPropertyChanged(nameof(HasLoadedData));
            OnPropertyChanged(nameof(IsGraphDrawn));
        }
    }

    #endregion

    #region 분석 메뉴 커맨드

    private bool CanDrawGraph(object? __parameter) => HasLoadedData;

    private bool CanFindExtrema(object? __parameter) => IsGraphDrawn;

    private void MenuAnalysis_DrawGraphCommand_Execute(object? __parameter)
    {
        CurrentDataAnalysisViewModel?.DrawGraphCommand.Execute(null);
        OnPropertyChanged(nameof(IsGraphDrawn));
    }

    private void MenuAnalysis_FindMaxCommand_Execute(object? __parameter)
    {
        CurrentDataAnalysisViewModel?.FindMaxCommand.Execute(null);
    }

    private void MenuAnalysis_FindMinCommand_Execute(object? __parameter)
    {
        CurrentDataAnalysisViewModel?.FindMinCommand.Execute(null);
    }

    private void MenuAnalysis_RegressionCommand_Execute(object? __parameter)
    {
        CurrentDataAnalysisViewModel?.CalculateRegressionCommand.Execute(null);
    }

    private void MenuAnalysis_StatisticsCommand_Execute(object? __parameter)
    {
        CurrentDataAnalysisViewModel?.CalculateStatisticsCommand.Execute(null);
    }

    #endregion
}
