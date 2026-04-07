using System;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;

namespace SnowflakesControlSample.Controls;

/// <summary>
/// A helper-class to create a custom draw operation.
/// </summary>
internal class ScoreRenderer : ICustomDrawOperation
{
    public ScoreRenderer(Rect bounds, string text)
    {
        Bounds = bounds;
        Text = text;
    }

        
    /// <inheritdoc />
    public bool Equals(ICustomDrawOperation? other)
    {
        // Equals is not used in this sample and thus will be always false. 
        return false;
    }
        
    /// <inheritdoc />
    public void Dispose()
    {
        // Nothing to dispose
    }

    /// <inheritdoc />
    public bool HitTest(Point p)
    {
        // The score shouldn't be hit-test visible
        return false;
    }

    /// <inheritdoc />
    public void Render(ImmediateDrawingContext context)
    {
        // Try to get the skia-feature.
        var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
            
        // In case we didn't find it, render the text with a fallback.
        if (leaseFeature == null)
        {
            var glyphTypeface = Typeface.Default.GlyphTypeface;
            // Map chars to glyph indices (ushort) using GetGlyphs
            ushort[] glyphIndices = new ushort[Text.Length];
            glyphTypeface.GetGlyphs(Text.AsSpan(), glyphIndices);

            var glyphRun = new GlyphRun(
                glyphTypeface,
                20,
                Text.AsMemory(),
                glyphIndices,
                Bounds.TopRight - new Point(50, 50)
            );
            context.DrawGlyphRun(Brushes.Goldenrod, glyphRun.TryCreateImmutableGlyphRunReference()!);
        }
        // Otherwise use SkiaSharp to render the text and apply some glow-effect.
        // Find the SkiaSharp-API here: https://learn.microsoft.com/en-us/dotnet/api/skiasharp?view=skiasharp-2.88 
        else
        {
            using var lease = leaseFeature.Lease();
            var canvas = lease.SkCanvas;
            canvas.Save();

            using (var paint = new SKPaint())
            {
                var font = new SKFont { Size = 30 };
                paint.Shader = SKShader.CreateColor(SKColors.Goldenrod);
                paint.ImageFilter = SKImageFilter.CreateDropShadow(0, 0, 10, 10, SKColors.White);

                var origin = Bounds.TopRight.ToSKPoint();
                origin.Offset(-25, +50);

                // Use new DrawText overload with SKTextAlign
                canvas.DrawText(Text, origin, SKTextAlign.Right, font, paint);
                font.Dispose();
            }
            canvas.Restore();
        }
    }

    /// <inheritdoc />
    public Rect Bounds { get; }
        
    /// <summary>
    /// Gets the Text to display.
    /// </summary>
    private string Text { get; }
}