using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileSet", menuName = "ScriptableObjects/TileSet", order = 1)]
public class TileSetSO : ScriptableObject
{

    [System.Serializable]
    public class TileEntry
    {
        public int bitmaskValue; // Unique wall bitmask (0-15)
        public Sprite tileSprite; // Corresponding sprite
    }

    /*
        Bitmask Reference for Tile Types:

        Binary  |  Decimal  |  Tile Type
        --------------------------------
        0000    |    0     |  No walls (empty tile)
        0001    |    1     |  Wall on North
        0010    |    2     |  Wall on East
        0011    |    3     |  Corner (North-East)
        0100    |    4     |  Wall on South
        0101    |    5     |  Vertical Corridor (North-South)
        0110    |    6     |  Corner (South-East)
        0111    |    7     |  T-Junction (North-East-South)
        1000    |    8     |  Wall on West
        1001    |    9     |  Corner (North-West)
        1010    |   10     |  Horizontal Corridor (East-West)
        1011    |   11     |  T-Junction (North-East-West)
        1100    |   12     |  Corner (South-West)
        1101    |   13     |  T-Junction (North-South-West)
        1110    |   14     |  T-Junction (East-South-West)
        1111    |   15     |  Closed Room (All Walls)
    */

    public TileEntry[] tileEntries;

    // Returns the correct sprite for a given bitmask
    public Sprite BitMaskToSprite(int bitmask)
    {
        foreach (var entry in tileEntries)
        {
            if (entry.bitmaskValue == bitmask)
                return entry.tileSprite;
        }
        return null; // Default case (optional)
    }
    public Sprite GetSprite(bool topWall, bool bottomWall, bool leftWall, bool rightWall)
    {
        int bitmask = 0;
        if (topWall) bitmask += 1;  // North
        if (rightWall) bitmask += 2;  // East
        if (bottomWall) bitmask += 4;  // South
        if (leftWall) bitmask += 8;  // West

        Sprite tileSprite = BitMaskToSprite(bitmask);
        return tileSprite;
    }
}
