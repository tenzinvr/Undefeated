using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public static List<Action> cardList = new List<Action>();

    void Awake()
    {
        cardList.Add(new Action(PlayerType.Player, PlayerState.Idle, 0));
        cardList.Add(new AttackAction(1, PlayerState.Jab, "Jab", AttackType.Jab, Hand.Lead, AttackRange.Ranged, 350, 100, 4, 20, 75, PlayerType.Player));
        cardList.Add(new AttackAction(2, PlayerState.Cross, "Cross", AttackType.Cross, Hand.Rear, AttackRange.Ranged, 450, 200, 6, 20, 125, PlayerType.Player));
        cardList.Add(new AttackAction(3, PlayerState.Hook, "Lead Hook", AttackType.LeadHook, Hand.Lead, AttackRange.Mid, 450, 200, 6, 20, 100, PlayerType.Player));
        cardList.Add(new AttackAction(4, PlayerState.Hook, "Rear Hook", AttackType.RearHook, Hand.Rear, AttackRange.Mid, 450, 200, 6, 20, 100, PlayerType.Player));
        cardList.Add(new AttackAction(5, PlayerState.Uppercut, "Lead Uppercut", AttackType.LeadUppercut, Hand.Lead, AttackRange.Pocket, 600, 300, 8, 20, 200, PlayerType.Player));
        cardList.Add(new AttackAction(6, PlayerState.Uppercut, "Rear Uppercut", AttackType.RearUppercut, Hand.Rear, AttackRange.Pocket, 600, 300, 8, 20, 200, PlayerType.Player));
        //cardList.Add(new AttackAction(6, PlayerState.Feint, "Feint", AttackType.Feint, Hand.Rear, AttackRange.Pocket, 150, 0, 0, 0, 0, PlayerType.Player));

        cardList.Add(new DefenceAction(7, PlayerState.Bob, "Bob", DefenceType.Bob, 200, 100, PlayerType.Player));
        cardList.Add(new DefenceAction(8, PlayerState.Slip, "Slip", DefenceType.Slip, 200, 100, PlayerType.Player));
        cardList.Add(new DefenceAction(9, PlayerState.Block, "Block", DefenceType.Block, 150, 100, PlayerType.Player));

        cardList.Add(new Action(10, PlayerState.Breathing, CardType.Breathing, "Breathe", 300, 0, PlayerType.Player));
        //cardList.Add(new Action(11, PlayerState.Breathing, CardType.Breathing, "Deep Breathe", 500, 0, PlayerType.Player));


        cardList.Add(new SpecialAction(12, "Creatine", "Retrieve 5 cards", PlayerType.Player));
        cardList.Add(new SpecialAction(13, "Protein Shake", "Next attack does +5 damage", PlayerType.Player));
        cardList.Add(new SpecialAction(14, "Wild Swing", "Next attack is a CRITICAL, attack cannot combo", PlayerType.Player));
        cardList.Add(new SpecialAction(15, "Instinct", "Can be played as any attack or defence card", PlayerType.Player));
        cardList.Add(new SpecialAction(16, "Flesh Wound", "Remove all stagger", PlayerType.Player));
        cardList.Add(new SpecialAction(17, "Hard Head", "Remove stun", PlayerType.Player));
        cardList.Add(new SpecialAction(18, "Feint", "Play on top of an attack, retrieve the attack card", PlayerType.Player));
        cardList.Add(new SpecialAction(19, "Coach's Advice", "Search your draw pile for a non-special card", PlayerType.Player));
        cardList.Add(new SpecialAction(20, "Sheer Dumb Luck", "Next attack has +50% crit chance", PlayerType.Player));
        cardList.Add(new SpecialAction(21, "Caffeine", "Next card has wind up time reduced by 100ms", PlayerType.Player));
        cardList.Add(new SpecialAction(22, "Copy cat", "Search your opponents discard pile for a non-special card, place it in your hand", PlayerType.Player));


        cardList.Add(new Action(PlayerType.Opponent, PlayerState.Idle, 0));
        //cardList.Add(new AttackAction(0, "None", CardType.Attack, 0, 0, 0, "None", Resources.Load<Sprite>(""), Player.Opponent));
        cardList.Add(new AttackAction(1, PlayerState.Jab, "Jab", AttackType.Jab, Hand.Lead, AttackRange.Ranged, 350, 100, 4, 50, 75, PlayerType.Opponent));
        cardList.Add(new AttackAction(2, PlayerState.Cross, "Cross", AttackType.Cross, Hand.Rear, AttackRange.Ranged, 450, 200, 6, 50, 125, PlayerType.Opponent));
        cardList.Add(new AttackAction(3, PlayerState.Hook, "Lead Hook", AttackType.LeadHook, Hand.Lead, AttackRange.Mid, 450, 200, 6, 50, 100, PlayerType.Opponent));
        cardList.Add(new AttackAction(4, PlayerState.Hook, "Rear Hook", AttackType.RearHook, Hand.Rear, AttackRange.Mid, 450, 200, 6, 50, 100, PlayerType.Opponent));
        cardList.Add(new AttackAction(5, PlayerState.Uppercut, "Lead Uppercut", AttackType.LeadUppercut, Hand.Lead, AttackRange.Pocket, 600, 300, 8, 50, 200, PlayerType.Opponent));
        cardList.Add(new AttackAction(6, PlayerState.Uppercut, "Rear Uppercut", AttackType.RearUppercut, Hand.Rear, AttackRange.Pocket, 600, 300, 8, 50, 200, PlayerType.Opponent));
        //cardList.Add(new AttackAction(6, PlayerState.Feint, "Feint", AttackType.Feint, Hand.Rear, AttackRange.Pocket, 150, 0, 0, 0, 0, PlayerType.Opponent));
        
        cardList.Add(new DefenceAction(7, PlayerState.Bob, "Bob", DefenceType.Bob, 200, 100, PlayerType.Opponent));
        cardList.Add(new DefenceAction(8, PlayerState.Slip, "Slip", DefenceType.Slip, 200, 100, PlayerType.Opponent));
        cardList.Add(new DefenceAction(9, PlayerState.Block, "Block", DefenceType.Block, 150, 100, PlayerType.Opponent));


        cardList.Add(new Action(10, PlayerState.Breathing, CardType.Breathing, "Breathe", 200, 0, PlayerType.Opponent));
        //cardList.Add(new Action(11, PlayerState.Breathing, CardType.Breathing, "Deep Breathe", 500, 0, PlayerType.Opponent));

        cardList.Add(new SpecialAction(12, "Creatine", "Retrieve 5 cards", PlayerType.Opponent));
        cardList.Add(new SpecialAction(13, "Protein Shake", "Next attack does +5 damage", PlayerType.Opponent));
        cardList.Add(new SpecialAction(14, "Wild Swing", "Next attack is a CRITICAL, attack cannot combo", PlayerType.Opponent));
        cardList.Add(new SpecialAction(15, "Instinct", "Can be played as any attack or defence card", PlayerType.Opponent));
        cardList.Add(new SpecialAction(16, "Flesh Wound", "Remove all stagger", PlayerType.Opponent));
        cardList.Add(new SpecialAction(17, "Hard Head", "Remove stun", PlayerType.Opponent));
        cardList.Add(new SpecialAction(18, "Feint", "Play on top of an attack, retrieve the attack card", PlayerType.Opponent));
        cardList.Add(new SpecialAction(19, "Coach's Advice", "Search your draw pile for a non-special card", PlayerType.Opponent));
        cardList.Add(new SpecialAction(20, "Sheer Dumb Luck", "Next attack has +50% crit chance", PlayerType.Opponent));
        cardList.Add(new SpecialAction(21, "Caffeine", "Next card has wind up time reduced by 100ms", PlayerType.Opponent));
        cardList.Add(new SpecialAction(22, "Copy cat", "Search your opponents discard pile for a non-special card, place it in your hand", PlayerType.Opponent));
        //Debug.Log(cardList.Count);
    }

    public Action GetCard(string name, PlayerType player)
    {
        foreach (Action card in cardList) 
        {
            if (card.playerType == player)
            {
                if (card.name == name)
                {
                    //Debug.Log("Returning card " + card.name);
                    return card;
                }
            } 
        }
        Debug.Log(name + " card not found");
        return null;
    }

    public List<Action> GetActionsOfType(CardType type, PlayerType player)
    {
        //Debug.Log("Get actions of type = " +  type);
        List<Action> actions = new List<Action>();
        foreach (Action card in cardList)
        {
            if (card.playerType == player && card.type == type) actions.Add(card);
        }
        return actions;
    }

    //public List<SpecialAction> GetSpecials(PlayerType player)
    //{
    //    List<SpecialAction> specialActions = new List<SpecialAction>();
    //    foreach (SpecialAction card in specialCardList)
    //    {
    //        if (card.player == player) specialActions.Add(card);
    //    }
    //    return specialActions;
    //}
}
