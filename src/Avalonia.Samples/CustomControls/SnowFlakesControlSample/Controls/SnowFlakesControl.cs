using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;
using SkiaSharp;
using SnowFlakesControlSample.Models;

namespace SnowFlakesControlSample.Controls;

public class SnowFlakesControl : Control, ICustomHitTest
{
    static SnowFlakesControl()
    {
        // Make sure Render is updated whenever one of these properties changes
        AffectsRender<SnowFlakesControl>(IsRunningProperty, SnowFlakesProperty, ScoreProperty);
    }
    
    private readonly Stopwatch _stopwatch = new();

    public static readonly DirectProperty<SnowFlakesControl, IList<SnowFlake>> SnowFlakesProperty =
        AvaloniaProperty.RegisterDirect<SnowFlakesControl, IList<SnowFlake>>(
            nameof(SnowFlakes),
            o => o.SnowFlakes,
            (o, v) => o.SnowFlakes = v);

    public static readonly DirectProperty<SnowFlakesControl, int> ScoreProperty =
        AvaloniaProperty.RegisterDirect<SnowFlakesControl, int>(
            nameof(Score),
            o => o.Score,
            (o, v) => o.Score = v,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<bool> IsRunningProperty =
        AvaloniaProperty.Register<SnowFlakesControl, bool>(nameof(IsRunning));

    private IList<SnowFlake> _snowFlakes = [];
    
    /// <summary>
    /// Gets or sets a List of <see cref="SnowFlake"/>s to render
    /// </summary>
    public IList<SnowFlake> SnowFlakes
    {
        get => _snowFlakes;
        set => SetAndRaise(SnowFlakesProperty, ref _snowFlakes, value);
    }

    private int _score;
    
    /// <summary>
    /// Gets or sets the current user score
    /// </summary>
    public int Score
    {
        get => _score;
        set => SetAndRaise(ScoreProperty, ref _score, value);
    }
    
    public bool IsRunning
    {
        get { return GetValue(IsRunningProperty); }
        set { SetValue(IsRunningProperty, value); }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsRunningProperty)
        {
            if (change.GetNewValue<bool>())
            {
                _stopwatch.Restart();
            }
            else
            {
                _stopwatch.Stop();
            }
        }
    }

    public override void Render(DrawingContext context)
    {
        var elapsedSeconds = _stopwatch.Elapsed.TotalSeconds;

        foreach (var snowFlake in SnowFlakes)
        {
            if (IsRunning)
            {
                snowFlake.Move(elapsedSeconds);
            }

            context.DrawEllipse(
                Brushes.White,
                null,
                snowFlake.GetCenterForViewport(Bounds),
                snowFlake.Radius,
                snowFlake.Radius);
        }

        // display hit counter
        
        // OPTION 1: Use formatted Text
        //
        // var formattedText =
        //     new FormattedText(
        //         "Your score: " + Score.ToString("N0"),
        //         CultureInfo.InvariantCulture,
        //         FlowDirection.LeftToRight,
        //         Typeface.Default,
        //         30,
        //         Brushes.Goldenrod);
        // formattedText.TextAlignment = TextAlignment.Right;
        
        // OPTION 2: Use a custom renderer using Skia-API
        context.Custom(new ScoreRenderer(Bounds, $"Your score: {Score:N0}"));

        base.Render(context);

        if (IsRunning)
        {
            // Request next frame as soon as possible 
            Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Background);
            _stopwatch.Restart();
        }
    }

    public bool HitTest(Point point)
    {
        if (!IsRunning) return false;

        var snowFlake = SnowFlakes.FirstOrDefault(x => x.IsHit(point, Bounds));
        if (snowFlake != null)
        {
            SnowFlakes.Remove(snowFlake);
            Score += snowFlake.GetHitScore();
        }

        return snowFlake != null;
    }

    private class ScoreRenderer : ICustomDrawOperation
    {
        public ScoreRenderer(Rect bounds, string text)
        {
            Bounds = bounds;
            Text = text;
        }

        public bool Equals(ICustomDrawOperation? other)
        {
            return false;
        }

        public void Dispose()
        {
            // Nothing to dispose
        }

        public bool HitTest(Point p)
        {
            // The score shouldn't be hit-test visible
            return false;
        }

        public void Render(ImmediateDrawingContext context)
        {
            var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
            if (leaseFeature == null)
            {
                FontFamily fontFamily = new FontFamily("Comic Sans MS");
                var typeface = new Typeface(fontFamily);
                var glyphs = Text.Select(c => Typeface.Default.GlyphTypeface.GetGlyph(c)).ToArray();

                var glyphRun = new GlyphRun(Typeface.Default.GlyphTypeface,
                    20,
                    Text.AsMemory(),
                    glyphs,
                    Bounds.TopRight - new Point(50, 50));
                context.DrawGlyphRun(Brushes.Goldenrod, glyphRun.TryCreateImmutableGlyphRunReference()!);
            }
            else
            {
                using var lease = leaseFeature.Lease();
                var canvas = lease.SkCanvas;
                canvas.Save();
                // create the first shader
                
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

        public Rect Bounds { get; }

        public string Text { get; }
    }
}