using TMPro;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChooseCardBehaviour : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerClickHandler
{
    private ChooseCardPanel chooseCardPanel;
    public Action action;
    private bool isPressed = false;

    //public int displayId;

    public PlayerType playerType;
    public int id;
    public string cardName;
    public string description;
    public CardType cardType;

    [SerializeField] private float highlightScale = 2;

    public Color[] cardColours = new Color[3];
    //public Sprite sprite;

    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private TMP_Text descriptionTxt;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chooseCardPanel = GetComponentInParent<ChooseCardPanel>();
    }

    public void SetAction(Action _action)
    {
        action = _action;
        playerType = _action.playerType;
        id = _action.id;
        cardName = _action.name;
        description = _action.description;
        cardType = action.type;
        nameTxt.text = cardName;
        descriptionTxt.text = description;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = new Vector3(highlightScale, highlightScale, 1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Pointer exit " + action.name);
        if (!isPressed) transform.localScale = Vector3.one;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (chooseCardPanel.CanCardBeSelected())
        {
            transform.localScale = new Vector3(highlightScale, highlightScale, 1);
            isPressed = true;
            chooseCardPanel.CardSelected(action.name);
        }
    }
}