# IGA (Instant Graphing Analysis)

A WPF desktop application for data analysis and visualization, built with .NET 9.0 and following the MVVM pattern.

## Features

- **Data Import**: Load CSV and Excel files (.xlsx, .xls, .xlsm)
- **Data Visualization**: Multiple chart types (Line, Column, Bar, Area, Scatter, StepLine, Pie)
- **Statistical Analysis**: Mean, variance, standard deviation, min/max, range
- **Linear Regression**: Calculate slope, intercept, R² value
- **Multi-Y Axis Support**: Display multiple Y columns on a single chart
- **Data Overlay**: Merge multiple data files into one chart
- **Docking System**: Flexible tab docking/undocking with floating windows

## Screenshots

*Coming soon*

## Requirements

- Windows 10/11 x64
- .NET 9.0 Runtime

## Building from Source

### Prerequisites

- .NET 9.0 SDK
- Visual Studio 2022 or later (optional)

### Build Commands

```bash
cd SourceCode/IGA
dotnet restore
dotnet build
```

### Run the Application

```bash
dotnet run --project IGA/IGA.csproj
```

## Project Structure

```
SourceCode/IGA/
├── IGA/                    # Main Application (WPF Executable)
│   ├── App.xaml            # Application entry point
│   └── MainWindow.xaml     # Main window shell
│
├── IGA_MVVM_Module/        # MVVM Module (WPF Class Library)
│   ├── Model/
│   │   ├── Data/           # Data models
│   │   │   ├── CsvDataModel.cs
│   │   │   ├── ChartDataPoint.cs
│   │   │   └── ChartType.cs
│   │   ├── Service/        # Business logic services
│   │   │   ├── CsvParser.cs
│   │   │   ├── CsvExporter.cs
│   │   │   ├── ExcelHandler.cs
│   │   │   ├── GraphAnalyzer.cs
│   │   │   └── ChartSeriesFactory.cs
│   │   └── Domain/         # Domain models
│   │       ├── DockDocumentModel.cs
│   │       ├── StatisticsResult.cs
│   │       ├── LinearRegressionResult.cs
│   │       ├── YColumnSelectionItem.cs
│   │       └── DataSourceItem.cs
│   ├── ViewModel/
│   │   ├── MainViewModel.cs
│   │   ├── DataAnalysisViewModel.cs
│   │   └── MainWindowViewModel.cs
│   └── View/
│       ├── MainView.xaml
│       ├── DataAnalysisView.xaml
│       └── Style/Theme.xaml
│
└── IGA_Common_Module/      # Shared Infrastructure
    ├── Class/
    │   ├── BaseViewModel.cs
    │   └── RelayCommand.cs
    └── Interfaces/
        ├── IView.cs
        └── IViewModel.cs
```

## Dependencies

| Package | Version | License |
|---------|---------|---------|
| ClosedXML | 0.104.2 | MIT |
| LiveChartsCore.SkiaSharpView.WPF | 2.0.0-rc4.5 | MIT |
| Dirkster.AvalonDock | 4.72.1 | Ms-PL |
| Microsoft.Extensions.DependencyInjection | 9.0.8 | MIT |

## Documentation

- [Design Document (Korean)](Documentation/IGA_Design_Document_V1.5.md)
- [CLAUDE.md](CLAUDE.md) - AI assistant guidelines

## License

MIT License

## Author

UpsilonSigmaEta (윤상현)
