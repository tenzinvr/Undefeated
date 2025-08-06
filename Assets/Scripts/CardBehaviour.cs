using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Action action;
    //private CanvasGroup canvasGroup;
    public GameObject timelineActionIcon;
    private PlayManager playManager;
    private bool actionPlayed;

    //public int displayId;

    public PlayerType playerType;
    public int id;
    public string cardName;
    public int knockBack;
    public string description;
    public CardType cardType;

    [SerializeField] private float highlightScale = 1.5f;
    [SerializeField] private float highlightOffset = 50;

    public Color[] cardColours = new Color[3];
    [SerializeField] private Image cardBackground;
    //public Sprite sprite;

    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private TMP_Text descriptionTxt;
    [SerializeField] private Image art;

    private TimelineManager timelineManager;
    private DeckManager deckManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //canvasGroup = GetComponent<CanvasGroup>(); 
        timelineManager = GameObject.FindGameObjectWithTag("Timeline").GetComponent<TimelineManager>();
        playManager = GameObject.FindGameObjectWithTag("Timeline").GetComponent<PlayManager>();
        GameObject playerObj = (playerType == PlayerType.Player) ? GameObject.FindGameObjectWithTag("Player1")
            : GameObject.FindGameObjectWithTag("Player2");
        deckManager = playerObj.GetComponentInChildren<DeckManager>();
    }

    public void SetAction(Action _action)
    {
        action = _action;
        playerType = _action.playerType;
        id = _action.id;
        cardName = _action.name;
        description = _action.description;
        cardType = action.type;
        //damage = card.damage;
        //knockBack = card.knockBack;
        //sprite = _action.sprite;
        switch (cardType)
        {
            case (CardType.Attack):
                cardBackground.color = cardColours[0];
                break;
            case (CardType.Defence):
                cardBackground.color = cardColours[1];
                break;
            default:
                cardBackground.color = cardColours[2];
                break;
        }

        nameTxt.text = cardName;
        //nameBtnTxt.text = cardName;
        descriptionTxt.text = description;
        //art.sprite = sprite;

        if (cardType != CardType.Special) timelineActionIcon.GetComponent<TimelineActionIcon>().action = action;
        action.icon = timelineActionIcon.gameObject;
        //Debug.Log(timelineActionIcon.GetComponent<TimelineActionIcon>().action.name);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (playManager.GetCurrentPlayerType() == playerType)
        {
            transform.localScale = new Vector3(highlightScale, highlightScale, 1);
            transform.position = new Vector3(transform.position.x, transform.position.y + highlightOffset, -1);
            if (!actionPlayed && cardType != CardType.Special)
            {
                timelineManager.PreviewActionIcon(action);
                Player player = playManager.GetCurrentPlayer();
                player.StartAnimation(action.name);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Pointer exit " + action.name);
        if (gameObject.activeInHierarchy && playManager.GetCurrentPlayerType() == playerType)
        {
            transform.localScale = Vector3.one;
            transform.position = new Vector3(transform.position.x, transform.position.y - highlightOffset, 0);
            if (cardType != CardType.Special) timelineManager.TurnOffPreviewActionIcon();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (playManager.playersTurn == action.playerType)
        {
            Transform parentTransform = transform.parent;
            if (parentTransform.parent.CompareTag("InstinctPanel"))
            {
                GameObject instinctPanel = parentTransform.parent.gameObject;
                instinctPanel.SetActive(false);
            }
            if (cardType == CardType.Special)
            {
                if (action is SpecialAction specialAction)
                {
                    SpecialBehaviour specialBehaviour = GetComponent<SpecialBehaviour>();
                    specialBehaviour.Instantiate(playerType, action);
                    specialBehaviour.Effect();
                }
                deckManager.CardUsed(gameObject);
                deckManager.ActionUsed(action);
                gameObject.SetActive(false);
                return;
            }

            playManager.AddActionToTurn(action);
            deckManager.CardUsed(gameObject);
            deckManager.ActionUsed(action);
            gameObject.SetActive(false);
            actionPlayed = !actionPlayed;
        }
    }
}
