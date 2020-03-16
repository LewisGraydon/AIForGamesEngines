﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    protected bool onPlayerTeam;
    public bool getOnPlayerTeam
    {
        get { return onPlayerTeam; }
    }

    protected int actionPips = 2;
    public int remainingPips
    {
        get
        {
            return actionPips;
        }
    }

    protected int health = 6;
    public int remainingHealth
    {
        get { return health; }
    }

    protected int maxHealthValue = 6;
    public int maximumHealth
    {
        get { return health; }
    }

    protected List<CharacterBase> _enemiesInSight = new List<CharacterBase>();
    public List<CharacterBase> enemiesInSight
    {
        get
        {
            return _enemiesInSight;
        }
    }

    public bool isInCover(CharacterBase fromEnemy)
    {
        return false;
    }

    public float sightDistance = 0.0f;

    public Tile occupiedTile;

    public int remainingShots;

    public int maxShots;
}
