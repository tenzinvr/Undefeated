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
    [SerializeField] private int feintChance = 50; // Chance to play feint, 0-100

    private DeckManager deckManager;
    private PlayManager playManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deckManager = GetComponentInChildren<DeckManager>();
        playManager = GameObject.FindGameObjectWithTag("Timeline").GetComponent<PlayManager>();
        //Debug.Log("opponent start");
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
            //Debug.Log("Next player action is attack " + playerAttack.name);
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
            playManager.EndTurn();
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
                    return;
                }
            }
        }
        //Debug.Log("Creatine not found");
    }

    private void CheckForSpecials()
    {
        Debug.Log("Check for specials");
        foreach (Action action in hand)
        {
            if (action is SpecialAction specialAction)
            {
            }
        }
    }

    private void PlaceDefence(Action attack, Action defence)
    {
        //Debug.Log("Place defence");
        cardFound = true;
        int endPoint = attack.timeOfEffect / 50;
        int difference = defence.windUpTime / 50;
        playManager.AddActionToSpecificPoint(defence, endPoint - difference);
        playManager.EndTurn();
    }

    private bool TryAttackBeforePlayer(AttackAction playerAttack)
    {
        //Debug.Log("Attack before player");
        foreach (Action action in hand)
        {
            if (action is AttackAction attackAction)
            {
                if (currentIndex + (attackAction.windUpTime / 50) <= playerAttack.timeOfEffect / 50)
                {
                    PlayCard(action);
                    playManager.EndTurn();
                    return true;
                }
            }
        }
        return false;
    }

    private void DecideOnAttack()
    {
        //Debug.Log("Decide on attak");
        if (previousAction is AttackAction prevAttack)
        {
            foreach (Action action in hand)
            {
                if (action is AttackAction attackAction)
                {
                    playManager.actionSynergy.TryGetValue((previousAction.name, action.name), out bool synergise);
                    if (synergise)
                    {
                        PlayCard(action);
                        if (!DecideOnFeint())
                        {
                            CheckForSpecials();
                        }
                        playManager.EndTurn();
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
                    playManager.EndTurn();
                    return;
                }
            }
        }
    }

    private bool DecideOnFeint()
    {
        Debug.Log("Decide on feint");
        foreach (Action action in hand)
        {
            if (action.name == "Feint" && action is SpecialAction feintAction)
            {
                if (Random.Range(0, 100) < feintChance)
                {
                    Feint();
                    return true;
                }
                return false;
            }
        }
        return false;
    }


    private void PlayCard(Action action)
    {
        //Debug.Log("Play card");
        cardFound = true;
        playManager.AddActionToTurn(action);
        previousAction = action;
    }

    private void Breathe()
    {
        playManager.AddActionToTurn(breatheAction);
    }

    private void Feint()
    {
        Action feintAction = new AttackAction(6, PlayerState.Feint, "Feint", AttackType.Feint, Hand.Rear, AttackRange.Pocket, 150, 0, 0, 0, 0, PlayerType.Opponent);
        playManager.AddActionToTurn(feintAction);
    }
}
