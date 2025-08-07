using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Deck", menuName = "Deck")]
public class DeckManager : ScriptableObject
{
    [SerializeField] private CardDatabase cardDatabase;
    public Stack<Action> deck = new Stack<Action>();
    public Stack<Action> drawPile = new Stack<Action>();
    public Stack<Action> discardPile = new Stack<Action>();
    public SpecialAction[] specialCards;
    public List<Action> cardsInPlay = new List<Action>();
    public List<Action> cardsInHand = new List<Action>();
    private List<GameObject> usedCards;
    [System.NonSerialized] public List<GameObject> icons = new List<GameObject>();

    [SerializeField] private GameObject deckPanel;
    [SerializeField] private HandPanel handPanel;
    [SerializeField] private GameObject discardPanel;
    //[SerializeField] private GameObject displayPlayedCardPanel;
    private int currentCardIndex;

    public GameObject instinctPanel;

    [SerializeField] private PlayerType player = PlayerType.Player;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject specialCardPrefab;

    public Action feintedAction;

    //public List<Card> container = new List<Card>();
    [SerializeField] private int deckSize;
    [SerializeField] private int handSize;

    private TimelineManager timelineManager;


    public void Initiate()
    {
        cardDatabase = GameObject.FindGameObjectWithTag("CardDatabase").GetComponent<CardDatabase>();
        //Debug.Log("Card database null? " + (cardDatabase == null));
        timelineManager = GameObject.FindGameObjectWithTag("Timeline").GetComponent<TimelineManager>();
        handPanel = player == PlayerType.Player ? GameObject.FindGameObjectWithTag("Hand").GetComponent<HandPanel>() : GameObject.FindGameObjectWithTag("OpponentHand").GetComponent<HandPanel>();
        Debug.Log(handPanel.name);
        instinctPanel = GameObject.FindGameObjectWithTag("InstinctPanel");
        discardPanel = GameObject.FindGameObjectWithTag("Discard");
        usedCards = new List<GameObject>();
        cardsInHand.Clear();

        deck.Push(cardDatabase.GetCard("Jab", player));
        deck.Push(cardDatabase.GetCard("Jab", player));
        deck.Push(cardDatabase.GetCard("Jab", player));
        deck.Push(cardDatabase.GetCard("Jab", player));
        deck.Push(cardDatabase.GetCard("Jab", player));
        deck.Push(cardDatabase.GetCard("Jab", player));
        deck.Push(cardDatabase.GetCard("Cross", player));
        deck.Push(cardDatabase.GetCard("Cross", player));
        deck.Push(cardDatabase.GetCard("Cross", player));
        deck.Push(cardDatabase.GetCard("Cross", player));
        deck.Push(cardDatabase.GetCard("Cross", player));
        deck.Push(cardDatabase.GetCard("Lead Hook", player));
        deck.Push(cardDatabase.GetCard("Lead Hook", player));
        deck.Push(cardDatabase.GetCard("Lead Hook", player));
        deck.Push(cardDatabase.GetCard("Rear Hook", player));
        deck.Push(cardDatabase.GetCard("Rear Hook", player));
        deck.Push(cardDatabase.GetCard("Rear Hook", player));
        deck.Push(cardDatabase.GetCard("Lead Uppercut", player));
        deck.Push(cardDatabase.GetCard("Lead Uppercut", player));
        deck.Push(cardDatabase.GetCard("Rear Uppercut", player));
        deck.Push(cardDatabase.GetCard("Rear Uppercut", player));
        deck.Push(cardDatabase.GetCard("Bob", player));
        deck.Push(cardDatabase.GetCard("Bob", player));
        deck.Push(cardDatabase.GetCard("Bob", player));
        deck.Push(cardDatabase.GetCard("Slip", player));
        deck.Push(cardDatabase.GetCard("Slip", player));
        deck.Push(cardDatabase.GetCard("Slip", player));
        deck.Push(cardDatabase.GetCard("Slip", player));
        deck.Push(cardDatabase.GetCard("Slip", player));
        deck.Push(cardDatabase.GetCard("Slip", player));

        deck.Push(cardDatabase.GetCard("Feint", player));
        deck.Push(cardDatabase.GetCard("Feint", player));
        deck.Push(cardDatabase.GetCard("Feint", player));
        deck.Push(cardDatabase.GetCard("Feint", player));
        deck.Push(cardDatabase.GetCard("Feint", player));
        deck.Push(cardDatabase.GetCard("Feint", player));

        deckSize = deck.Count;
    }

    public void AddCardToDeck(string name, int number)
    {
        for (int i = 0; i <= number; i++)
        {
            deck.Push(cardDatabase.GetCard(name, player));
        }
    }

    public void Shuffle()
    {
        Action[] temp = deck.ToArray();
        Array.Reverse(temp); // Optional but recommended to preserve stack order logic

        for (int i = temp.Length - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            (temp[i], temp[randomIndex]) = (temp[randomIndex], temp[i]);
        }

        // Refill drawPile
        drawPile.Clear(); // Optional if not empty
        for (int i = 0; i < temp.Length; i++)
        {
            drawPile.Push(temp[i]);
        }
    }

    private void Reshuffle()
    {
        //Debug.Log("Reshuffling deck");
        drawPile.Clear();
        while (discardPile.Count > 0)
        {
            Action action = discardPile.Pop();
            drawPile.Push(action);
        }
        Shuffle();
    }

    public void DrawCards(int numberOfCards = 7)
    {
        //Debug.Log("Num cards to draw " + numberOfCards);
        for (int i = 0; i <= numberOfCards; i++)
        {
            if (drawPile.Count == 0)
            {
                Debug.Log("No more cards to draw");
                Reshuffle();
            }
            Action drawnCard = drawPile.Pop();
            //Debug.Log(drawnCard == null);
            AddCardToHand(drawnCard);
            cardsInHand.Add(drawnCard);
            //Debug.Log(cardsInHand.Count);
        }
    }

    public void AddCardToHand(Action action)
    {
        //Debug.Log("Adding card to hand: " + action.name);
        GameObject cardObj;
        //Debug.Log("Card type: " + action.type.ToString());
        //Debug.Log("Hand panel null? " + (handPanel == null).ToString());
        if (action.type != CardType.Special) cardObj = Instantiate(cardPrefab, handPanel.transform);
        else cardObj = Instantiate(specialCardPrefab, handPanel.transform);
        handPanel.AddCard(cardObj);
        CardBehaviour cardBehaviour = cardObj.GetComponent<CardBehaviour>();
        if (cardBehaviour != null)
        {
            //cardBehaviour.displayId = drawnCard.id;
            cardBehaviour.SetAction(action);
        }
        else { Debug.Log("Card behaviour null"); }
    }

    public void AddCardToPanel(Transform panelTransform, Action action)
    {
        Debug.Log("Add card " + action.name + " to panel " + panelTransform.name);
        GameObject cardObj = Instantiate(cardPrefab, panelTransform);
        CardBehaviour cardBehaviour = cardObj.GetComponent<CardBehaviour>();
        if (cardBehaviour != null)
        {
            //cardBehaviour.displayId = drawnCard.id;
            cardBehaviour.SetAction(action);
        }
        else { Debug.Log("Card behaviour null"); }
    }

    public void CardUsed(GameObject card)
    {
        //Debug.Log("Card used: " + card.name);
        //StartCoroutine(DisplayUsedCard(card));
        //card.transform.SetParent(displayPlayedCardPanel.transform, false);
        handPanel.SetCardPositions();
        card.transform.SetParent(discardPanel.transform, false);
    }

    public void CardUsed(Action action)
    {
        Debug.Log("Card used: " + action.name);
        GameObject cardObj = Instantiate(cardPrefab, discardPanel.transform);
        cardObj.name = "New card";
        CardBehaviour cardBehaviour = cardObj.GetComponent<CardBehaviour>();
        if (cardBehaviour != null)
        {
            //cardBehaviour.displayId = drawnCard.id;
            cardBehaviour.SetAction(action);
        }
        //StartCoroutine(DisplayUsedCard(card));
        //cardObj.transform.SetParent(displayPlayedCardPanel.transform, false);
        handPanel.SetCardPositions();
        cardObj.transform.SetParent(discardPanel.transform, false);
    }

    //private IEnumerator DisplayUsedCard(GameObject card)
    //{ 
    //    card.transform.SetParent(displayPlayedCardPanel.transform, false);
    //    handPanel.SetCardPositions();
    //    card.transform.SetParent(discardPanel.transform, false);
    //}

    public void ActionUsed(Action action)
    {
        discardPile.Push(action);
    }

    public Action RetrieveLastCard()
    {
        return discardPile.Pop();
    }

    // public void CardRetrieved(GameObject card)
    // {
    //     usedCards.Remove(card);
    //     CardBehaviour iconBehaviour = card.GetComponent<CardBehaviour>();
    //     Debug.Log("card is: " + iconBehaviour.action.type.ToString());
    //     switch (iconBehaviour.action.type)
    //     {
    //         case (CardType.Defence):
    //             card.transform.SetParent(defenceMenu.transform, false);
    //             Debug.Log("card added to defence");
    //             break;
    //         case (CardType.Attack):
    //             Debug.Log("card added to attack");
    //             card.transform.SetParent(attackMenu.transform, false);
    //             break;
    //         default:
    //             card.transform.SetParent(specialMenu.transform, false);
    //             break;
    //     }
    //     card.SetActive(true);
    //     RemoveRetrievableActionsMenu();
    // }

    public void DisplayRetrievableActionsMenu()
    {
        foreach (GameObject card in usedCards)
        {
            card.transform.SetParent(instinctPanel.transform, false);
            card.SetActive(true);
        }
        instinctPanel.SetActive(true);
    }


    public List<Action> GetActionsOfType(CardType type, PlayerType player)
    {
        List<Action> actions = new List<Action>();
        //Debug.Log("Deck size = " + deck.Length);
        foreach (Action card in deck)
        {
            //Debug.Log("Checking card: " + card.name + " of type " + card.type + " for player " + card.playerType);
            if (card.playerType == player && card.type == type && !actions.Contains(card)) actions.Add(card);
        }
        return actions;
    }

    private void RemoveRetrievableActionsMenu()
    {
        //foreach (GameObject card in usedCards)
        //{
        //    card.transform.SetParent(transform, false);
        //}
        instinctPanel.SetActive(false);
    }
}
