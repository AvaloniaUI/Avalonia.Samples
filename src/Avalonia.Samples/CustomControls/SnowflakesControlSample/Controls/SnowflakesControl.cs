using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;
using SkiaSharp;
using SnowflakesControlSample.Models;

namespace SnowflakesControlSample.Controls;

/// <summary>
/// A control to render some <see cref="Snowflake">Snowflakes</see>. This control also adds the needed interaction logic
/// for the game to operate.
/// </summary>
public class SnowflakesControl : Control, ICustomHitTest
{
    static SnowflakesControl()
    {
        // Make sure Render is updated whenever one of these properties changes
        AffectsRender<SnowflakesControl>(IsRunningProperty, SnowflakesProperty, ScoreProperty);
    }
    
    // We use a stopwatch to measure elapsed time between two render loops as it has higher precision compared to 
    // other options. See: https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch
    private readonly Stopwatch _stopwatch = new();
    
    // A collection which holds all current visible score infos to render.
    private readonly ICollection<ScoreHint> _scoreHintsCollection = new Collection<ScoreHint>();

    /// <summary>
    /// Defines the <see cref="Snowflakes"/> property.
    /// </summary>
    public static readonly DirectProperty<SnowflakesControl, IList<Snowflake>> SnowflakesProperty =
        AvaloniaProperty.RegisterDirect<SnowflakesControl, IList<Snowflake>>(
            nameof(Snowflakes),
            o => o.Snowflakes,
            (o, v) => o.Snowflakes = v);

    /// <summary>
    /// Defines the <see cref="Score"/> property.
    /// </summary>
    public static readonly DirectProperty<SnowflakesControl, int> ScoreProperty =
        AvaloniaProperty.RegisterDirect<SnowflakesControl, int>(
            nameof(Score),
            o => o.Score,
            (o, v) => o.Score = v,
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Defines the <see cref="IsRunning"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsRunningProperty =
        AvaloniaProperty.Register<SnowflakesControl, bool>(nameof(IsRunning));

    private IList<Snowflake> _snowflakes = [];
    
    /// <summary>
    /// Gets or sets a List of  <see cref="Snowflake">Snowflakes</see> to render.
    /// </summary>
    public IList<Snowflake> Snowflakes
    {
        get => _snowflakes;
        set => SetAndRaise(SnowflakesProperty, ref _snowflakes, value);
    }

    private int _score;
    
    /// <summary>
    /// Gets or sets the current user score.
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

        // If IsRunning is updated, we need to start or stop the stopwatch accordingly.
        if (change.Property == IsRunningProperty)
        {
            if (change.GetNewValue<bool>())
            {
                // Resets and starts the stopwatch
                _stopwatch.Restart();
                
                // Clear any previous score hints
                _scoreHintsCollection.Clear();
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
        // Figure out how much time elapsed since last render loop
        var elapsedMilliseconds = _stopwatch.Elapsed.TotalMilliseconds;

        foreach (var snowFlake in Snowflakes)
        {
            // If the game is running, move each flake to it's updated position
            if (IsRunning)
            {
                snowFlake.Move(elapsedMilliseconds);
            }

            // Draw the snowflake (we use a simple circle here)
            context.DrawEllipse(
                Brushes.White,
                null,
                snowFlake.GetCenterForViewport(Bounds),
                snowFlake.Radius,
                snowFlake.Radius);
        }

        // Bonus: Render the score hint if any available.
        foreach (var scoreHint in _scoreHintsCollection.ToArray())
        {
            // If the game is running, move each flake to it's updated position
            if (IsRunning)
            {
                scoreHint.Update(elapsedMilliseconds);
            }
            
            // Use a formatted text to render the score hint.
            var formattedText =
                new FormattedText(
                    scoreHint.ToString(),
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    Typeface.Default,
                    20,
                    new SolidColorBrush(Colors.Yellow, scoreHint.Opacity));
            
            context.DrawText(formattedText, scoreHint.GetTopLeftForViewport(Bounds, new Size(formattedText.Width, formattedText.Height)));
        }

        // Display user score
        
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

        // Request next frame as soon as possible, if the game is running. Remember to reset the stopwatch.
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
            
            // Add a text hint about the earned score. We also hand over the containing collection,
            // so it can auto-remove itself after 1 second.
            _scoreHintsCollection.Add(new ScoreHint(snowFlake, _scoreHintsCollection));
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
        public string Text { get; }
    }
}