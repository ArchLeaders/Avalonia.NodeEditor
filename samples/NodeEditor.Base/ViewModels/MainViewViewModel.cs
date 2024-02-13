using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NodeEditor.Controls;
using NodeEditor.Mvvm;
using NodeEditorDemo.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NodeEditorDemo.ViewModels;

public partial class MainViewViewModel : ViewModelBase
{
    [ObservableProperty] private EditorViewModel? _editor;
    [ObservableProperty] private bool _isToolboxVisible;

    public MainViewViewModel()
    {
        _isToolboxVisible = true;
    }

    [RelayCommand]
    private void ToggleToolboxVisible()
    {
        IsToolboxVisible = !IsToolboxVisible;
    }

    [RelayCommand]
    private static void Exit()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime) {
            desktopLifetime.Shutdown();
        }
    }

    [RelayCommand]
    private static void About()
    {
        // TODO: About dialog.
    }

    [RelayCommand]
    private void New()
    {
        if (Editor?.Factory is { }) {
            Editor.Drawing = Editor.Factory.CreateDrawing();
            Editor.Drawing.SetSerializer(Editor.Serializer);
        }
    }

    private List<FilePickerFileType> GetOpenFileTypes()
    {
        return
        [
            StorageService.Json,
            StorageService.All
        ];
    }

    private static List<FilePickerFileType> GetSaveFileTypes()
    {
        return
        [
            StorageService.Json,
            StorageService.All
        ];
    }

    private static List<FilePickerFileType> GetExportFileTypes()
    {
        return
        [
            StorageService.ImagePng,
            StorageService.ImageSvg,
            StorageService.Pdf,
            StorageService.Xps,
            StorageService.ImageSkp,
            StorageService.All
        ];
    }

    [RelayCommand]
    private async Task Open()
    {
        if (Editor?.Serializer is null) {
            return;
        }

        IStorageProvider? storageProvider = StorageService.GetStorageProvider();
        if (storageProvider is null) {
            return;
        }

        IReadOnlyList<IStorageFile> result = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions {
            Title = "Open drawing",
            FileTypeFilter = GetOpenFileTypes(),
            AllowMultiple = false
        });

        IStorageFile? file = result.Count > 0 ? result[0] : null;

        if (file is not null) {
            try {
                await using Stream stream = await file.OpenReadAsync();
                using StreamReader reader = new(stream);
                string json = await reader.ReadToEndAsync();
                DrawingNodeViewModel? drawing = Editor.Serializer.Deserialize<DrawingNodeViewModel?>(json);
                if (drawing is { }) {
                    Editor.Drawing = drawing;
                    Editor.Drawing.SetSerializer(Editor.Serializer);
                }
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        if (Editor?.Serializer is null) {
            return;
        }

        IStorageProvider? storageProvider = StorageService.GetStorageProvider();
        if (storageProvider is null) {
            return;
        }

        IStorageFile? file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions {
            Title = "Save drawing",
            FileTypeChoices = GetSaveFileTypes(),
            SuggestedFileName = Path.GetFileNameWithoutExtension("drawing"),
            DefaultExtension = "json",
            ShowOverwritePrompt = true
        });

        if (file is not null) {
            try {
                string json = Editor.Serializer.Serialize(Editor.Drawing);
                await using Stream stream = await file.OpenWriteAsync();
                await using StreamWriter writer = new(stream);
                await writer.WriteAsync(json);
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
        }
    }

    [RelayCommand]
    public async Task Export()
    {
        if (Editor?.Drawing is null) {
            return;
        }

        IStorageProvider? storageProvider = StorageService.GetStorageProvider();
        if (storageProvider is null) {
            return;
        }

        IStorageFile? file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions {
            Title = "Export drawing",
            FileTypeChoices = GetExportFileTypes(),
            SuggestedFileName = Path.GetFileNameWithoutExtension("drawing"),
            DefaultExtension = "png",
            ShowOverwritePrompt = true
        });

        if (file is not null) {
            try {
                DrawingNode control = new() {
                    DataContext = Editor.Drawing
                };

                ExportRoot root = new() {
                    Width = Editor.Drawing.Width,
                    Height = Editor.Drawing.Height,
                    Child = control
                };

                root.ApplyTemplate();
                root.InvalidateMeasure();
                root.InvalidateArrange();
                root.UpdateLayout();

                Size size = new(Editor.Drawing.Width, Editor.Drawing.Height);

                if (file.Name.EndsWith(".png", StringComparison.OrdinalIgnoreCase)) {
                    using MemoryStream ms = new();
                    ExportRenderer.RenderPng(root, size, ms);
                    await using Stream stream = await file.OpenWriteAsync();
                    ms.Position = 0;
                    await stream.WriteAsync(ms.ToArray());
                }

                if (file.Name.EndsWith(".svg", StringComparison.OrdinalIgnoreCase)) {
                    using MemoryStream ms = new();
                    ExportRenderer.RenderSvg(root, size, ms);
                    await using Stream stream = await file.OpenWriteAsync();
                    ms.Position = 0;
                    await stream.WriteAsync(ms.ToArray());
                }

                if (file.Name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)) {
                    using MemoryStream ms = new();
                    ExportRenderer.RenderPdf(root, size, ms, 96);
                    await using Stream stream = await file.OpenWriteAsync();
                    ms.Position = 0;
                    await stream.WriteAsync(ms.ToArray());
                }

                if (file.Name.EndsWith("xps", StringComparison.OrdinalIgnoreCase)) {
                    using MemoryStream ms = new();
                    ExportRenderer.RenderXps(control, size, ms, 96);
                    await using Stream stream = await file.OpenWriteAsync();
                    ms.Position = 0;
                    await stream.WriteAsync(ms.ToArray());
                }

                if (file.Name.EndsWith("skp", StringComparison.OrdinalIgnoreCase)) {
                    using MemoryStream ms = new();
                    ExportRenderer.RenderSkp(control, size, ms);
                    await using Stream stream = await file.OpenWriteAsync();
                    ms.Position = 0;
                    await stream.WriteAsync(ms.ToArray());
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
