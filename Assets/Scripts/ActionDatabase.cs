using System.Collections.Generic;
using UnityEngine;

public class ActionDatabase : MonoBehaviour
{
    public static List<Action> cardList = new List<Action>();

    void Awake()
    {
        //cardList.Add(new Action(0, "None", CardType.Attack, 0, 0, 0, "None", Resources.Load<Sprite>(""), Player.Player));
        cardList.Add(new AttackAction(1, PlayerState.Jab, "Jab", AttackType.Jab, 350, 100, 4, 75, PlayerType.Player));
        cardList.Add(new AttackAction(2, PlayerState.Cross, "Cross", AttackType.Cross, 450, 200, 6, 125, PlayerType.Player));
        cardList.Add(new AttackAction(3, PlayerState.Hook, "Lead Hook", AttackType.LeadHook, 450, 200, 6, 100, PlayerType.Player));
        cardList.Add(new AttackAction(4, PlayerState.Uppercut, "Rear Uppercut", AttackType.RearUppercut, 600, 300, 8, 200, PlayerType.Player));
        //cardList.Add(new Card(4, "Right Hook", 2, 3, "Right Hook", Resources.Load<Sprite>("RightHook")));
        cardList.Add(new DefenceAction(5, PlayerState.Bob, "Bob", DefenceType.Bob, 200, 100, PlayerType.Player));
        cardList.Add(new DefenceAction(6, PlayerState.Slip, "Slip", DefenceType.Slip, 200, 100, PlayerType.Player));
        cardList.Add(new DefenceAction(7, PlayerState.Block, "Block", DefenceType.Block, 150, 100, PlayerType.Player));

        //cardList.Add(new AttackAction(0, "None", CardType.Attack, 0, 0, 0, "None", Resources.Load<Sprite>(""), Player.Opponent));
        cardList.Add(new AttackAction(1, PlayerState.Jab, "Jab", AttackType.Jab, 350, 100, 4, 75, PlayerType.Opponent));
        cardList.Add(new AttackAction(2, PlayerState.Cross, "Cross", AttackType.Cross, 450, 200, 6, 125, PlayerType.Opponent));
        cardList.Add(new AttackAction(3, PlayerState.Hook, "Lead Hook", AttackType.LeadHook, 450, 200, 6, 100, PlayerType.Opponent));
        cardList.Add(new AttackAction(4, PlayerState.Uppercut, "Rear Uppercut", AttackType.RearUppercut, 600, 300, 8, 200, PlayerType.Opponent));
        //cardList.Add(new Card(4, "Right Hook", 2, 3, "Right Hook", Resources.Load<Sprite>("RightHook")));
        cardList.Add(new DefenceAction(5, PlayerState.Bob, "Bob", DefenceType.Bob, 200, 100, PlayerType.Opponent));
        cardList.Add(new DefenceAction(6, PlayerState.Slip, "Slip", DefenceType.Slip, 200, 100, PlayerType.Opponent));
        cardList.Add(new DefenceAction(7, PlayerState.Block, "Block", DefenceType.Block, 150, 100, PlayerType.Opponent));
        //Debug.Log(cardList.Count);
    }

    public Action GetCard(string name, PlayerType player)
    {
        foreach (Action card in cardList) 
        {
            if (card.player == player)
            {
                if (card.name == name)
                {
                    return card;
                }
            } 
        }
        Debug.Log(name + " card not found");
        return null;
    }

    public List<Action> GetActionsOfType(CardType type, PlayerType player)
    {
        List<Action> actions = new List<Action>();
        foreach (Action card in cardList)
        {
            if (card.player == player && card.type == type) actions.Add(card);
        }
        return actions;
    }
}
