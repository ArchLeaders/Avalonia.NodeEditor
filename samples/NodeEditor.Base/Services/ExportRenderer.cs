using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Skia.Helpers;
using SkiaSharp;
using System;
using System.IO;
using System.Reflection;

namespace NodeEditorDemo.Services;

internal static class ExportRenderer
{
    private static void Render(Control target, SKCanvas canvas, double dpi = 96)
    {
        using Avalonia.Platform.IDrawingContextImpl drawingContextImpl = DrawingContextHelper.WrapSkiaCanvas(canvas, new Vector(dpi, dpi));
        Type? platformDrawingContextType = typeof(DrawingContext).Assembly.GetType("Avalonia.Media.PlatformDrawingContext");
        if (platformDrawingContextType is { }) {
            DrawingContext? drawingContext = (DrawingContext?)Activator.CreateInstance(
                platformDrawingContextType,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                new object?[] { drawingContextImpl, true },
                null);
            if (drawingContext is { }) {
                // TODO: ImmediateRenderer.Render(target, drawingContext);
            }
        }
    }

    public static void RenderPng(Control target, Size size, Stream stream, double dpi = 96)
    {
        PixelSize pixelSize = new((int)size.Width, (int)size.Height);
        Vector dpiVector = new(dpi, dpi);
        using RenderTargetBitmap bitmap = new(pixelSize, dpiVector);
        target.Measure(size);
        target.Arrange(new Rect(size));
        bitmap.Render(target);
        bitmap.Save(stream);
    }

    public static void RenderSvg(Control target, Size size, Stream stream, double dpi = 96)
    {
        using SKManagedWStream managedWStream = new(stream);
        SKRect bounds = SKRect.Create(new SKSize((float)size.Width, (float)size.Height));
        using SKCanvas canvas = SKSvgCanvas.Create(bounds, managedWStream);
        target.Measure(size);
        target.Arrange(new Rect(size));
        Render(target, canvas, dpi);
    }

    public static void RenderSkp(Control target, Size size, Stream stream, double dpi = 96)
    {
        SKRect bounds = SKRect.Create(new SKSize((float)size.Width, (float)size.Height));
        using SKPictureRecorder pictureRecorder = new();
        using SKCanvas canvas = pictureRecorder.BeginRecording(bounds);
        target.Measure(size);
        target.Arrange(new Rect(size));
        Render(target, canvas, dpi);
        using SKPicture picture = pictureRecorder.EndRecording();
        picture.Serialize(stream);
    }

    public static void RenderPdf(Control target, Size size, Stream stream, double dpi = 72)
    {
        using SKManagedWStream managedWStream = new(stream);
        using SKDocument document = SKDocument.CreatePdf(stream, (float)dpi);
        using SKCanvas canvas = document.BeginPage((float)size.Width, (float)size.Height);
        target.Measure(size);
        target.Arrange(new Rect(size));
        Render(target, canvas, dpi);
    }

    public static void RenderXps(Control target, Size size, Stream stream, double dpi = 72)
    {
        using SKManagedWStream managedWStream = new(stream);
        using SKDocument document = SKDocument.CreateXps(stream, (float)dpi);
        using SKCanvas canvas = document.BeginPage((float)size.Width, (float)size.Height);
        target.Measure(size);
        target.Arrange(new Rect(size));
        Render(target, canvas, dpi);
    }
}
