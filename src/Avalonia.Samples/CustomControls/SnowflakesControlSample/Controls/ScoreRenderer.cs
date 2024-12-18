using System;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
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
            var glyphs = Text.Select(c => Typeface.Default.GlyphTypeface.GetGlyph(c)).ToArray();

            var glyphRun = new GlyphRun(Typeface.Default.GlyphTypeface,
                20,
                Text.AsMemory(),
                glyphs,
                Bounds.TopRight - new Point(50, 50));
                
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
                paint.Shader = SKShader.CreateColor(SKColors.Goldenrod);
                paint.TextSize = 30;
                paint.TextAlign = SKTextAlign.Right;
                    
                var origin = Bounds.TopRight.ToSKPoint();
                origin.Offset(-25, +50);

                paint.ImageFilter = SKImageFilter.CreateDropShadow(0, 0, 10, 10, SKColors.White);
                canvas.DrawText(Text, origin, paint);
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