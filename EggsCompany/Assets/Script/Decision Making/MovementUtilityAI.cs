using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementUtilityAI : MonoBehaviour
{

    // needs to take into account, self cover, if flanking enemy, pip cost, shouldBeShootingPosition 

    // so... Clamp(0, 1, AverageOf((1 - pipCost) + (isFlanking) + (isInCover) + (isShootingPosition)))
    public void ConsiderTiles(CharacterBase unitConsideringTiles, Tile[] tiles)
    {
        int bestTileIndex = 0;
        float bestTileFloat = 0.0f;
        for (int i = 0; i < tiles.Length; i++)
        {
            float tileConsidered = Consider(unitConsideringTiles, tiles[i]);
            if (tileConsidered >= bestTileFloat)
            {
                bestTileIndex = i;
                bestTileFloat = tileConsidered;
            }
        }
    }

    public float Consider(CharacterBase unitConsideringTile, Tile tile)
    {
        float tileValue = 0.0f;
        foreach(Consideration c in ConsiderationLists.moveConsiderationList)
        {
            tileValue += c.ConsiderTile(unitConsideringTile, tile);
        }
        return tileValue;
    }
}
