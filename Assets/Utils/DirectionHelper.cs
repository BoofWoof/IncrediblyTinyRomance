using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum Directions
{
    Up,
    Down,
    Left,
    Right,
    UpperLeft,
    UpperRight,
    LowerLeft,
    LowerRight,
    NULL
}
public static class DirectionHelper
{
    static public Directions[] CardinalDirections = new Directions[]
    {
        Directions.Up, Directions.Down, Directions.Left, Directions.Right
    };

    static public Directions[] DiagonalDirections = new Directions[]
    {
        Directions.UpperLeft, Directions.UpperRight, Directions.LowerLeft, Directions.LowerRight
    };
    static public Directions[] AllDirections = new Directions[]
    {
        Directions.Up, Directions.Down, Directions.Left, Directions.Right,
        Directions.UpperLeft, Directions.UpperRight, Directions.LowerLeft, Directions.LowerRight
    };

    static Dictionary<Directions, Directions> FlipDict = new Dictionary<Directions, Directions>
    {
        { Directions.Up, Directions.Down},
        { Directions.Down, Directions.Up},
        { Directions.Left, Directions.Right},
        { Directions.Right, Directions.Left},
        { Directions.UpperLeft, Directions.LowerRight},
        { Directions.LowerRight, Directions.UpperLeft},
        { Directions.UpperRight, Directions.LowerLeft},
        { Directions.LowerLeft, Directions.UpperRight},
        { Directions.NULL, Directions.NULL}
    };
    static Dictionary<Directions, Directions> ClockwiseDict = new Dictionary<Directions, Directions>
    {
        { Directions.Up, Directions.Right},
        { Directions.Down, Directions.Left},
        { Directions.Left, Directions.Up},
        { Directions.Right, Directions.Down},
        { Directions.UpperLeft, Directions.UpperRight},
        { Directions.LowerRight, Directions.LowerLeft},
        { Directions.UpperRight, Directions.LowerRight},
        { Directions.LowerLeft, Directions.UpperLeft},
        { Directions.NULL, Directions.NULL}
    };
    static Dictionary<Directions, Directions> CounterClockwiseDict = new Dictionary<Directions, Directions>
    {
        { Directions.Up, Directions.Left},
        { Directions.Down, Directions.Right},
        { Directions.Left, Directions.Down},
        { Directions.Right, Directions.Up},
        { Directions.UpperLeft, Directions.LowerLeft},
        { Directions.LowerRight, Directions.UpperRight},
        { Directions.UpperRight, Directions.UpperLeft},
        { Directions.LowerLeft, Directions.LowerRight},
        { Directions.NULL, Directions.NULL}
    };
    static Dictionary<Directions, Vector2Int> CordShiftDict = new Dictionary<Directions, Vector2Int>
    {
        { Directions.Up, new Vector2Int(0, 1)},
        { Directions.Down, new Vector2Int(0, -1)},
        { Directions.Left, new Vector2Int(-1, 0)},
        { Directions.Right, new Vector2Int(1, 0)},
        { Directions.UpperLeft, new Vector2Int(-1, 1)},
        { Directions.LowerRight, new Vector2Int(1, -1)},
        { Directions.UpperRight, new Vector2Int(1, 1)},
        { Directions.LowerLeft, new Vector2Int(-1, -1)},
        { Directions.NULL, new Vector2Int(0, 0)}
    };

    public static Directions FlipDirection(this Directions currentDirection)
    {
        return FlipDict[currentDirection];
    }
    public static Directions TurnClockwise(this Directions currentDirection)
    {
        return ClockwiseDict[currentDirection];
    }
    public static Directions TurnCounterClockwise(this Directions currentDirection)
    {
        return CounterClockwiseDict[currentDirection];
    }
    public static Vector2Int ToCordShift(this Directions currentDirection)
    {
        return CordShiftDict[currentDirection];
    }
}
