using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using IGA_Common_Module.Class;
using IGA_GUI_Module.Model.Data;
using IGA_GUI_Module.Model.Domain;
using IGA_GUI_Module.Model.Service;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace IGA_GUI_Module.ViewModel;

/// <summary>
/// 데이터 분석 화면의 ViewModel
/// DataGrid와 Chart를 관리하고 분석 기능을 제공
/// </summary>
public class DataAnalysisViewModel : BaseViewModel
{
    #region Private Fields

    private CsvDataModel? _csvData;
    private DataTable? _dataTable;
    private ObservableCollection<ISeries> _chartSeries = new();
    private ObservableCollection<Axis> _xAxes = new();
    private ObservableCollection<Axis> _yAxes = new();
    private bool _isGraphDrawn;
    private string _maxValueText = string.Empty;
    private string _minValueText = string.Empty;
    private string _statisticsText = string.Empty;
    private string _regressionText = string.Empty;
    private int _selectedXColumnIndex;
    private ObservableCollection<int> _selectedYColumnIndices = new();
    private ChartDataPoint? _maxPoint;
    private ChartDataPoint? _minPoint;
    private bool _showRegressionLine;
    private double _referenceValue;
    private bool _showDeviations;
    private LinearRegressionResult? _currentRegression;
    private StatisticsResult? _currentStatistics;
    private ChartTypeItem _selectedChartType = null!;
    private bool _isPieChartVisible;
    private readonly List<CsvDataModel> _overlayDataList = new();
    private bool _hasOverlayData;
    private DataSourceItem? _selectedDataSource;

    // 3D 차트 관련 필드
    private bool _is3DChartVisible;
    private int _selectedZColumnIndex;
    private ObservableCollection<ChartDataPoint3D> _data3DPoints = new();
    private Model3DGroup? _chart3DModel;
    private SurfaceData? _currentSurfaceData;
    private bool _showWireframe;
    private bool _showAxes3D = true;
    private bool _showGridFloor = true;

    #endregion

    #region Properties

    /// <summary>
    /// CSV 데이터 모델
    /// </summary>
    public CsvDataModel? CsvData
    {
        get => _csvData;
        set
        {
            _csvData = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasData));
            OnPropertyChanged(nameof(ColumnHeaders));
            OnPropertyChanged(nameof(XAxisOptions));
            UpdateDataTable();
            InitializeYColumnSelection();
            UpdateDataSourceItems();
        }
    }

    /// <summary>
    /// DataGrid 바인딩용 DataTable
    /// </summary>
    public DataTable? DataTable
    {
        get => _dataTable;
        private set
        {
            _dataTable = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 데이터가 로드되었는지 여부
    /// </summary>
    public bool HasData => _csvData?.IsValid ?? false;

    /// <summary>
    /// 컬럼 헤더 목록 (ComboBox용)
    /// </summary>
    public ObservableCollection<string> ColumnHeaders =>
        _csvData?.Headers ?? new ObservableCollection<string>();

    /// <summary>
    /// X축 선택 옵션 목록 (행 번호 포함)
    /// </summary>
    public ObservableCollection<string> XAxisOptions
    {
        get
        {
            var options = new ObservableCollection<string> { "(행 번호)" };
            if (_csvData?.Headers != null)
            {
                foreach (var header in _csvData.Headers)
                {
                    options.Add(header);
                }
            }
            return options;
        }
    }

    /// <summary>
    /// Y축 선택용 체크박스 아이템 목록
    /// </summary>
    public ObservableCollection<YColumnSelectionItem> YColumnSelectionItems { get; } = new();

    /// <summary>
    /// 선택된 X축 컬럼 인덱스
    /// </summary>
    public int SelectedXColumnIndex
    {
        get => _selectedXColumnIndex;
        set
        {
            _selectedXColumnIndex = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 선택된 Y축 컬럼 인덱스 목록
    /// </summary>
    public ObservableCollection<int> SelectedYColumnIndices
    {
        get => _selectedYColumnIndices;
        set
        {
            _selectedYColumnIndices = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 차트 시리즈 컬렉션 (LiveCharts2 바인딩용)
    /// </summary>
    public ObservableCollection<ISeries> ChartSeries
    {
        get => _chartSeries;
        set
        {
            _chartSeries = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// X축 컬렉션 (LiveCharts2 바인딩용)
    /// </summary>
    public ObservableCollection<Axis> XAxes
    {
        get => _xAxes;
        set
        {
            _xAxes = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Y축 컬렉션 (LiveCharts2 바인딩용)
    /// </summary>
    public ObservableCollection<Axis> YAxes
    {
        get => _yAxes;
        set
        {
            _yAxes = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 그래프가 그려졌는지 여부
    /// </summary>
    public bool IsGraphDrawn
    {
        get => _isGraphDrawn;
        private set
        {
            _isGraphDrawn = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 최댓값 표시 텍스트
    /// </summary>
    public string MaxValueText
    {
        get => _maxValueText;
        set
        {
            _maxValueText = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 최솟값 표시 텍스트
    /// </summary>
    public string MinValueText
    {
        get => _minValueText;
        set
        {
            _minValueText = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 통계 정보 표시 텍스트
    /// </summary>
    public string StatisticsText
    {
        get => _statisticsText;
        set
        {
            _statisticsText = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 회귀 분석 결과 표시 텍스트
    /// </summary>
    public string RegressionText
    {
        get => _regressionText;
        set
        {
            _regressionText = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 회귀선 표시 여부
    /// </summary>
    public bool ShowRegressionLine
    {
        get => _showRegressionLine;
        set
        {
            _showRegressionLine = value;
            OnPropertyChanged();
            if (IsGraphDrawn)
                UpdateChartWithOptions();
        }
    }

    /// <summary>
    /// 편차 기준값
    /// </summary>
    public double ReferenceValue
    {
        get => _referenceValue;
        set
        {
            _referenceValue = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 편차 표시 여부
    /// </summary>
    public bool ShowDeviations
    {
        get => _showDeviations;
        set
        {
            _showDeviations = value;
            OnPropertyChanged();
            if (IsGraphDrawn)
                UpdateChartWithOptions();
        }
    }

    /// <summary>
    /// 현재 회귀 분석 결과
    /// </summary>
    public LinearRegressionResult? CurrentRegression
    {
        get => _currentRegression;
        private set
        {
            _currentRegression = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 현재 통계 결과
    /// </summary>
    public StatisticsResult? CurrentStatistics
    {
        get => _currentStatistics;
        private set
        {
            _currentStatistics = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 차트 유형 목록 (ComboBox 바인딩용)
    /// </summary>
    public ObservableCollection<ChartTypeItem> ChartTypeItems { get; } = new()
    {
        new ChartTypeItem(ChartType.Line, "꺾은선 그래프"),
        new ChartTypeItem(ChartType.Column, "막대 그래프 (세로)"),
        new ChartTypeItem(ChartType.Bar, "막대 그래프 (가로)"),
        new ChartTypeItem(ChartType.Area, "영역 그래프"),
        new ChartTypeItem(ChartType.Scatter, "점 그래프"),
        new ChartTypeItem(ChartType.StepLine, "계단형 그래프"),
        new ChartTypeItem(ChartType.Pie, "파이 그래프"),
        new ChartTypeItem(ChartType.Scatter3D, "3D 점 그래프"),
        new ChartTypeItem(ChartType.Surface3D, "3D 표면 그래프")
    };

    /// <summary>
    /// 선택된 차트 유형
    /// </summary>
    public ChartTypeItem SelectedChartType
    {
        get => _selectedChartType;
        set
        {
            _selectedChartType = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Is3DChartTypeSelected));
            if (IsGraphDrawn)
            {
                DrawGraph_Execute(null);
            }
        }
    }

    /// <summary>
    /// 파이 차트 시리즈 (파이 그래프용)
    /// </summary>
    public ObservableCollection<ISeries> PieSeries { get; } = new();

    /// <summary>
    /// 파이 차트 표시 여부
    /// </summary>
    public bool IsPieChartVisible
    {
        get => _isPieChartVisible;
        private set
        {
            _isPieChartVisible = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsCartesianChartVisible));
        }
    }

    /// <summary>
    /// 카테시안 차트 표시 여부 (파이 차트, 3D 차트가 아닐 때)
    /// </summary>
    public bool IsCartesianChartVisible => !_isPieChartVisible && !_is3DChartVisible;

    /// <summary>
    /// 오버레이 데이터가 있는지 여부
    /// </summary>
    public bool HasOverlayData
    {
        get => _hasOverlayData;
        private set
        {
            _hasOverlayData = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 오버레이 데이터 정보 텍스트
    /// </summary>
    public string OverlayDataInfo => _overlayDataList.Count > 0
        ? $"병합된 데이터: {string.Join(", ", _overlayDataList.Select(__d => __d.FileName))}"
        : string.Empty;

    /// <summary>
    /// 데이터 소스 목록 (데이터 테이블 전환용)
    /// </summary>
    public ObservableCollection<DataSourceItem> DataSourceItems { get; } = new();

    /// <summary>
    /// 선택된 데이터 소스
    /// </summary>
    public DataSourceItem? SelectedDataSource
    {
        get => _selectedDataSource;
        set
        {
            _selectedDataSource = value;
            OnPropertyChanged();
            UpdateDataTableFromSelectedSource();
        }
    }

    #endregion

    #region 3D Chart Properties

    /// <summary>
    /// 3D 차트 표시 여부
    /// </summary>
    public bool Is3DChartVisible
    {
        get => _is3DChartVisible;
        private set
        {
            _is3DChartVisible = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsCartesianChartVisible));
        }
    }

    /// <summary>
    /// 선택된 Z축 컬럼 인덱스
    /// </summary>
    public int SelectedZColumnIndex
    {
        get => _selectedZColumnIndex;
        set
        {
            _selectedZColumnIndex = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 3D 데이터 포인트 컬렉션
    /// </summary>
    public ObservableCollection<ChartDataPoint3D> Data3DPoints
    {
        get => _data3DPoints;
        set
        {
            _data3DPoints = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 3D 차트 모델 (View에서 바인딩)
    /// </summary>
    public Model3DGroup? Chart3DModel
    {
        get => _chart3DModel;
        private set
        {
            _chart3DModel = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 현재 Surface 데이터
    /// </summary>
    public SurfaceData? CurrentSurfaceData
    {
        get => _currentSurfaceData;
        private set
        {
            _currentSurfaceData = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 와이어프레임 표시 여부
    /// </summary>
    public bool ShowWireframe
    {
        get => _showWireframe;
        set
        {
            _showWireframe = value;
            OnPropertyChanged();
            if (IsGraphDrawn && SelectedChartType.Is3D)
            {
                DrawGraph_Execute(null);
            }
        }
    }

    /// <summary>
    /// 3D 축 표시 여부
    /// </summary>
    public bool ShowAxes3D
    {
        get => _showAxes3D;
        set
        {
            _showAxes3D = value;
            OnPropertyChanged();
            if (IsGraphDrawn && SelectedChartType.Is3D)
            {
                DrawGraph_Execute(null);
            }
        }
    }

    /// <summary>
    /// 3D 그리드 바닥면 표시 여부
    /// </summary>
    public bool ShowGridFloor
    {
        get => _showGridFloor;
        set
        {
            _showGridFloor = value;
            OnPropertyChanged();
            if (IsGraphDrawn && SelectedChartType.Is3D)
            {
                DrawGraph_Execute(null);
            }
        }
    }

    /// <summary>
    /// 3D 차트 유형 선택 여부 (UI 표시용)
    /// </summary>
    public bool Is3DChartTypeSelected => SelectedChartType?.Is3D ?? false;

    #endregion

    #region Commands

    public ICommand DrawGraphCommand { get; }
    public ICommand FindMaxCommand { get; }
    public ICommand FindMinCommand { get; }
    public ICommand CalculateRegressionCommand { get; }
    public ICommand CalculateStatisticsCommand { get; }
    public ICommand MergeDataCommand { get; }
    public ICommand ClearOverlayDataCommand { get; }

    #endregion

    #region Constructor

    public DataAnalysisViewModel()
    {
        DrawGraphCommand = new RelayCommand(DrawGraph_Execute, CanDrawGraph);
        FindMaxCommand = new RelayCommand(FindMax_Execute, CanFindExtrema);
        FindMinCommand = new RelayCommand(FindMin_Execute, CanFindExtrema);
        CalculateRegressionCommand = new RelayCommand(CalculateRegression_Execute, CanDrawGraph);
        CalculateStatisticsCommand = new RelayCommand(CalculateStatistics_Execute, CanDrawGraph);
        MergeDataCommand = new RelayCommand(MergeData_Execute, CanDrawGraph);
        ClearOverlayDataCommand = new RelayCommand(ClearOverlayData_Execute, _ => HasOverlayData);

        _selectedChartType = ChartTypeItems[0];
        InitializeAxes();
    }

    #endregion

    #region Initialization

    private void InitializeAxes()
    {
        _xAxes = new ObservableCollection<Axis>
        {
            new Axis
            {
                Name = "X",
                Labeler = __value => __value.ToString("0.##")
            }
        };

        _yAxes = new ObservableCollection<Axis>
        {
            new Axis
            {
                Name = "Y",
                Labeler = __value => __value.ToString("0.##")
            }
        };
    }

    private void InitializeYColumnSelection()
    {
        YColumnSelectionItems.Clear();
        if (_csvData == null)
            return;

        for (int i = 0; i < _csvData.Headers.Count; i++)
        {
            var item = new YColumnSelectionItem
            {
                Index = i,
                Header = _csvData.Headers[i],
                IsSelected = i == 1
            };
            item.PropertyChanged += (__sender, __e) =>
            {
                if (__e.PropertyName == nameof(YColumnSelectionItem.IsSelected))
                {
                    UpdateSelectedYColumnIndices();
                }
            };
            YColumnSelectionItems.Add(item);
        }

        UpdateSelectedYColumnIndices();
    }

    private void UpdateSelectedYColumnIndices()
    {
        SelectedYColumnIndices.Clear();
        foreach (var item in YColumnSelectionItems)
        {
            if (item.IsSelected)
            {
                SelectedYColumnIndices.Add(item.Index);
            }
        }
    }

    private void UpdateDataTable()
    {
        if (_csvData == null)
        {
            DataTable = null;
            return;
        }

        DataTable = _csvData.ToDataTable();
    }

    private void UpdateDataSourceItems()
    {
        DataSourceItems.Clear();

        if (_csvData != null)
        {
            var mainItem = new DataSourceItem(_csvData, true);
            DataSourceItems.Add(mainItem);
        }

        foreach (var overlayData in _overlayDataList)
        {
            DataSourceItems.Add(new DataSourceItem(overlayData, false));
        }

        if (DataSourceItems.Count > 0 && _selectedDataSource == null)
        {
            SelectedDataSource = DataSourceItems[0];
        }
    }

    private void UpdateDataTableFromSelectedSource()
    {
        if (_selectedDataSource?.Data == null)
        {
            DataTable = null;
            return;
        }

        DataTable = _selectedDataSource.Data.ToDataTable();
    }

    #endregion

    #region Command Handlers

    private bool CanDrawGraph(object? __parameter) => HasData && ColumnHeaders.Count >= 2;

    private bool CanFindExtrema(object? __parameter) => IsGraphDrawn;

    private void DrawGraph_Execute(object? __parameter)
    {
        if (_csvData == null || !HasData)
        {
            MessageBox.Show("데이터가 로드되지 않았습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var selectedYIndices = SelectedYColumnIndices.ToList();
        if (selectedYIndices.Count == 0)
        {
            MessageBox.Show("Y축 컬럼을 하나 이상 선택해주세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (SelectedChartType.Type != ChartType.Pie)
        {
            if (SelectedXColumnIndex < 0 || SelectedXColumnIndex >= ColumnHeaders.Count)
            {
                MessageBox.Show("X축 컬럼을 선택해주세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        try
        {
            if (SelectedChartType.Type == ChartType.Pie)
            {
                DrawPieChart(selectedYIndices);
                return;
            }

            // 3D 차트 처리
            if (SelectedChartType.Is3D)
            {
                Draw3DGraph(selectedYIndices);
                return;
            }

            IsPieChartVisible = false;
            Is3DChartVisible = false;

            // X축 데이터 가져오기 (0: 행 번호, 1~: 컬럼)
            double[] xValues;
            string xAxisName;
            bool useRowNumber = SelectedXColumnIndex == 0;

            if (useRowNumber)
            {
                // 행 번호 사용
                int rowCount = _csvData.Rows.Count;
                xValues = new double[rowCount];
                for (int i = 0; i < rowCount; i++)
                {
                    xValues[i] = i + 1; // 1부터 시작
                }
                xAxisName = "행 번호";
            }
            else
            {
                // 컬럼 데이터 사용 (인덱스 -1)
                int actualColumnIndex = SelectedXColumnIndex - 1;
                xValues = _csvData.GetColumnAsDoubles(actualColumnIndex);
                xAxisName = ColumnHeaders[actualColumnIndex];
            }

            if (xValues.Length == 0)
            {
                MessageBox.Show("X축 컬럼에 유효한 숫자 데이터가 없습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newSeries = new ObservableCollection<ISeries>();
            int colorIndex = 0;

            foreach (int yIndex in selectedYIndices)
            {
                double[] yValues = _csvData.GetColumnAsDoubles(yIndex);

                if (yValues.Length == 0)
                    continue;

                int minLength = Math.Min(xValues.Length, yValues.Length);
                var dataPoints = new ObservableCollection<ChartDataPoint>();

                for (int i = 0; i < minLength; i++)
                {
                    dataPoints.Add(new ChartDataPoint(xValues[i], yValues[i]));
                }

                var color = ChartSeriesFactory.GetColor(colorIndex);
                ISeries series = ChartSeriesFactory.CreateSeries(
                    SelectedChartType.Type,
                    ColumnHeaders[yIndex],
                    dataPoints,
                    color);
                newSeries.Add(series);
                colorIndex++;
            }

            if (newSeries.Count == 0)
            {
                MessageBox.Show("선택한 Y축 컬럼에 유효한 숫자 데이터가 없습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            foreach (var overlayData in _overlayDataList)
            {
                AddOverlayDataToSeries(newSeries, overlayData, ref colorIndex);
            }

            ChartSeries = newSeries;

            XAxes = new ObservableCollection<Axis>
            {
                new Axis
                {
                    Name = xAxisName,
                    Labeler = __value => __value.ToString("0.##")
                }
            };

            YAxes = new ObservableCollection<Axis>
            {
                new Axis
                {
                    Name = selectedYIndices.Count == 1 ? ColumnHeaders[selectedYIndices[0]] : "Y",
                    Labeler = __value => __value.ToString("0.##")
                }
            };

            ClearAnalysisResults();
            IsGraphDrawn = true;

            if (selectedYIndices.Count > 0)
            {
                CalculateStatistics_Execute(null);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"그래프 그리기 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void DrawPieChart(List<int> __selectedYIndices)
    {
        if (_csvData == null)
            return;

        IsPieChartVisible = true;
        PieSeries.Clear();

        int yIndex = __selectedYIndices[0];
        double[] yValues = _csvData.GetColumnAsDoubles(yIndex);

        if (yValues.Length == 0)
        {
            MessageBox.Show("선택한 Y축 컬럼에 유효한 숫자 데이터가 없습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        string[] labels;
        if (SelectedXColumnIndex >= 0 && SelectedXColumnIndex < ColumnHeaders.Count)
        {
            labels = new string[_csvData.Rows.Count];
            for (int i = 0; i < _csvData.Rows.Count; i++)
            {
                labels[i] = _csvData.Rows[i][SelectedXColumnIndex];
            }
        }
        else
        {
            labels = Enumerable.Range(1, yValues.Length).Select(__i => $"항목 {__i}").ToArray();
        }

        int minLength = Math.Min(yValues.Length, labels.Length);

        for (int i = 0; i < minLength; i++)
        {
            var color = ChartSeriesFactory.GetColor(i);
            var pieSeries = ChartSeriesFactory.CreatePieSeries(labels[i], yValues[i], color);
            PieSeries.Add(pieSeries);
        }

        ClearAnalysisResults();
        IsGraphDrawn = true;
        CalculateStatistics_Execute(null);
    }

    private void ClearAnalysisResults()
    {
        MaxValueText = string.Empty;
        MinValueText = string.Empty;
        StatisticsText = string.Empty;
        RegressionText = string.Empty;
        _maxPoint = null;
        _minPoint = null;
        CurrentRegression = null;
        CurrentStatistics = null;
    }

    /// <summary>
    /// X축 값 배열과 축 이름 가져오기 (행 번호 옵션 지원)
    /// </summary>
    private (double[] values, string name, int actualColumnIndex) GetXValuesAndName()
    {
        if (_csvData == null)
            return (Array.Empty<double>(), string.Empty, -1);

        if (SelectedXColumnIndex == 0)
        {
            // 행 번호 사용
            int rowCount = _csvData.Rows.Count;
            var values = new double[rowCount];
            for (int i = 0; i < rowCount; i++)
            {
                values[i] = i + 1;
            }
            return (values, "행 번호", -1);
        }
        else
        {
            // 컬럼 데이터 사용
            int actualColumnIndex = SelectedXColumnIndex - 1;
            var values = _csvData.GetColumnAsDoubles(actualColumnIndex);
            return (values, ColumnHeaders[actualColumnIndex], actualColumnIndex);
        }
    }

    private void FindMax_Execute(object? __parameter)
    {
        if (!IsGraphDrawn || _csvData == null)
            return;

        var selectedYIndices = SelectedYColumnIndices.ToList();
        if (selectedYIndices.Count == 0)
            return;

        try
        {
            int yIndex = selectedYIndices[0];
            var (xValues, _, _) = GetXValuesAndName();
            double[] yValues = _csvData.GetColumnAsDoubles(yIndex);

            if (yValues.Length == 0)
                return;

            var (maxY, maxIndex) = GraphAnalyzer.FindMax(yValues);
            double maxX = maxIndex < xValues.Length ? xValues[maxIndex] : maxIndex + 1;
            _maxPoint = new ChartDataPoint(maxX, maxY);

            MaxValueText = $"최댓값: ({maxX:0.##}, {maxY:0.##})";
            UpdateMarkersOnChart();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"최댓값 찾기 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void FindMin_Execute(object? __parameter)
    {
        if (!IsGraphDrawn || _csvData == null)
            return;

        var selectedYIndices = SelectedYColumnIndices.ToList();
        if (selectedYIndices.Count == 0)
            return;

        try
        {
            int yIndex = selectedYIndices[0];
            var (xValues, _, _) = GetXValuesAndName();
            double[] yValues = _csvData.GetColumnAsDoubles(yIndex);

            if (yValues.Length == 0)
                return;

            var (minY, minIndex) = GraphAnalyzer.FindMin(yValues);
            double minX = minIndex < xValues.Length ? xValues[minIndex] : minIndex + 1;
            _minPoint = new ChartDataPoint(minX, minY);

            MinValueText = $"최솟값: ({minX:0.##}, {minY:0.##})";
            UpdateMarkersOnChart();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"최솟값 찾기 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CalculateRegression_Execute(object? __parameter)
    {
        if (_csvData == null || !HasData)
            return;

        var selectedYIndices = SelectedYColumnIndices.ToList();
        if (selectedYIndices.Count == 0)
        {
            MessageBox.Show("Y축 컬럼을 선택해주세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            int yIndex = selectedYIndices[0];
            var (xValues, _, _) = GetXValuesAndName();
            double[] yValues = _csvData.GetColumnAsDoubles(yIndex);

            int minLength = Math.Min(xValues.Length, yValues.Length);
            if (minLength < 2)
            {
                MessageBox.Show("회귀 분석에는 최소 2개의 데이터가 필요합니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            xValues = xValues.Take(minLength).ToArray();
            yValues = yValues.Take(minLength).ToArray();

            CurrentRegression = GraphAnalyzer.CalculateLinearRegression(xValues, yValues);
            RegressionText = $"회귀식: {CurrentRegression.GetEquation()}, R² = {CurrentRegression.RSquared:F4}";

            ShowRegressionLine = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"회귀 분석 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CalculateStatistics_Execute(object? __parameter)
    {
        if (_csvData == null || !HasData)
            return;

        var selectedYIndices = SelectedYColumnIndices.ToList();
        if (selectedYIndices.Count == 0)
            return;

        try
        {
            int yIndex = selectedYIndices[0];
            double[] yValues = _csvData.GetColumnAsDoubles(yIndex);

            if (yValues.Length == 0)
            {
                StatisticsText = "통계 데이터 없음";
                return;
            }

            CurrentStatistics = GraphAnalyzer.CalculateAllStatistics(yValues);

            StatisticsText = $"개수: {CurrentStatistics.Count} | " +
                            $"평균: {CurrentStatistics.Mean:F4} | " +
                            $"표준편차: {CurrentStatistics.StandardDeviation:F4} | " +
                            $"범위: {CurrentStatistics.Range:F4}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"통계 계산 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void MergeData_Execute(object? __parameter)
    {
        var openDialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "데이터 파일 (*.csv;*.xlsx;*.xls)|*.csv;*.xlsx;*.xls|CSV 파일 (*.csv)|*.csv|Excel 파일 (*.xlsx;*.xls)|*.xlsx;*.xls|모든 파일 (*.*)|*.*",
            DefaultExt = ".csv",
            Title = "병합할 데이터 파일 선택"
        };

        if (openDialog.ShowDialog() != true)
            return;

        try
        {
            CsvDataModel overlayData;

            if (ExcelHandler.IsExcelFile(openDialog.FileName))
            {
                overlayData = ExcelHandler.LoadFromFile(openDialog.FileName);
            }
            else
            {
                overlayData = CsvParser.LoadFromFile(openDialog.FileName);
            }

            _overlayDataList.Add(overlayData);
            HasOverlayData = true;
            OnPropertyChanged(nameof(OverlayDataInfo));
            UpdateDataSourceItems();

            if (IsGraphDrawn)
            {
                DrawGraph_Execute(null);
            }

            MessageBox.Show($"'{overlayData.FileName}' 데이터가 병합되었습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"데이터 병합 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ClearOverlayData_Execute(object? __parameter)
    {
        _overlayDataList.Clear();
        HasOverlayData = false;
        OnPropertyChanged(nameof(OverlayDataInfo));

        _selectedDataSource = null;
        UpdateDataSourceItems();

        if (IsGraphDrawn)
        {
            DrawGraph_Execute(null);
        }

        MessageBox.Show("병합된 데이터가 초기화되었습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    #endregion

    #region Chart Update Methods

    private void UpdateChartWithOptions()
    {
        if (_csvData == null || !IsGraphDrawn)
            return;

        var newSeries = new ObservableCollection<ISeries>();
        foreach (var series in ChartSeries)
        {
            if (series.Name != "최댓값" && series.Name != "최솟값" &&
                series.Name != "회귀선" && series.Name != "편차")
            {
                newSeries.Add(series);
            }
        }

        if (ShowRegressionLine && CurrentRegression != null)
        {
            AddRegressionLineToSeries(newSeries);
        }

        if (_maxPoint != null)
        {
            newSeries.Add(ChartSeriesFactory.CreateMarkerSeries(_maxPoint, "최댓값", SKColors.Red, true));
        }
        if (_minPoint != null)
        {
            newSeries.Add(ChartSeriesFactory.CreateMarkerSeries(_minPoint, "최솟값", SKColors.Blue, false));
        }

        ChartSeries = newSeries;
    }

    private void AddRegressionLineToSeries(ObservableCollection<ISeries> __series)
    {
        if (_csvData == null || CurrentRegression == null)
            return;

        var (xValues, _, _) = GetXValuesAndName();
        if (xValues.Length < 2)
            return;

        double minX = xValues.Min();
        double maxX = xValues.Max();

        var regressionPoints = new ObservableCollection<ChartDataPoint>
        {
            new ChartDataPoint(minX, CurrentRegression.Slope * minX + CurrentRegression.Intercept),
            new ChartDataPoint(maxX, CurrentRegression.Slope * maxX + CurrentRegression.Intercept)
        };

        __series.Add(ChartSeriesFactory.CreateRegressionLineSeries(regressionPoints));
    }

    private void AddOverlayDataToSeries(ObservableCollection<ISeries> __series, CsvDataModel __overlayData, ref int __colorIndex)
    {
        // 행 번호 사용 여부 확인
        bool useRowNumber = SelectedXColumnIndex == 0;
        double[] overlayXValues;
        int overlayXIndex = -1;  // 행 번호 사용 시 -1, 그 외에는 X축 컬럼 인덱스

        if (useRowNumber)
        {
            // 오버레이 데이터도 행 번호 사용
            int rowCount = __overlayData.Rows.Count;
            overlayXValues = new double[rowCount];
            for (int i = 0; i < rowCount; i++)
            {
                overlayXValues[i] = i + 1;
            }
        }
        else
        {
            // 컬럼 이름으로 매칭
            int actualXIndex = SelectedXColumnIndex - 1;
            string xColumnName = actualXIndex < ColumnHeaders.Count ? ColumnHeaders[actualXIndex] : string.Empty;

            for (int i = 0; i < __overlayData.Headers.Count; i++)
            {
                if (__overlayData.Headers[i] == xColumnName)
                {
                    overlayXIndex = i;
                    break;
                }
            }

            if (overlayXIndex < 0 && __overlayData.Headers.Count > 0)
            {
                overlayXIndex = 0;
            }

            overlayXValues = __overlayData.GetColumnAsDoubles(overlayXIndex);
        }

        var selectedYIndices = SelectedYColumnIndices.ToList();
        foreach (int yIndex in selectedYIndices)
        {
            string yColumnName = ColumnHeaders[yIndex];
            int overlayYIndex = -1;

            for (int i = 0; i < __overlayData.Headers.Count; i++)
            {
                if (__overlayData.Headers[i] == yColumnName)
                {
                    overlayYIndex = i;
                    break;
                }
            }

            if (overlayYIndex < 0)
                continue;

            double[] overlayYValues = __overlayData.GetColumnAsDoubles(overlayYIndex);
            if (overlayYValues.Length == 0)
                continue;

            int minLength = Math.Min(overlayXValues.Length, overlayYValues.Length);
            var dataPoints = new ObservableCollection<ChartDataPoint>();

            for (int i = 0; i < minLength; i++)
            {
                dataPoints.Add(new ChartDataPoint(overlayXValues[i], overlayYValues[i]));
            }

            var color = ChartSeriesFactory.GetColor(__colorIndex);
            string seriesName = $"{__overlayData.FileName}: {yColumnName}";
            ISeries series = ChartSeriesFactory.CreateSeries(SelectedChartType.Type, seriesName, dataPoints, color);
            __series.Add(series);
            __colorIndex++;
        }

        bool hasMatchingColumn = selectedYIndices.Any(__yIdx =>
        {
            string yName = ColumnHeaders[__yIdx];
            return __overlayData.Headers.Contains(yName);
        });

        if (!hasMatchingColumn && __overlayData.Headers.Count > 1)
        {
            for (int i = 0; i < __overlayData.Headers.Count; i++)
            {
                if (i == overlayXIndex)
                    continue;

                double[] overlayYValues = __overlayData.GetColumnAsDoubles(i);
                if (overlayYValues.Length == 0)
                    continue;

                int minLength = Math.Min(overlayXValues.Length, overlayYValues.Length);
                var dataPoints = new ObservableCollection<ChartDataPoint>();

                for (int j = 0; j < minLength; j++)
                {
                    dataPoints.Add(new ChartDataPoint(overlayXValues[j], overlayYValues[j]));
                }

                var color = ChartSeriesFactory.GetColor(__colorIndex);
                string seriesName = $"{__overlayData.FileName}: {__overlayData.Headers[i]}";
                ISeries series = ChartSeriesFactory.CreateSeries(SelectedChartType.Type, seriesName, dataPoints, color);
                __series.Add(series);
                __colorIndex++;
            }
        }
    }

    private void UpdateMarkersOnChart()
    {
        UpdateChartWithOptions();
    }

    #endregion

    #region 3D Chart Methods

    /// <summary>
    /// 3D 그래프 그리기
    /// </summary>
    private void Draw3DGraph(List<int> __selectedYIndices)
    {
        if (_csvData == null)
            return;

        // 3D 차트는 최소 3개의 컬럼 필요 (X, Y, Z)
        if (SelectedChartType.Type == ChartType.Scatter3D)
        {
            if (__selectedYIndices.Count < 1)
            {
                MessageBox.Show("3D 점 그래프는 Y축과 Z축 컬럼을 선택해야 합니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Y축 첫 번째 선택을 Y로, Z축 선택(또는 두 번째 Y)을 Z로 사용
            int yIndex = __selectedYIndices[0];
            int zIndex = SelectedZColumnIndex >= 0 && SelectedZColumnIndex < ColumnHeaders.Count
                ? SelectedZColumnIndex
                : (__selectedYIndices.Count > 1 ? __selectedYIndices[1] : yIndex);

            Draw3DScatterChart(yIndex, zIndex);
        }
        else if (SelectedChartType.Type == ChartType.Surface3D)
        {
            if (__selectedYIndices.Count < 1)
            {
                MessageBox.Show("3D 표면 그래프는 Y축과 Z축 컬럼을 선택해야 합니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int yIndex = __selectedYIndices[0];
            int zIndex = SelectedZColumnIndex >= 0 && SelectedZColumnIndex < ColumnHeaders.Count
                ? SelectedZColumnIndex
                : (__selectedYIndices.Count > 1 ? __selectedYIndices[1] : yIndex);

            Draw3DSurfaceChart(yIndex, zIndex);
        }
    }

    /// <summary>
    /// 3D 산점도 그리기
    /// </summary>
    private void Draw3DScatterChart(int __yIndex, int __zIndex)
    {
        if (_csvData == null)
            return;

        var (xValues, xAxisName, _) = GetXValuesAndName();
        double[] yValues = _csvData.GetColumnAsDoubles(__yIndex);
        double[] zValues = _csvData.GetColumnAsDoubles(__zIndex);

        if (xValues.Length == 0 || yValues.Length == 0 || zValues.Length == 0)
        {
            MessageBox.Show("유효한 숫자 데이터가 없습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        int minLength = Math.Min(Math.Min(xValues.Length, yValues.Length), zValues.Length);
        var points = new ObservableCollection<ChartDataPoint3D>();

        for (int i = 0; i < minLength; i++)
        {
            points.Add(new ChartDataPoint3D(xValues[i], yValues[i], zValues[i]));
        }

        Data3DPoints = points;

        // 3D 모델 생성
        var model = new Model3DGroup();

        // 산점 추가
        var scatterModel = Chart3DFactory.CreateScatter3D(points, 0.3, true);
        model.Children.Add(scatterModel);

        // 축 추가
        if (ShowAxes3D)
        {
            double xMin = xValues.Min(), xMax = xValues.Max();
            double yMin = yValues.Min(), yMax = yValues.Max();
            double zMin = zValues.Min(), zMax = zValues.Max();

            var axes = Chart3DFactory.CreateAxes3D(
                xMin, xMax, yMin, yMax, zMin, zMax,
                xAxisName,
                ColumnHeaders[__yIndex],
                ColumnHeaders[__zIndex]);
            model.Children.Add(axes);

            // 그리드 바닥면
            if (ShowGridFloor)
            {
                var grid = Chart3DFactory.CreateGridFloor(xMin, xMax, yMin, yMax, zMin, 10);
                model.Children.Add(grid);
            }
        }

        Chart3DModel = model;

        // UI 상태 업데이트
        IsPieChartVisible = false;
        Is3DChartVisible = true;
        ClearAnalysisResults();
        IsGraphDrawn = true;

        // 통계 계산
        CalculateStatistics_Execute(null);
    }

    /// <summary>
    /// 3D 표면 그래프 그리기
    /// </summary>
    private void Draw3DSurfaceChart(int __yIndex, int __zIndex)
    {
        if (_csvData == null)
            return;

        var (xValues, xAxisName, _) = GetXValuesAndName();
        double[] yValues = _csvData.GetColumnAsDoubles(__yIndex);
        double[] zValues = _csvData.GetColumnAsDoubles(__zIndex);

        if (xValues.Length == 0 || yValues.Length == 0 || zValues.Length == 0)
        {
            MessageBox.Show("유효한 숫자 데이터가 없습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        int minLength = Math.Min(Math.Min(xValues.Length, yValues.Length), zValues.Length);
        var points = new List<ChartDataPoint3D>();

        for (int i = 0; i < minLength; i++)
        {
            points.Add(new ChartDataPoint3D(xValues[i], yValues[i], zValues[i]));
        }

        // 점 데이터를 Surface로 변환
        int gridResolution = Math.Min(50, (int)Math.Sqrt(minLength) + 5);
        var surfaceData = SurfaceData.FromScatterPoints(points, gridResolution);
        CurrentSurfaceData = surfaceData;

        // 3D 모델 생성
        var model = new Model3DGroup();

        // 표면 추가
        var surfaceModel = Chart3DFactory.CreateSurface3D(surfaceData, ShowWireframe);
        model.Children.Add(surfaceModel);

        // 축 추가
        if (ShowAxes3D)
        {
            var axes = Chart3DFactory.CreateAxes3D(
                surfaceData.XMin, surfaceData.XMax,
                surfaceData.YMin, surfaceData.YMax,
                surfaceData.ZMin, surfaceData.ZMax,
                xAxisName,
                ColumnHeaders[__yIndex],
                ColumnHeaders[__zIndex]);
            model.Children.Add(axes);

            // 그리드 바닥면
            if (ShowGridFloor)
            {
                var grid = Chart3DFactory.CreateGridFloor(
                    surfaceData.XMin, surfaceData.XMax,
                    surfaceData.YMin, surfaceData.YMax,
                    surfaceData.ZMin, 10);
                model.Children.Add(grid);
            }
        }

        Chart3DModel = model;

        // UI 상태 업데이트
        IsPieChartVisible = false;
        Is3DChartVisible = true;
        ClearAnalysisResults();
        IsGraphDrawn = true;

        // 통계 계산 (Z값 기준)
        CalculateStatistics_Execute(null);
    }

    #endregion
}
