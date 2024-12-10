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
        AffectsRender<SnowFlakesControl>(IsRunningProperty, SnowflakesProperty, ScoreProperty);
    }
    
    private readonly Stopwatch _stopwatch = new();

    public static readonly DirectProperty<SnowFlakesControl, IList<SnowFlake>> SnowflakesProperty =
        AvaloniaProperty.RegisterDirect<SnowFlakesControl, IList<SnowFlake>>(
            nameof(Snowflakes),
            o => o.Snowflakes,
            (o, v) => o.Snowflakes = v);

    public static readonly DirectProperty<SnowFlakesControl, int> ScoreProperty =
        AvaloniaProperty.RegisterDirect<SnowFlakesControl, int>(
            nameof(Score),
            o => o.Score,
            (o, v) => o.Score = v,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<bool> IsRunningProperty =
        AvaloniaProperty.Register<SnowFlakesControl, bool>(nameof(IsRunning));

    private IList<SnowFlake> _snowflakes = [];
    
    /// <summary>
    /// Gets or sets a List of <see cref="SnowFlake"/>s to render
    /// </summary>
    public IList<SnowFlake> Snowflakes
    {
        get => _snowflakes;
        set => SetAndRaise(SnowflakesProperty, ref _snowflakes, value);
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
    
    /// <summary>
    /// Gets or sets a bool indicating if the Game whether the game is currently running.
    /// </summary>
    public bool IsRunning
    {
        get { return GetValue(IsRunningProperty); }
        set { SetValue(IsRunningProperty, value); }
    }
    
    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        // if IsRunning is updated, we need to start or stop the stopwatch accordingly.
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

    /// <inheritdoc />
    public override void Render(DrawingContext context)
    {
        // figure out how much time elapsed since last render loop
        var elapsedMilliseconds = _stopwatch.Elapsed.TotalMilliseconds;

        foreach (var snowFlake in Snowflakes)
        {
            // if the game is running, move each flake to it's updated position
            if (IsRunning)
            {
                snowFlake.Move(elapsedMilliseconds);
            }

            // draw the snowflake (we use a simple circle here)
            context.DrawEllipse(
                Brushes.White,
                null,
                snowFlake.GetCenterForViewport(Bounds),
                snowFlake.Radius,
                snowFlake.Radius);
        }

        // display user score
        
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

        // Request next frame as soon as possible, if the game is running. Remember to restart the stopwatch.
        if (IsRunning)
        {
            Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Background);
            _stopwatch.Restart();
        }
    }

    /// <summary>
    /// This method is needed to customize hit-testing. In our case we want the pointer hit only in case
    /// the game is running and the pointer is directly over one of our snowflakes.
    /// </summary>
    /// <param name="point">the pointer point to test.</param>
    /// <returns>true if the pointer hit the control, otherwise false</returns>
    /// <remarks>Used by the <see cref="ICustomHitTest"/>-interface, which this control implements.</remarks>
    public bool HitTest(Point point)
    {
        if (!IsRunning) return false;

        var snowFlake = Snowflakes.FirstOrDefault(x => x.IsHit(point, Bounds));
        if (snowFlake != null)
        {
            Snowflakes.Remove(snowFlake);
            Score += snowFlake.GetHitScore();
        }

        return snowFlake != null;
    }

    /// <summary>
    /// A helper-class to create a custom draw operation.
    /// </summary>
    private class ScoreRenderer : ICustomDrawOperation
    {
        public ScoreRenderer(Rect bounds, string text)
        {
            Bounds = bounds;
            Text = text;
        }

        // equals is not used in this sample and thus will be always false. 
        /// <inheritdoc />
        public bool Equals(ICustomDrawOperation? other)
        {
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
            // try to get the skia-feature.
            var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
            
            // in case we didn't find it, render the text with a fallback.
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
            // otherwise use SkiaSharp to render the text and apply some glow-effect.
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
        public string Text { get; }
    }
}