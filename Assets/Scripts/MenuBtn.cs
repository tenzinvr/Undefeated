using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class MenuBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private ActionDatabase cardDatabase;
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject otherMenu;
    private MenuBehaviour menuBehaviour;
    [SerializeField] private CardType cardType;
    [SerializeField] private PlayerType playerType;
    [SerializeField] private GameObject iconPrefab;
    private List<Action> actions = new List<Action>();
    private bool isSelected;

    private void Start()
    {
        cardDatabase = GameObject.FindAnyObjectByType<ActionDatabase>();
        menuBehaviour = menu.GetComponent<MenuBehaviour>();
        GetActions();
    }

    private void GetActions()
    {
        actions = cardDatabase.GetActionsOfType(cardType, playerType);
        foreach (Action action in actions)
        {
            GameObject iconObj = Instantiate(iconPrefab, menu.transform);
            IconBehaviour iconBehaviour = iconObj.GetComponent<IconBehaviour>();
            iconBehaviour.displayId = action.id;
            iconBehaviour.SetAction(action);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isSelected = true;
        otherMenu.SetActive(false);
        menu.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        menu.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(WaitToTurnOffMenu());
    }

    private IEnumerator WaitToTurnOffMenu()
    {
        yield return new WaitForSeconds(0.5f);
        if (!menuBehaviour.selected) menu.SetActive(false);
    }
}
