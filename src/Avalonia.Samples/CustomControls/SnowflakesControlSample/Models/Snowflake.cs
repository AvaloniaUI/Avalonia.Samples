using System.Diagnostics;
using System.Globalization;
using Avalonia;

namespace SnowflakesControlSample.Models;

/// <summary>
/// This model represents a snowflake.
/// </summary>
public class Snowflake
{
    /// <summary>
    /// Creates a new Snowflake.
    /// </summary>
    /// <param name="x">The relative position in x-direction [0 .. 1]</param>
    /// <param name="y">The relative position in x-direction [0 .. 1]</param>
    /// <param name="diameter">The diameter in pixel</param>
    /// <param name="speed">The speed in 1/sec</param>
    public Snowflake(double x, double y, double diameter, double speed)
    {
        X = x;
        Y = y;
        Diameter = diameter;
        Radius = diameter / 2.0;
        Speed = speed;
    }
    
    /// <summary>
    /// Gets the x-position in relative coordinates [0 ... 1]
    /// </summary>
    public double X { get; private set; }
    
    /// <summary>
    /// Gets the x-position in relative coordinates [0 ... 1]
    /// </summary>
    public double Y { get; private set; }
    
    /// <summary>
    /// Gets the diameter of the snowflake in pixel.
    /// </summary>
    public double Diameter { get; }

    /// <summary>
    /// Gets the radius of the snowflake in pixel.
    /// </summary>
    public double Radius { get; }
    
    /// <summary>
    /// Gets the speed of the snowflake in 1/sec.
    /// </summary>
    public double Speed { get; }
    
    /// <summary>
    /// Gets the center of the snow flake in absolute coordinates for a given Viewport.
    /// </summary>
    /// <param name="viewport">the viewport info.</param>
    /// <returns>The center point in (px, px)</returns>
    public Point GetCenterForViewport(Rect viewport)
    {
        return new Point(X * viewport.Width + viewport.Left, Y * viewport.Height + viewport.Top);
    }

    /// <summary>
    /// Moves the snowflake according to its speed in Y-direction. 
    /// </summary>
    /// <param name="elapsedMilliseconds">the elapsed time to consider in ms.</param>
    /// <remarks>If the snowflakes position is >1, it starts at the top again.</remarks>
    public void Move(double elapsedMilliseconds)
    {
        Y += elapsedMilliseconds * Speed / 1000.0; // Speed is in 1/sec so we have to convert it into 1/ms.
        if (Y > 1)
        {
            Y = 0;
        }
    }

    /// <summary>
    /// Calculates the score if the user hit this snowflake.
    /// </summary>
    /// <returns>the score for this snowflake.</returns>
    /// <remarks>The smaller and the faster the snowflake, the higher the score.</remarks>
    public int GetHitScore()
    {
        return (int)(1/Radius * 200 + Speed / 10.0);
    }

    /// <summary>
    /// Tests if a given point is inside hit this snowflake. 
    /// </summary>
    /// <param name="point">the point to test.</param>
    /// <param name="viewport">the viewport to consider.</param>
    /// <returns>true if point is within the snowflake, otherwise false.</returns>
    public bool IsHit(Point point, Rect viewport)
    {
        // since the snowflake is represented as a circle, we just can test if the distance to the center is 
        // equal of smaller than the radius. 
        var distance = ((Vector)(GetCenterForViewport(viewport) - point)).Length;
        return distance <= Radius;
    }
}