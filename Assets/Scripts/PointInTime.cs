using NUnit.Framework.Constraints;
using UnityEngine;

public class PointInTime : MonoBehaviour
{
    public GameObject playerTarget;
    public GameObject opponentTarget;
    public PlayerState playerState;
    public PlayerState opponentState;
    public Action playerAction;
    public Action opponentAction;
    public int time;
    public int index;
    public bool actionStart;
    public bool playerAnimationStarted;
    public bool opponentAnimationStarted;
    public bool playerAttackFinished;
    public bool opponentAttackFinished;
    public bool playerEvent;
    public bool opponentEvent;

    public bool evaluated = false;


    public void AddToPoint(Action newAction, PlayerType player)
    {
        //Debug.Log("Add to point, " + index + " state = " +  newAction.playerState);
        if (player == PlayerType.Player)
        {
            playerAction = newAction;
            playerState = newAction.playerState;
        }
        else
        {
            opponentAction = newAction;
            opponentState = newAction.playerState;
        }
    }

    public void AddToPoint(Action newAction, PlayerType player, PlayerState state)
    {
        AddToPoint(newAction, player);
        if (player == PlayerType.Player)
        {
            playerState = state;
        }
        else
        {
            opponentState = state;
        }
    }

    public void ClearPoint(PlayerType player)
    {
        Action nullAction = new Action(player);
        if (player == PlayerType.Player)
        {
            playerAction = nullAction;
            playerState = PlayerState.Null;
            playerAnimationStarted = false;
            playerAttackFinished = false;
            playerEvent = false;
        }
        else
        {
            opponentAction = nullAction;
            opponentState = PlayerState.Null;
            opponentAnimationStarted = false;
            opponentAttackFinished = false;
            opponentEvent = false;
        }
    }
}