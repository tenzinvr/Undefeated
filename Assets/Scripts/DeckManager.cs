using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class DeckManager : MonoBehaviour
{
    private CardDatabase cardDatabase;
    public Action[] deck;
    public Action[] drawPile;
    public Stack<Action> discardPile = new Stack<Action>();
    public SpecialAction[] specialCards;
    public List<Action> cardsInPlay = new List<Action>();
    public List<Action> cardsInHand = new List<Action>();
    private List<GameObject> usedCards;
    [System.NonSerialized] public List<GameObject> icons = new List<GameObject>();

    [SerializeField] private GameObject deckPanel;
    [SerializeField] private HandPanel handPanel;
    [SerializeField] private GameObject discardPanel;
    private int currentCardIndex;

    public GameObject retrieveActionPanel;

    [SerializeField] private PlayerType player = PlayerType.Player;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject specialCardPrefab;

    public Action feintedAction;

    //public List<Card> container = new List<Card>();
    [SerializeField] private int deckSize;
    [SerializeField] private int handSize;

    private TimelineManager timelineManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cardDatabase = GameObject.FindAnyObjectByType<CardDatabase>();
        timelineManager = GameObject.FindGameObjectWithTag("Timeline").GetComponent<TimelineManager>();
        deck = new Action[deckSize];
        usedCards = new List<GameObject>();

        int i = 0;
        deck[i++] = cardDatabase.GetCard("Jab", player);
        deck[i++] = cardDatabase.GetCard("Jab", player);
        deck[i++] = cardDatabase.GetCard("Jab", player);
        deck[i++] = cardDatabase.GetCard("Jab", player);
        deck[i++] = cardDatabase.GetCard("Jab", player);
        deck[i++] = cardDatabase.GetCard("Cross", player);
        deck[i++] = cardDatabase.GetCard("Cross", player);
        deck[i++] = cardDatabase.GetCard("Cross", player);
        deck[i++] = cardDatabase.GetCard("Cross", player);
        deck[i++] = cardDatabase.GetCard("Cross", player);
        deck[i++] = cardDatabase.GetCard("Lead Hook", player);
        deck[i++] = cardDatabase.GetCard("Lead Hook", player);
        deck[i++] = cardDatabase.GetCard("Lead Hook", player);
        deck[i++] = cardDatabase.GetCard("Rear Hook", player);
        deck[i++] = cardDatabase.GetCard("Rear Hook", player);
        deck[i++] = cardDatabase.GetCard("Lead Uppercut", player);
        deck[i++] = cardDatabase.GetCard("Lead Uppercut", player);
        deck[i++] = cardDatabase.GetCard("Rear Uppercut", player);
        deck[i++] = cardDatabase.GetCard("Rear Uppercut", player);
        deck[i++] = cardDatabase.GetCard("Bob", player);
        deck[i++] = cardDatabase.GetCard("Bob", player);
        deck[i++] = cardDatabase.GetCard("Bob", player);
        deck[i++] = cardDatabase.GetCard("Bob", player);
        deck[i++] = cardDatabase.GetCard("Slip", player);
        deck[i++] = cardDatabase.GetCard("Slip", player);
        deck[i++] = cardDatabase.GetCard("Slip", player);
        deck[i++] = cardDatabase.GetCard("Slip", player);
        deck[i++] = cardDatabase.GetCard("Slip", player);
        deck[i++] = cardDatabase.GetCard("Slip", player);

        deck[i++] = cardDatabase.GetCard("Wild Swing", player);
        deck[i++] = cardDatabase.GetCard("Wild Swing", player);
        deck[i++] = cardDatabase.GetCard("Wild Swing", player);
        deck[i++] = cardDatabase.GetCard("Creatine", player);
        deck[i++] = cardDatabase.GetCard("Creatine", player);
        deck[i++] = cardDatabase.GetCard("Creatine", player);
        deck[i++] = cardDatabase.GetCard("Protein Shake", player);
        deck[i++] = cardDatabase.GetCard("Protein Shake", player);
        deck[i++] = cardDatabase.GetCard("Protein Shake", player);
        // deck[i++] = cardDatabase.GetCard("Instinct", player);
        // deck[i++] = cardDatabase.GetCard("Instinct", player);
        // deck[i++] = cardDatabase.GetCard("Instinct", player);
        deck[i++] = cardDatabase.GetCard("Hard Head", player);
        deck[i++] = cardDatabase.GetCard("Hard Head", player);
        deck[i++] = cardDatabase.GetCard("Feint", player);
        deck[i++] = cardDatabase.GetCard("Feint", player);
        deck[i++] = cardDatabase.GetCard("Feint", player);
        deck[i++] = cardDatabase.GetCard("Feint", player);
        deck[i++] = cardDatabase.GetCard("Feint", player);
        deck[i++] = cardDatabase.GetCard("Feint", player);


        drawPile = deck;
        currentCardIndex = 0;
        deckSize = drawPile.Length;
        //Debug.Log(deckSize);
        Shuffle();
        // foreach (Action action in deck)
        // {
        //     if (action != null) Debug.Log(action.name);
        //     else { Debug.Log("action null"); }
        // }
        DrawCards(handSize);
    }

    public void Shuffle()
    {
        int randomIndex = Random.Range(0, deckSize);
        Action currentCard = drawPile[0];
        for (int i = 0; i < deckSize; i++)
        {
            randomIndex = Random.Range(i, deckSize);
            currentCard = drawPile[i];
            drawPile[i] = drawPile[randomIndex];
            drawPile[randomIndex] = currentCard;
            //Debug.Log("Card at " + i + " swapped with card at " + randomIndex);
        }
    }

    public void DrawCards(int numberOfCards)
    {
        for (int i = 0; i <= numberOfCards; i++)
        {
            Action drawnCard = drawPile[currentCardIndex++];
            //Debug.Log(drawnCard == null);
            AddCard(drawnCard);
            cardsInHand.Add(drawnCard);
            //Debug.Log(cardsInHand.Count);

        }
    }

    public void AddCard(Action card)
    {
        GameObject cardObj;
        if (card.type != CardType.Special) cardObj = Instantiate(cardPrefab, handPanel.transform);
        else cardObj = Instantiate(specialCardPrefab, handPanel.transform);
        handPanel.AddCard(cardObj);
        CardBehaviour cardBehaviour = cardObj.GetComponent<CardBehaviour>();
        if (cardBehaviour != null)
        {
            //cardBehaviour.displayId = drawnCard.id;
            cardBehaviour.SetAction(card);
        }
        else { Debug.Log("Card behaviour null"); }
    }

    public void CardUsed(GameObject card)
    {
        card.transform.SetParent(discardPanel.transform, false);
        handPanel.SetCardPositions();
    }

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
            card.transform.SetParent(retrieveActionPanel.transform, false);
            card.SetActive(true);
        }
        retrieveActionPanel.SetActive(true);
    }


    public List<Action> GetActionsOfType(CardType type, PlayerType player)
    {
        List<Action> actions = new List<Action>();
        foreach (Action card in deck)
        {
            if (card.playerType == player && card.type == type) actions.Add(card);
        }
        return actions;
    }

    private void RemoveRetrievableActionsMenu()
    {
        foreach (GameObject card in usedCards)
        {
            card.transform.SetParent(transform, false);
        }
        retrieveActionPanel.SetActive(false);
    }
}
