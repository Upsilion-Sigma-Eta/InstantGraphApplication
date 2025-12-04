# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

IGA (Instant Graphing Analysis) is a WPF desktop application for data analysis and visualization, built with .NET 9.0 and following the MVVM (Model-View-ViewModel) architectural pattern.

## Build Commands

Navigate to `SourceCode/IGA/` before running commands:

```bash
cd SourceCode/IGA
```

### Build
```bash
dotnet build
```

### Build without restore
```bash
dotnet build --no-restore
```

### Run the application
```bash
dotnet run --project IGA/IGA.csproj
```

### Clean build artifacts
```bash
dotnet clean
```

### Restore dependencies
```bash
dotnet restore
```

## Solution Structure

The solution consists of three projects with clear separation of concerns:

### 1. **IGA** (Main Application)
- **Location**: `SourceCode/IGA/IGA/`
- **Type**: WPF Windows executable (.exe)
- **Framework**: net9.0-windows
- **Purpose**: Entry point and main window hosting
- **Dependencies**: References both IGA_MVVM_Module and IGA_Common_Module
- **Key files**:
  - `App.xaml.cs`: Application entry point
  - `MainWindow.xaml`: Main window shell that hosts views via ContentControl with data binding

### 2. **IGA_MVVM_Module** (Previously IGA_GUI_Module)
- **Location**: `SourceCode/IGA/IGA_MVVM_Module/`
- **Type**: WPF class library
- **Framework**: net9.0-windows
- **Purpose**: Contains all Views, ViewModels, and Models following MVVM pattern
- **Structure** (V1.8):
  ```
  Model/
  ├── Data/           # Data model classes
  │   ├── CsvDataModel.cs       # CSV data container
  │   ├── ChartDataPoint.cs     # 2D Chart X,Y point
  │   ├── ChartDataPoint3D.cs   # 3D Chart X,Y,Z point (NEW)
  │   ├── SurfaceData.cs        # 3D Surface grid data (NEW)
  │   └── ChartType.cs          # Chart type enum (includes 3D types)
  │
  ├── Service/        # Business logic services
  │   ├── CsvParser.cs          # CSV file parsing
  │   ├── CsvExporter.cs        # CSV file export
  │   ├── ExcelHandler.cs       # Excel file handling
  │   ├── GraphAnalyzer.cs      # Statistics, regression, extrema
  │   ├── ChartSeriesFactory.cs # LiveCharts2 2D series factory
  │   ├── Chart3DFactory.cs     # HelixToolkit 3D element factory (NEW)
  │   └── ChartExporter.cs      # Chart image export
  │
  └── Domain/         # Domain & UI binding models
      ├── DockDocumentModel.cs      # AvalonDock document wrapper
      ├── StatisticsResult.cs       # Statistics calculation result
      ├── LinearRegressionResult.cs # Regression analysis result
      ├── YColumnSelectionItem.cs   # Y-axis selection checkbox item
      └── DataSourceItem.cs         # Data source selection item

  ViewModel/
  ├── MainViewModel.cs           # Main view: docking, file/analysis commands
  ├── DataAnalysisViewModel.cs   # Data analysis: chart, statistics, regression
  └── MainWindowViewModel.cs     # Window-level view model

  View/
  ├── MainView.xaml              # Main content view
  ├── DataAnalysisView.xaml      # Data analysis UI
  └── Style/Theme.xaml           # Dark theme styling

  ResourceDictionary/
  └── DataTemplate.xaml          # View-ViewModel mappings
  ```
- **Note**: The namespace is `IGA_GUI_Module` (historical name) but the assembly/project is `IGA_MVVM_Module`

### 3. **IGA_Common_Module** (Shared Infrastructure)
- **Location**: `SourceCode/IGA/IGA_Common_Module/`
- **Type**: Class library
- **Framework**: net9.0
- **Purpose**: Shared MVVM infrastructure and base classes
- **Contents**:
  - `Class/BaseViewModel.cs`: Base class for all ViewModels with INotifyPropertyChanged
  - `Class/RelayCommand.cs`: ICommand implementation for command binding
  - `Interfaces/IView.cs`: View contract
  - `Interfaces/IViewModel.cs`: ViewModel contract

## Architecture Patterns

### MVVM Implementation

The application uses a strict MVVM pattern with the following flow:

1. **MainWindow** (Shell) → Binds to **MainWindowViewModel**
2. **MainWindowViewModel.MainView** → Contains the current active ViewModel (IViewModel)
3. **MainView** (UserControl) → Displays content based on the bound ViewModel
4. Views use **ContentControl** with **DataTemplate** to automatically select the correct View for each ViewModel

### View Switching Pattern

Views are switched by changing the `MainView` property in `MainWindowViewModel`. The ContentControl in MainWindow automatically renders the appropriate View based on the ViewModel type using DataTemplates defined in `ResourceDictionary/DataTemplate.xaml`.

### Command Pattern

All user interactions are handled through ICommand bindings:
- Commands are defined as properties in ViewModels (e.g., `MenuFileSaveCommand`, `MenuFileImportDataCommand`)
- Implementation uses `RelayCommand` from IGA_Common_Module
- Commands follow the pattern: `private void CommandName_Execute(object? parameter)`

### Data Binding

- All ViewModels inherit from `BaseViewModel` which implements INotifyPropertyChanged
- Property changes trigger UI updates via `OnPropertyChanged()` method
- Views bind to ViewModel properties and commands using `{Binding PropertyName}` syntax

## Key Services (Model/Service)

### CsvParser
- **Location**: `Model/Service/CsvParser.cs`
- **Responsibility**: CSV file parsing
- **Key Methods**:
  - `LoadFromFile(string __filePath)`: Parse CSV to CsvDataModel
  - `IsUtf8Encoding(string __filePath)`: Check UTF-8 encoding
  - `ParseLine(string __line)`: Parse CSV line with quote handling

### CsvExporter
- **Location**: `Model/Service/CsvExporter.cs`
- **Responsibility**: CSV file export
- **Key Methods**:
  - `ExportToString(CsvDataModel)`: Convert to CSV string
  - `SaveToFile(DataTable, string)`: Save DataTable as CSV

### ExcelHandler
- **Location**: `Model/Service/ExcelHandler.cs`
- **Responsibility**: Excel file (.xlsx, .xls, .xlsm) handling
- **Key Methods**:
  - `LoadFromFile(string __filePath, int __sheetIndex)`: Load Excel to CsvDataModel
  - `GetSheetNames(string __filePath)`: Get sheet names
  - `IsExcelFile(string __filePath)`: Check if Excel file

### GraphAnalyzer
- **Location**: `Model/Service/GraphAnalyzer.cs`
- **Responsibility**: Statistical analysis and calculations
- **Key Methods**:
  - Extrema: `FindMax()`, `FindMin()`
  - Statistics: `CalculateMean()`, `CalculateVariance()`, `CalculateStandardDeviation()`, `CalculateAllStatistics()`
  - Regression: `CalculateLinearRegression()`, `GenerateRegressionLine()`
  - Deviation: `CalculateDeviations()`, `CalculateMeanAbsoluteDeviation()`

### ChartSeriesFactory
- **Location**: `Model/Service/ChartSeriesFactory.cs`
- **Responsibility**: Create LiveCharts2 2D series objects
- **Key Methods**:
  - `CreateSeries(ChartType, string, ObservableCollection<ChartDataPoint>, SKColor)`: Factory method
  - `CreateLineSeries()`, `CreateColumnSeries()`, `CreateBarSeries()`, etc.: Type-specific creators
  - `CreatePieSeries()`: Pie chart series
  - `CreateRegressionLineSeries()`: Regression line series
  - `CreateMarkerSeries()`: Max/min marker series

### Chart3DFactory
- **Location**: `Model/Service/Chart3DFactory.cs`
- **Responsibility**: Create HelixToolkit.WPF 3D visualization elements
- **Key Methods**:
  - `CreateScatter3D(IEnumerable<ChartDataPoint3D>, double, bool)`: 3D scatter plot
  - `CreateSurface3D(SurfaceData, bool)`: 3D surface plot
  - `CreateAxes3D(...)`: 3D axes (X: red, Y: green, Z: blue)
  - `CreateGridFloor(...)`: 3D grid floor
  - `GetColorFromGradient(double)`: Color gradient (blue→green→yellow→red)

## Domain Models (Model/Domain)

### StatisticsResult
- Statistics calculation result container
- Properties: Count, Sum, Mean, Variance, StandardDeviation, SampleVariance, SampleStandardDeviation, Max, MaxIndex, Min, MinIndex, Range

### LinearRegressionResult
- Linear regression result container
- Properties: Slope, Intercept, RSquared, Correlation, DataCount
- Method: `GetEquation()` - Returns equation string

### DockDocumentModel
- AvalonDock document model for tab docking
- Properties: Title, Content (IViewModel), CanClose, IsSelected
- Event: CloseRequested

## UI Structure

The application uses a two-level layout:
1. **Top Level**: Menu bar with File and Analysis menus (Korean labels: "파일", "분석")
2. **Main Area**: AvalonDock DockingManager occupying full remaining space for dockable data analysis views

### Dark Theme Styling (Theme.xaml)
Located at `IGA_MVVM_Module/View/Style/Theme.xaml`:
- **Color Palette**: Cool blue (#1F7AE0) + Warm orange (#FF8A3D) accent
- **Custom Control Templates**:
  - Menu/MenuItem with dark background and hover states
  - TabControl/TabItem with blue selection highlighting
  - ComboBox with custom dropdown popup (required for dark theme visibility)
  - DataGrid styling in DataAnalysisView.xaml
  - AvalonDock uses built-in default theme
- **Brushes**: Brush.Primary, Brush.Accent, Brush.Bg, Brush.Surface, Brush.Border, Brush.Text, Brush.Text.Muted

## Development Notes

### Language
- UI elements use Korean language labels (파일 = File, 저장 = Save, 불러오기 = Open, etc.)
- Code comments and method names are in English

### Current Implementation Status
All core features are fully implemented:
- **File Menu**: Open, Save, Import Data, Export Data (all functional)
- **Analysis Menu**: Draw Graph, Find Max, Find Min, Linear Regression, Statistics (all functional)
- **UI Features**: Multi-Y axis selection, regression line display toggle, statistics panel

## Build Requirements

**All code changes must achieve 0 warnings.** Run `dotnet build` and verify the output shows `경고 0개` (0 warnings) before committing.

If new warnings arise:
- Fix the underlying issue when possible
- Use `#pragma warning disable` with a comment explaining why (for unavoidable warnings like interface requirements)
- Add `<NoWarn>` to `.csproj` only for NuGet package compatibility warnings (NU1701)

## Coding Conventions

### Parameter Naming
- Method parameters use double underscore prefix: `__parameterName`
- Example: `public void AddDataPoints(double[] __xValues, double[] __yValues)`

## Design Documentation

Detailed design documentation is available at:
- `Documentation/IGA_Design_Document_V1.8.md` - Full design specification including use cases and architecture (Korean)

## Implementation Progress (V1.8)

### Completed Features
- [x] UC-01: CSV 파일 불러오기
- [x] UC-02: 데이터 저장 (CSV Export)
- [x] UC-03: 그래프 그리기
- [x] UC-04: 최댓값 찾기
- [x] UC-05: 최솟값 찾기

### V1.4 신규 기능 (완료)
- [x] 다중 Y축 선택 기능 (하나의 X축에 여러 Y축 표시)
- [x] 선형 회귀 분석 (GraphAnalyzer.CalculateLinearRegression)
- [x] 특정 값 기준 편차 계산 (GraphAnalyzer.CalculateDeviations)
- [x] 통계 값 연산 (평균, 표준편차, 분산, 최대/최소, 범위 등)

### V1.5 신규 기능 (완료)
- [x] 차트 유형 선택 기능 (꺾은선, 막대(세로/가로), 영역, 점, 계단형, 파이 그래프)
- [x] 데이터 병합 기능 (여러 파일의 데이터를 하나의 그래프에 오버레이)
- [x] Excel 파일 지원 (.xlsx, .xls, .xlsm 파일 불러오기)

### V1.6 신규 기능 (완료)
- [x] 탭 도킹/언도킹 기능 (AvalonDock 기반)
- [x] 플로팅 윈도우 지원 (탭을 별도 창으로 분리)
- [x] 탭 간 도킹 지원 (여러 탭을 한 화면에서 분할 표시)

### V1.7 리팩터링 (완료)
- [x] Model 폴더 구조 재편 (Data, Service, Domain 분리)
- [x] CsvHandlerComp → CsvParser + CsvExporter 분리
- [x] GraphAnalyzerComp → GraphAnalyzer 이름 변경 및 결과 클래스 분리
- [x] ChartSeriesFactory 서비스 추출 (차트 시리즈 생성 로직)
- [x] 도메인 모델 분리 (StatisticsResult, LinearRegressionResult, YColumnSelectionItem, DataSourceItem)

### UI 개선 (완료)
- [x] 탭 닫기 기능 (AvalonDock 기본 제공)
- [x] 다크 테마 ComboBox 드롭다운 수정 (커스텀 템플릿 적용)

### V1.8 신규 기능 (완료)
- [x] 3D 차트 기능 (HelixToolkit.WPF)
  - [x] 3D 산점도 (Scatter3D)
  - [x] 3D 표면 그래프 (Surface3D)
  - [x] 3D 축/그리드/와이어프레임 옵션
  - [x] 마우스 회전/확대/이동 지원
- [x] 데이터 테이블 정렬 기능
  - [x] 컬럼 헤더 클릭으로 오름차순/내림차순 토글
  - [x] Shift+클릭으로 다중 컬럼 정렬
- [x] X축 행 번호 선택 옵션
  - [x] "(행 번호)" 옵션 추가
  - [x] 모든 차트/분석 기능에서 행 번호 지원

## Dependencies and Licenses

| Package | Version | License | URL |
|---------|---------|---------|-----|
| ClosedXML | 0.104.2 | MIT | [NuGet](https://www.nuget.org/packages/closedxml/) |
| LiveChartsCore.SkiaSharpView.WPF | 2.0.0-rc4.5 | MIT | [GitHub](https://github.com/beto-rodriguez/LiveCharts2) |
| HelixToolkit.Wpf | 2.25.0 | MIT | [GitHub](https://github.com/helix-toolkit/helix-toolkit) |
| SkiaSharp | (transitive) | MIT | [GitHub](https://github.com/mono/SkiaSharp) |
| Dirkster.AvalonDock | 4.72.1 | Ms-PL | [GitHub](https://github.com/Dirkster99/AvalonDock) |
| Microsoft.Extensions.DependencyInjection | 9.0.8 | MIT | [NuGet](https://www.nuget.org/packages/microsoft.extensions.dependencyinjection) |

### Recommended Project License

Based on the dependencies used, the following licenses are compatible:

1. **MIT License** (Recommended)
   - Most permissive, widely understood
   - Compatible with all dependencies
   - Note: Must include AvalonDock's Ms-PL notice in NOTICES file

2. **Ms-PL (Microsoft Public License)**
   - Fully compatible with all MIT dependencies
   - Same license as AvalonDock

## Git Workflow

The repository uses conventional commits with prefixes:
- `feat:` - New features
- `docs:` - Documentation changes
- `fix:` - Bug fixes
- `refactor:` - Code refactoring
