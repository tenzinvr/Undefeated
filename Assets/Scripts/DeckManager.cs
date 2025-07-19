using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class DeckManager : MonoBehaviour
{
    private ActionDatabase cardDatabase;
    public Action[] deck;
    public Action[] drawPile;
    public List<Action> discardPile = new List<Action>();
    public List<Action> cardsInPlay = new List<Action>();
    public List<Action> cardsInHand = new List<Action>();

    [SerializeField] private PlayerType player = PlayerType.Player;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject deckPanel;
    [SerializeField] private GameObject handPanel;
    private int currentCardIndex;

    //public List<Card> container = new List<Card>();
    [SerializeField] private int deckSize;
    [SerializeField] private int handSize;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cardDatabase = GameObject.FindAnyObjectByType<ActionDatabase>();
        deck = new Action[deckSize];

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
        deck[i++] = cardDatabase.GetCard("Rear Uppercut", player);
        deck[i++] = cardDatabase.GetCard("Rear Uppercut", player);
        deck[i++] = cardDatabase.GetCard("Bob", player);
        deck[i++] = cardDatabase.GetCard("Bob", player);
        deck[i++] = cardDatabase.GetCard("Slip", player);
        deck[i++] = cardDatabase.GetCard("Slip", player);
        deck[i++] = cardDatabase.GetCard("Slip", player);

        drawPile = deck;
        currentCardIndex = 0;
        deckSize = drawPile.Length;
        //Debug.Log(deckSize);
        Shuffle();

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

    void DrawCards(int numberOfCards)
    {
        for (int i = 0; i < numberOfCards; i++)
        {
            Action drawnCard = drawPile[currentCardIndex++];
            //Debug.Log(drawnCard == null);
            cardsInHand.Add(drawnCard);
            //Debug.Log(cardsInHand.Count);
            GameObject cardObj = Instantiate(cardPrefab, handPanel.transform);
            IconBehaviour iconBehaviour = cardObj.GetComponent<IconBehaviour>();
            iconBehaviour.displayId = drawnCard.id;
            iconBehaviour.SetAction(drawnCard);
        }
    }
}
