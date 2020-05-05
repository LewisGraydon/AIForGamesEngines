using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EGameState 
{
    
    setupState,
    playerSetup,
    playerTurn,
    enemySetup,
    enemyTurn,
    movement,
    beeAttack, // Maybe rename this eventually
    victoryScreen,
    failureScreen
}
