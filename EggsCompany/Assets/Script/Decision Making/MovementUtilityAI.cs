using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementUtilityAI : MonoBehaviour
{

    // needs to take into account, self cover, if flanking enemy, pip cost, shouldBeShootingPosition 

    // so... Clamp(0, 1, AverageOf((1 - pipCost) + (isFlanking) + (isInCover) + (isShootingPosition)))
    public void ConsiderTiles(Tile[] tiles)
    {
        int bestTileIndex = 0;
        float bestTileFloat = 0.0f;
        for (int i = 0; i < tiles.Length; i++)
        {
            float tileConsidered = Consider(tiles[0]);
            if (tileConsidered >= bestTileFloat)
            {
                bestTileIndex = i;
                bestTileFloat = tileConsidered;
            }
        }
    }

    public float Consider(Tile tile)
    {
        return 0.0f;
    }
}
