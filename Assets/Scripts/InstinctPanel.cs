using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class InstinctPanel : MonoBehaviour
{
    [SerializeField] private PlayerType playerType;
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private GameObject attackPanel;
    [SerializeField] private GameObject defencePanel;

    public void DisplayPanel()
    {
        Debug.Log("Displaying instinct panel for " + playerType);
        List<Action> attackActions = deckManager.GetActionsOfType(CardType.Attack, playerType);
        List<Action> defenceActions = deckManager.GetActionsOfType(CardType.Defence, playerType);
        foreach (Action action in attackActions)
        {
            deckManager.AddCardToPanel(attackPanel.transform, action);
        }
        foreach (Action action in defenceActions)
        {
            deckManager.AddCardToPanel(defencePanel.transform, action);
        }
    }
}
