using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class OpponentBehaviour : MonoBehaviour
{
    private List<Action> hand = new List<Action>();
    private int currentIndex;
    private PointInTime nextActivePoint;
    private Action nextPlayerAction;
    private Action previousAction;
    [SerializeField] private Action breatheAction;
    private bool cardFound;

    private DeckManager deckManager;
    private PlayManager playManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deckManager = GetComponentInChildren<DeckManager>();
        playManager = GameObject.FindGameObjectWithTag("Timeline").GetComponent<PlayManager>();
        Debug.Log("opponent start");
        hand = deckManager.cardsInHand;
    }

    public void StartTurn()
    {
        hand = deckManager.cardsInHand;
        CheckForCreatine();
        cardFound = false;
        currentIndex = playManager.GetLastActiveIndex(PlayerType.Opponent);
        nextActivePoint = playManager.GetNextActivePointForPlayer(PlayerType.Player);
        if (nextActivePoint == null)
        {
            DecideOnAttack();
            return;
        }
        nextPlayerAction = nextActivePoint.playerAction;
        if (nextPlayerAction is AttackAction playerAttack)
        {
            Debug.Log("Next player action is attack " + playerAttack.name);
            foreach (Action action in hand)
            {
                if (action is DefenceAction defenceAction)
                {
                    playManager.attackDefenceEffects.TryGetValue((playerAttack.name, defenceAction.name), out Effect effect);
                    if (effect == Effect.Missed)
                    {
                        int endPoint = playerAttack.timeOfEffect / 50;
                        int difference = defenceAction.windUpTime / 50;
                        if (currentIndex + difference <= endPoint)
                        {
                            PlaceDefence(playerAttack, action);
                            return;
                        }
                    }
                }
            }
            TryAttackBeforePlayer(playerAttack);
        }
        else if (hand.Count <= 5) { Debug.Log("Too few cards, breathe"); Breathe(); }
        else
        {
            DecideOnAttack();
        }
        if (!cardFound)
        {
            Debug.Log("Card not found breathe");
            Breathe();
        }
    }

    private IEnumerator WaitToPlay()
    {
        yield return new WaitForSeconds(1);

    }

    private void CheckForCreatine()
    {
        foreach (Action action in hand)
        {
            if (action.name == "Creatine")
            {
                if (action is SpecialAction creatineAction)
                {
                    creatineAction.Effect();
                }
            }
        }
    }

    private void CheckForSpecials()
    {
        foreach (Action action in hand)
        {
            if (action is SpecialAction specialAction)
            {

            }
        }
    }

    private void PlaceDefence(Action attack, Action defence)
    {
        Debug.Log("Place defence");
        cardFound = true;
        int endPoint = attack.timeOfEffect / 50;
        int difference = defence.windUpTime / 50;
        playManager.AddActionToSpecificPoint(defence, endPoint - difference);
        playManager.EndTurn();
    }

    private bool TryAttackBeforePlayer(AttackAction playerAttack)
    {
        Debug.Log("Attack before player");
        foreach (Action action in hand)
        {
            if (action is AttackAction attackAction)
            {
                if (currentIndex + (attackAction.windUpTime / 50) <= playerAttack.timeOfEffect / 50)
                {
                    PlayCard(action);
                    return true;
                }
            }
        }
        return false;
    }

    private void DecideOnAttack()
    {
        Debug.Log("Decide on attak");
        if (previousAction is AttackAction prevAttack)
        {
            foreach (Action action in hand)
            {
                if (action is AttackAction attackAction)
                {
                    playManager.actionSynergy.TryGetValue((previousAction.name, action.name), out bool synergise);
                    if (synergise)
                    {
                        Debug.Log("Found attack synergy");
                        PlayCard(action);
                        return;
                    }
                }
            }
        }
        else
        {
            foreach (Action action in hand)
            {
                if (action is AttackAction attackAction)
                {
                    PlayCard(action);
                    return;
                }
            }
        }
    }

    private void PlayCard(Action action)
    {
        Debug.Log("Play card");
        cardFound = true;
        playManager.AddActionToTurn(action);
        previousAction = action;
        playManager.EndTurn();
    }

    private void Breathe()
    {
        playManager.AddActionToTurn(breatheAction);
        playManager.EndTurn();
    }
}
