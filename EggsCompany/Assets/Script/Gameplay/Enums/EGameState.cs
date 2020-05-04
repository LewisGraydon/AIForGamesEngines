using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EGameState 
{
    setupState,
    playerTurn,
    enemyTurn,
    movement,
    beeAttack, // Maybe rename this eventually
    victoryScreen,
    failureScreen,
    enemySetup
}
