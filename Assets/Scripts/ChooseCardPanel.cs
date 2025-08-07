using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class ChooseCardPanel : MonoBehaviour
{
    [SerializeField] private PlayerType player;
    private CardDatabase cardDatabase;
    [SerializeField] private GameObject chooseCardPrefab;
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private GameObject panel;
    [SerializeField] private string[] specialCards;
    [SerializeField] private int numberOfCards = 2;
    private List<string> selectedCards = new List<string>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cardDatabase = GameObject.FindAnyObjectByType<CardDatabase>();
        Time.timeScale = 0;
        Action action = new Action(player);
        foreach (string  cardName in specialCards)
        {
            //Debug.Log("Add card + " + cardName);
            action = cardDatabase.GetCard(cardName, player);
            //Debug.Log("Got card " + action.name);
            AddCardToPanel(panel.transform, action);
        }
    }

    public void AddCardToPanel(Transform panelTransform, Action action)
    {
        //Debug.Log("Add card " + action.name + " to panel " + panelTransform.name);
        GameObject cardObj = Instantiate(chooseCardPrefab, panel.transform);
        ChooseCardBehaviour cardBehaviour = cardObj.GetComponent<ChooseCardBehaviour>();
        if (cardBehaviour != null)
        {
            //cardBehaviour.displayId = drawnCard.id;
            cardBehaviour.SetAction(action);
        }
        else { Debug.Log("Card behaviour null"); }
    }

    public bool CanCardBeSelected()
    {
        return (selectedCards.Count < numberOfCards);
    }

    public void CardSelected(string cardName)
    {
        if (CanCardBeSelected() && !selectedCards.Contains(cardName))
        {
            //Debug.Log(cardName + " selected");
            selectedCards.Add(cardName);
        }
    }

    public void OnStartClick()
    {
        foreach (string cardName in selectedCards)
        {
            deckManager.AddCardToDeck(cardName, 2);
        }
        deckManager.Shuffle();
        deckManager.DrawCards();
        gameObject.SetActive(false);
    }
}
