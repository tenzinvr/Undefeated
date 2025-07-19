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
    public bool playerAnimationStarted;
    public bool opponentAnimationStarted;
    public bool playerAttackFinished;
    public bool opponentAttackFinished;

    public bool hasTakenEffect = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void SwitchPlayerTargets()
    {
        //playerTarget.SetActive(!playerTarget.activeInHierarchy);
        //opponentTarget.SetActive(!opponentTarget.activeInHierarchy);
    }

    public void AddToPoint(Action newAction, PlayerType player)
    {
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
}