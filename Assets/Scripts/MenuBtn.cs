using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class MenuBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private CardDatabase cardDatabase;
    private DeckManager deckManager;
    [SerializeField] private GameObject menu;
    private MenuBehaviour menuBehaviour;
    [SerializeField] private CardType cardType;
    [SerializeField] private PlayerType playerType;
    [SerializeField] private GameObject iconPrefab;
    private List<Action> actions = new List<Action>();
    private List<SpecialAction> specialActions = new List<SpecialAction>();

    private void Start()
    {
        cardDatabase = GameObject.FindAnyObjectByType<CardDatabase>(); 
        GameObject playerObj = (playerType == PlayerType.Player) ? GameObject.FindGameObjectWithTag("Player1")
            : GameObject.FindGameObjectWithTag("Player2");
        deckManager = playerObj.GetComponentInChildren<DeckManager>();
        menuBehaviour = menu.GetComponent<MenuBehaviour>();
        GetCards();
    }

    private void GetCards()
    {
        actions = cardDatabase.GetActionsOfType(cardType, playerType);
        //Debug.Log("Cards in type = " + actions.Count);
        foreach (Action action in actions)
        {
            GameObject iconObj = Instantiate(iconPrefab, menu.transform);
            CardBehaviour iconBehaviour = iconObj.GetComponent<CardBehaviour>();
            //iconBehaviour.displayId = action.id;
            //Debug.Log("Set action " + action.name);
            iconBehaviour.SetAction(action);
        }
        //return;
        //if (cardType != CardType.Special)
        //{
        //    actions = cardDatabase.GetActionsOfType(cardType, playerType);
        //    foreach (Action action in actions)
        //    {
        //        GameObject iconObj = Instantiate(iconPrefab, menu.transform);
        //        CardBehaviour iconBehaviour = iconObj.GetComponent<CardBehaviour>();
        //        iconBehaviour.displayId = action.id;
        //        iconBehaviour.SetAction(action);
        //    }
        //    return;
        //}
        //specialActions = cardDatabase.GetSpecials(playerType); 
        //foreach (SpecialAction action in specialActions)
        //{
        //    GameObject iconObj = Instantiate(iconPrefab, menu.transform);
        //    SpecialCardBehaviour cardBehaviour = iconObj.GetComponent<SpecialCardBehaviour>();
        //    cardBehaviour.displayId = action.id;
        //    cardBehaviour.SetAction(action);
        //}
    }

    public void AddCards(GameObject card)
    {
        card.transform.SetParent(transform, false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        menu.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        menu.SetActive(true);
        StopAllCoroutines();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(WaitToTurnOffMenu());
    }

    private IEnumerator WaitToTurnOffMenu()
    {
        yield return new WaitForSeconds(0.2f);
        if (!menuBehaviour.selected) menu.SetActive(false);
    }
}
