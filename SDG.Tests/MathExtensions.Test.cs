using Microsoft.Xna.Framework;
using SDG.Extensions;
using Xunit;

namespace SDG.Tests;

public class MathExtensionsTest
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(5.5f, -5.5f)]
    [InlineData(10.12345f, -100.12345f)]
    public void Vector2ToPoint(float x, float y)
    {
        var vec = new Vector2(x, y);
        var point = vec.ToPoint();
        Assert.Equal((int)vec.X, point.X);
        Assert.Equal( (int)vec.Y, point.Y);
    }
    
    [Theory]
    [InlineData(0, 0)]
    [InlineData(5.5f, -5.5f)]
    [InlineData(10.9999f, -100.9999f)]
    [InlineData(10.4999f, -100.4999f)]
    public void Vector2ToPointRound(float x, float y)
    {
        var vec = new Vector2(x, y);
        var point = vec.ToPointRound();
        Assert.Equal((int)Math.Round(vec.X), point.X);
        Assert.Equal( (int)Math.Round(vec.Y), point.Y);
    }

    [Fact]
    public void Vector2AngleTo()
    {
        Assert.Equal(Math.PI * .5f, Vector2.Zero.AngleTo(new Vector2(1, 0)), 6);
        Assert.Equal(Math.PI * .75f, Vector2.Zero.AngleTo(new Vector2(1, 1)), 6);
        Assert.Equal(Math.PI, Vector2.Zero.AngleTo(new Vector2(0, 1)), 6);
        Assert.Equal(Math.PI * 1.25f, Vector2.Zero.AngleTo(new Vector2(-1, 1)), 6);
        Assert.Equal(Math.PI * 1.5f, Vector2.Zero.AngleTo(new Vector2(-1, 0)), 6);
        Assert.Equal(Math.PI * 1.75f, Vector2.Zero.AngleTo(new Vector2(-1, -1)), 6);
        Assert.Equal(0, Vector2.Zero.AngleTo(new Vector2(0, -1)), 6);
        Assert.Equal(Math.PI * 0.25f, Vector2.Zero.AngleTo(new Vector2(1, -1)), 6);
    }
    
    [Fact]
    public void Vector2DegreesTo()
    {
        Assert.Equal(90, Vector2.Zero.DegreesTo(new Vector2(1, 0)), 6);
        Assert.Equal(135, Vector2.Zero.DegreesTo(new Vector2(1, 1)), 6);
        Assert.Equal(180, Vector2.Zero.DegreesTo(new Vector2(0, 1)), 6);
        Assert.Equal(225, Vector2.Zero.DegreesTo(new Vector2(-1, 1)), 6);
        Assert.Equal(270, Vector2.Zero.DegreesTo(new Vector2(-1, 0)), 6);
        Assert.Equal(315, Vector2.Zero.DegreesTo(new Vector2(-1, -1)), 6);
        Assert.Equal(0, Vector2.Zero.DegreesTo(new Vector2(0, -1)), 6);
        Assert.Equal(45, Vector2.Zero.DegreesTo(new Vector2(1, -1)), 6);
    }
    
    [Fact]
    public void PointAngleTo()
    {
        Assert.Equal(Math.PI * .5f, Point.Zero.AngleTo(new Point(1, 0)), 6);
        Assert.Equal(Math.PI * .75f, Point.Zero.AngleTo(new Point(1, 1)), 6);
        Assert.Equal(Math.PI, Point.Zero.AngleTo(new Point(0, 1)), 6);
        Assert.Equal(Math.PI * 1.25f, Point.Zero.AngleTo(new Point(-1, 1)), 6);
        Assert.Equal(Math.PI * 1.5f, Point.Zero.AngleTo(new Point(-1, 0)), 6);
        Assert.Equal(Math.PI * 1.75f, Point.Zero.AngleTo(new Point(-1, -1)), 6);
        Assert.Equal(0, Point.Zero.AngleTo(new Point(0, -1)), 6);
        Assert.Equal(Math.PI * 0.25f, Point.Zero.AngleTo(new Point(1, -1)), 6);
    }
    
    [Fact]
    public void PointDegreesTo()
    {
        Assert.Equal(90, Point.Zero.DegreesTo(new Point(1, 0)), 6);
        Assert.Equal(135, Point.Zero.DegreesTo(new Point(1, 1)), 6);
        Assert.Equal(180, Point.Zero.DegreesTo(new Point(0, 1)), 6);
        Assert.Equal(225, Point.Zero.DegreesTo(new Point(-1, 1)), 6);
        Assert.Equal(270, Point.Zero.DegreesTo(new Point(-1, 0)), 6);
        Assert.Equal(315, Point.Zero.DegreesTo(new Point(-1, -1)), 6);
        Assert.Equal(0, Point.Zero.DegreesTo(new Point(0, -1)), 6);
        Assert.Equal(45, Point.Zero.DegreesTo(new Point(1, -1)), 6);
    }

    [Fact]
    public void Vector2TrajectoryXAndY()
    {
        var sqrtOf2 = (float)Math.Sqrt(2.0);
        
        Assert.Equal(0, Vector2.Zero.TrajectoryX(0, 1), 6);
        Assert.Equal(-1, Vector2.Zero.TrajectoryY(0, 1), 6);
        Assert.Equal(1, Vector2.Zero.TrajectoryX(45, sqrtOf2), 6);
        Assert.Equal(-1, Vector2.Zero.TrajectoryY(45, sqrtOf2), 6);
        Assert.Equal(1, Vector2.Zero.TrajectoryX(90, 1), 6);
        Assert.Equal(0, Vector2.Zero.TrajectoryY(90, 1), 6);
        Assert.Equal(1, Vector2.Zero.TrajectoryX(135, sqrtOf2), 6);
        Assert.Equal(1, Vector2.Zero.TrajectoryY(135, sqrtOf2), 6);
        Assert.Equal(0, Vector2.Zero.TrajectoryX(180, 1), 6);
        Assert.Equal(1, Vector2.Zero.TrajectoryY(180, 1), 6);
        Assert.Equal(-1, Vector2.Zero.TrajectoryX(225, sqrtOf2), 6);
        Assert.Equal(1, Vector2.Zero.TrajectoryY(225, sqrtOf2), 6);
        Assert.Equal(-1, Vector2.Zero.TrajectoryX(270, 1), 6);
        Assert.Equal(0, Vector2.Zero.TrajectoryY(270, 1), 6);
        Assert.Equal(-1, Vector2.Zero.TrajectoryX(315, sqrtOf2), 6);
        Assert.Equal(-1, Vector2.Zero.TrajectoryY(315, sqrtOf2), 6);
        Assert.Equal(0, Vector2.Zero.TrajectoryX(360, 1), 6);
        Assert.Equal(-1, Vector2.Zero.TrajectoryY(360, 1), 6);
    }
    

    [Theory]
    [InlineData(0, 2)]
    [InlineData(45, 14)]
    [InlineData(-45, -1234)]
    [InlineData(-12345, 1234)]
    [InlineData(360, 1234)]
    [InlineData(359.99999, .002)]
    public void Vector2Trajectory(float degrees, float distance)
    {
        // Test assumes that all tests passed in Vector2TrajectoryXAndY
        var result = Vector2.Zero.Trajectory(degrees, distance);
        Assert.Equal(result.X, Vector2.Zero.TrajectoryX(degrees, distance));
        Assert.Equal(result.Y, Vector2.Zero.TrajectoryY(degrees, distance));
    }
    
    
    [Fact]
    public void PointTrajectoryXAndY()
    {
        var sqrtOf2 = (float)Math.Sqrt(2.0);
        
        Assert.Equal(0, Point.Zero.TrajectoryX(0, 1), 6);
        Assert.Equal(-1, Point.Zero.TrajectoryY(0, 1), 6);
        Assert.Equal(1, Point.Zero.TrajectoryX(45, sqrtOf2), 6);
        Assert.Equal(-1, Point.Zero.TrajectoryY(45, sqrtOf2), 6);
        Assert.Equal(1, Point.Zero.TrajectoryX(90, 1), 6);
        Assert.Equal(0, Point.Zero.TrajectoryY(90, 1), 6);
        Assert.Equal(1, Point.Zero.TrajectoryX(135, sqrtOf2), 6);
        Assert.Equal(1, Point.Zero.TrajectoryY(135, sqrtOf2), 6);
        Assert.Equal(0, Point.Zero.TrajectoryX(180, 1), 6);
        Assert.Equal(1, Point.Zero.TrajectoryY(180, 1), 6);
        Assert.Equal(-1, Point.Zero.TrajectoryX(225, sqrtOf2), 6);
        Assert.Equal(1, Point.Zero.TrajectoryY(225, sqrtOf2), 6);
        Assert.Equal(-1, Point.Zero.TrajectoryX(270, 1), 6);
        Assert.Equal(0, Point.Zero.TrajectoryY(270, 1), 6);
        Assert.Equal(-1, Point.Zero.TrajectoryX(315, sqrtOf2), 6);
        Assert.Equal(-1, Point.Zero.TrajectoryY(315, sqrtOf2), 6);
        Assert.Equal(0, Point.Zero.TrajectoryX(360, 1), 6);
        Assert.Equal(-1, Point.Zero.TrajectoryY(360, 1), 6);
    }
    
    
    [Theory]
    [InlineData(0, 2)]
    [InlineData(45, 14)]
    [InlineData(-45, -1234)]
    [InlineData(-12345, 1234)]
    [InlineData(360, 1234)]
    [InlineData(359.99999, .002)]
    public void PointTrajectory(float degrees, float distance)
    {
        // Test assumes that all tests passed in PointTrajectoryXAndY
        var result = Point.Zero.Trajectory(degrees, distance);
        Assert.Equal(result.X, Point.Zero.TrajectoryX(degrees, distance));
        Assert.Equal(result.Y, Point.Zero.TrajectoryY(degrees, distance));
    }
}
