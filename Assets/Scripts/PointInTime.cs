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
    public string playerAnimationName;
    public string opponentAnimationName;
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
            playerAttackFinished = false;
            playerAnimationName = null;
            playerEvent = false;
        }
        else
        {
            opponentAction = nullAction;
            opponentState = PlayerState.Null;
            opponentAttackFinished = false;
            opponentAnimationName = null;
            opponentEvent = false;
        }
    }
}