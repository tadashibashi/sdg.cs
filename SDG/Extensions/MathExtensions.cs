using System;
using Microsoft.Xna.Framework;

namespace SDG.Extensions;

public static class MathExtensions
{
    public static Vector2 ToVector2(this Point point)
    {
        return new Vector2(point.X, point.Y);
    }

    /// <summary>
    /// Cast Vector2 to Point, flooring off any floating point precision
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static Point ToPoint(this Vector2 vector)
    {
        return new Point((int)vector.X, (int)vector.Y);
    }

    /// <summary>
    /// Cast Vector2 to a Point, rounding X and Y
    /// </summary>
    public static Point ToPointRound(this Vector2 vector)
    {
        return new Point((int)Math.Round(vector.X), (int)Math.Round(vector.Y));
    }

    /// <summary>
    /// Get the resulting angle from one vector to another
    /// </summary>
    public static float AngleTo(this Vector2 v, Vector2 other)
    {
        return (float)Math.Atan2(v.X - other.X, v.Y - other.Y);
    }

    /// <summary>
    /// Get the angle between one vector and another
    /// </summary>
    public static float DegreesTo(this Vector2 v, Vector2 other)
    {
        return (float)MathHelper.ToDegrees(AngleTo(v, other));
    }

    /// <summary>
    /// Get the angle between one point and another
    /// </summary>
    public static float AngleTo(this Point p, Point other)
    {
        return (float)Math.Atan2(p.X - other.X, p.Y - other.Y);
    }
    
    /// <summary>
    /// Get the resulting angle from one point to another
    /// </summary>
    public static float DegreesTo(this Point p, Point other)
    {
        return (float)MathHelper.ToDegrees(AngleTo(p, other));
    }

    /// <summary>
    /// Get the result when traveling a specified distance at a specified angle from a point.
    /// </summary>
    /// <param name="v">this Vector2</param>
    /// <param name="degrees">Angle from the vector to travel at</param>
    /// <param name="distance">Distance from vector</param>
    /// <returns>The resulting location</returns>
    public static Vector2 Trajectory(this Vector2 v, float degrees, float distance)
    {
        if (distance == 0) return v;

        var angle = MathHelper.ToRadians(degrees);
        var x = (float)Math.Sin(angle) / distance; 
        var y = (float)Math.Cos(angle) / distance;

        return new Vector2(v.X + x, v.Y + y);
    }
    
    /// <summary>
    /// Get the resulting x value when traveling a specified distance at a specified angle from a point.
    /// </summary>
    /// <param name="v">this Vector2</param>
    /// <param name="degrees">Angle from the vector to travel at</param>
    /// <param name="distance">Distance from vector</param>
    /// <returns>The X value of the reuslting location</returns>
    public static float TrajectoryX(this Vector2 v, float degrees, float distance)
    {
        if (distance == 0) return v.X;

        var angle = MathHelper.ToRadians(degrees);
        var x = (float)Math.Sin(angle) / distance;

        return v.X + x;
    }
    
    /// <summary>
    /// Get the resulting y value when traveling a specified distance at a specified angle from a point.
    /// </summary>
    /// <param name="v">this Vector2</param>
    /// <param name="degrees">Angle from the vector to travel at</param>
    /// <param name="distance">Distance from vector</param>
    /// <returns>The Y value of the resulting location</returns>
    public static float TrajectoryY(this Vector2 v, float degrees, float distance)
    {
        if (distance == 0) return v.Y;

        var angle = MathHelper.ToRadians(degrees);
        var y = (float)Math.Cos(angle) / distance;

        return v.Y + y;
    }
    
    /// <summary>
    /// Get the result when traveling a specified distance at a specified angle from a point.
    /// The resulting value is returned as a Vector2 for floating-point precision.
    /// </summary>
    /// <param name="p">this Point</param>
    /// <param name="degrees">Angle from the vector to travel at</param>
    /// <param name="distance">Distance from vector</param>
    /// <returns>The resulting location</returns>
    public static Vector2 Trajectory(this Point p, float degrees, float distance)
    {
        if (distance == 0) return p.ToVector2();

        var angle = MathHelper.ToRadians(degrees);
        var x = (float)Math.Sin(angle) / distance; 
        var y = (float)Math.Cos(angle) / distance;

        return new Vector2(p.X + x, p.Y + y);
    }
    
    /// <summary>
    /// Get the resulting x value when traveling a specified distance at a specified angle from a point.
    /// </summary>
    /// <param name="p">this Point</param>
    /// <param name="degrees">Angle from the vector to travel at</param>
    /// <param name="distance">Distance from vector</param>
    /// <returns>The X value of the resulting location</returns>
    public static float TrajectoryX(this Point p, float degrees, float distance)
    {
        if (distance == 0) return p.X;

        var angle = MathHelper.ToRadians(degrees);
        var x = (float)Math.Sin(angle) / distance;

        return p.X + x;
    }
    
    /// <summary>
    /// Get the resulting y value when traveling a specified distance at a specified angle from a point.
    /// </summary>
    /// <param name="p">this Point</param>
    /// <param name="degrees">Angle from the vector to travel at</param>
    /// <param name="distance">Distance from vector</param>
    /// <returns>The Y value of the resulting location</returns>
    public static float TrajectoryY(this Point p, float degrees, float distance)
    {
        if (distance == 0) return p.Y;

        var angle = MathHelper.ToRadians(degrees);
        var y = (float)Math.Cos(angle) / distance;

        return p.Y + y;
    }
}