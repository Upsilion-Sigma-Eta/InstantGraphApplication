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
- **Structure**:
  - `View/`: UserControls for UI (MainView, DataAnalysisView)
  - `ViewModel/`: View logic and commands (MainViewModel, DataAnalysisViewModel, MainWindowViewModel)
  - `Model/`: Business logic and data structures (CSVHandlerComp, TabItemModel)
  - `View/Style/`: XAML themes and styling
  - `ResourceDictionary/`: Reusable XAML resources (DataTemplate.xaml)
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

## Key Components

### CSV Handling
- **CSVHandlerComp**: Located in `IGA_MVVM_Module/Model/`
- Contains `Parse()` method for CSV file parsing
- Uses configurable delimiter field

### Tab Management
- **TabItemModel**: Represents individual tabs in the UI
- **MainViewModel.TabItems**: ObservableCollection managing dynamic tabs
- TabControl in MainView binds to this collection for dynamic tab creation

## UI Structure

The application uses a two-level layout:
1. **Top Level**: Menu bar with File and Analysis menus (Korean labels: "파일", "분석")
2. **Main Area**: TabControl occupying full remaining space for data analysis views

## Development Notes

### Language
- UI elements use Korean language labels (파일 = File, 저장 = Save, 불러오기 = Open, etc.)
- Code comments and method names are in English

### Current Implementation Status
Based on the MainViewModel, the following features are:
- **Implemented**: Import Data functionality (shows MessageBox)
- **Not Implemented** (throw NotImplementedException):
  - Save (MenuFileSaveCommand)
  - Open (MenuFileOpenCommand)
  - Export Data (MenuFileExportDataCommand)

### Known Build Warnings
- `RelayCommand.CanExecuteChanged` event is unused (CS0067) - can be ignored or suppressed as it's part of the ICommand interface

## Design Documentation

Detailed design documentation is available at:
- `Documentation/IGA_Design_Document_V.1.1.html` - Full design specification including use cases and architecture

## Git Workflow

The repository uses conventional commits with prefixes:
- `feat:` - New features
- `docs:` - Documentation changes
- `fix:` - Bug fixes
