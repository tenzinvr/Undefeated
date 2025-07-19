using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IconBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Action action;
    //private Transform returnTransform;
    [SerializeField] private GameObject displayCard;
    private CanvasGroup canvasGroup;
    public GameObject timelineActionIcon;
    private PlayManager playManager;

    public int displayId;

    public PlayerType player;
    public int id;
    public string cardName;
    public int time;
    public int damage;
    public int knockBack;
    public string description;
    //public Sprite sprite;

    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private TMP_Text nameBtnTxt;
    [SerializeField] private TMP_Text timeTxt;
    [SerializeField] private TMP_Text damageTxt;
    [SerializeField] private Image art;

    private TimelineManager timelineManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>(); 
        timelineManager = GameObject.FindGameObjectWithTag("Timeline").GetComponent<TimelineManager>();
        playManager = GameObject.FindGameObjectWithTag("Timeline").GetComponent<PlayManager>();
    }

    public void SetAction(Action _action)
    {
        action = _action;
        player = _action.player;
        id = _action.id;
        cardName = _action.name;
        time = _action.windUpTime;
        //damage = card.damage;
        //knockBack = card.knockBack;
        //sprite = _action.sprite;

        nameTxt.text = cardName;
        //nameBtnTxt.text = cardName;
        timeTxt.text = time.ToString();
        damageTxt.text = damage.ToString();
        //art.sprite = sprite;

        timelineActionIcon.GetComponent<TimelineActionIcon>().action = action;
        action.icon = timelineActionIcon.gameObject;
        //Debug.Log(timelineActionIcon.GetComponent<TimelineActionIcon>().action.name);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = new Vector3(1.2f, 1.2f, 1);
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        timelineManager.PreviewActionIcon(action);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        timelineManager.TurnOffPreviewActionIcon();
    }

    public void OnIconClick()
    {
        //Debug.Log(action.name);
        if (playManager.playersTurn == action.player)
        {
            playManager.AddActionToTurn(action);
            gameObject.SetActive(false);
        }
    }
}
