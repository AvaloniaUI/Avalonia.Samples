﻿using System;
using System.Collections.Generic;
using Avalonia;

namespace SnowflakesControlSample.Models;

/// <summary>
/// A class which represents a score hint to display when a <see cref="Snowflake"/> was hit and added to the total score.
/// </summary>
public class ScoreHint
{
    /// <summary>
    /// Stores the totla elapsed time in milliseconds. 
    /// </summary>
    private double _elapsedMillisecondsTotal;
    
    private readonly ICollection<ScoreHint> _scoreHintsCollection;
    
    /// <summary>
    /// Creates a new ScoreHint based on a given <see cref="Snowflake"/>.
    /// </summary>
    /// <param name="snowflake">The <see cref="Snowflake"/> that was hit.</param>
    /// <param name="scoreHintsCollection">The reference to the collection to render this item.</param>
    public ScoreHint(Snowflake snowflake, ICollection<ScoreHint> scoreHintsCollection)
    {
        Score = snowflake.GetHitScore();
        X = snowflake.X;
        Y = snowflake.Y;
        Opacity = 1;
        _scoreHintsCollection = scoreHintsCollection;
    }
    
    /// <summary>
    /// Gets the score to display.
    /// </summary>
    public int Score { get; }
    
    /// <summary>
    /// Gets the x-position in relative coordinates [0 ... 1].
    /// </summary>
    public double X { get; private set; }
    
    /// <summary>
    /// Gets the x-position in relative coordinates [0 ... 1].
    /// </summary>
    public double Y { get; private set; }
    
    /// <summary>
    /// Gets the current opacity.
    /// </summary>
    public double Opacity { get; private set; }

    /// <summary>
    /// Gets the center of the snowflake in absolute coordinates for a given Viewport.
    /// </summary>
    /// <param name="viewport">The viewport info.</param>
    /// <param name="textSize">The size of the rendered text to calculate the offset.</param>
    /// <returns>The center point in (px, px)</returns>
    public Point GetTopLeftForViewport(Rect viewport, Size textSize)
    {
        var left = (X * viewport.Width + viewport.Left) - textSize.Width / 2.0;
        var top = (Y * viewport.Height + viewport.Top) - textSize.Height;
        
        // Make sure text is not out of bounds
        if (left < 0) left = 0;
        if (top < 0) top = 0;
        if (left + textSize.Width > viewport.Width) left = viewport.Width - textSize.Width;
        
        return new Point(left, top);
    }
    
    /// <summary>
    /// Updates this items <see cref="Opacity"/> and <see cref="Y"/>-position.
    /// </summary>
    /// <param name="elapsedMilliseconds">The elapsed time in ms.</param>
    public void Update(double elapsedMilliseconds)
    {
        // Increment total elapsed time
        _elapsedMillisecondsTotal += elapsedMilliseconds;

        // remove this item from the collection if it was there for 1 second (1000 ms)
        if (_elapsedMillisecondsTotal > 1000)
        {
            _scoreHintsCollection.Remove(this);
        }
        
        // after 500 ms we wipe out the opacity and move the text upwards
        if (_elapsedMillisecondsTotal > 500)
        {
            var percentage = (_elapsedMillisecondsTotal - 500) / 500;
            Opacity = Math.Max(1.0 - percentage, 0); // Negative opacity doesn't work, so make sure it is always positive.
            Y -= (0.01 * percentage); // move slightly up. 
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"+{Score:N0}";
    }
}