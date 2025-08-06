using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TimelineActionIcon : MonoBehaviour
{
    public Action action;
    private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform windUpTimeIcon;
    [SerializeField] private RectTransform returnTimeIcon;
    [SerializeField] private TMP_Text text;
    private PlayManager playManager;
    public PointInTime pointInTime;
    private RectTransform rectTransform;
    [SerializeField] private float width = 30;
    [SerializeField] private Image background;
    [SerializeField] private Color defenceColour;
    [SerializeField] private Color attackColour;
    [SerializeField] private Color specialColour;

    [SerializeField] private GameObject backwardsBtn;
    [SerializeField] private GameObject forwardsBtn;

    private TimelineManager timelineManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playManager = GameObject.FindGameObjectWithTag("Timeline").GetComponent<PlayManager>();
        timelineManager = GameObject.FindGameObjectWithTag("Timeline").GetComponent<TimelineManager>();
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetAction(Action _action)
    {
        action = _action;
        text.text = action.name;
        switch (action.type)
        {
            case (CardType.Attack):
                background.color = attackColour;
                break;
            case (CardType.Defence):
                background.color = defenceColour;
                break;
            default:
                background.color = specialColour;
                break;
        }

    }

    public void SetTransform(PointInTime point, Action action)
    {
        Transform playerTransform = (action.playerType == PlayerType.Player ? point.playerTarget : point.opponentTarget).transform;
        transform.SetParent(playerTransform, false);
        //Debug.Log("Time = " + action.timeDelay);
        rectTransform = GetComponent<RectTransform>();
        //rectTransform.localPosition = parent.localPosition;
        //Debug.Log("Local Pos = " + rectTransform.localPosition + "\n Point pos = " + parent.localPosition);
        int windUpWidth = SetWidth(windUpTimeIcon, action.windUpTime);
        //Debug.Log("Wind up width = " + windUpWidth);
        int returnWidth = SetWidth(returnTimeIcon, action.returnTime);
        //Debug.Log("Return width = " + returnWidth);
        int totalWidth = windUpWidth + returnWidth;
        rectTransform.sizeDelta = new Vector2(totalWidth, 20);
        //Debug.Log("Action pos = " + totalWidth / 2);

        rectTransform.localPosition = new Vector3(totalWidth / 2, 0, 0);
        //Debug.Log("WInd up positino = " + windUpWidth / 2);
        windUpTimeIcon.localPosition = new Vector3((windUpWidth - totalWidth) / 2, 0, 0);
        //Debug.Log("Local pos = " + windUpTimeIcon.localPosition);
        returnTimeIcon.localPosition = new Vector3((totalWidth - returnWidth) / 2, 0, 0);
        //Debug.Log("New width = " + newWidth);
        //Debug.Log(rectTransform.position);
    }

    private int SetWidth(RectTransform rectTransform, int time)
    {
        rectTransform.localScale = Vector3.one;
        int newWidth = (int)(width * time / 50);
        rectTransform.sizeDelta = new Vector2(newWidth, 20);
        return newWidth;
    }

    //public void AdjustWidth(int time)
    //{
    //    RectTransform windUpRectTransform = windUpTimeIcon.GetComponent<RectTransform>();
    //    RectTransform returnRectTransform = windUpTimeIcon.GetComponent<RectTransform>();
    //    if (action.initialTime < time) { }
    //}

    public void Forward()
    {
        PointInTime nextPoint = playManager.GetNextPointInTime(pointInTime);
        if (nextPoint == null)
        {
            Debug.Log("next point null");
            return;
        }
        playManager.ChangeActionPointInTime(action, pointInTime, nextPoint);
        SetTransform(nextPoint, action);
        pointInTime = nextPoint;
        backwardsBtn.SetActive(true);
    }

    public void Backwards()
    {
        Debug.Log("Current point at " + pointInTime.index);
        PointInTime prevPoint = playManager.GetPreviousPointInTime(pointInTime);
        Debug.Log("Prev point at " + prevPoint.index);
        if (action.playerType == PlayerType.Player)
        {
            if (prevPoint == null || (prevPoint.playerState != PlayerState.Null && prevPoint.playerState != PlayerState.Idle))
            {
                Debug.Log("Prev point null? " + (prevPoint == null) + "\nprev point state = " + prevPoint.playerState);
                return;
            }
        }
        else
        {
            if (prevPoint == null || (prevPoint.opponentState != PlayerState.Null && prevPoint.playerState != PlayerState.Idle))
            {
                Debug.Log("Prev point null? " + (prevPoint == null) + "\nprev point state = " + prevPoint.opponentState);
                return;
            }
        }
        playManager.ChangeActionPointInTime(action, pointInTime, prevPoint);
        SetTransform(prevPoint, action);
        pointInTime = prevPoint;
        if (playManager.GetPreviousPointInTime(pointInTime) == null) backwardsBtn.SetActive(false);
    }

    public void ActionCancelled(int time)
    {
        Debug.Log("action cancelled");
        gameObject.SetActive(false);
        //rectTransform.localPosition = transform.parent.
        //Debug.Log("Local Pos = " + rectTransform.localPosition + "\n Point pos = " + parent.localPosition);
        int windUpWidth = 0;
        if (time <= action.windUpTime)
        {
           windUpWidth = SetWidth(windUpTimeIcon, time - action.initialTime);
        }
        int returnWidth = SetWidth(returnTimeIcon, 0);
        //Debug.Log("Wind up width = " + windUpWidth);
        //Debug.Log("Return width = " + returnWidth);
        int totalWidth = windUpWidth;
        rectTransform.sizeDelta = new Vector2(totalWidth, 20);
        //Debug.Log("Action pos = " + totalWidth / 2);

        rectTransform.localPosition = new Vector3(totalWidth / 2, 0, 0);
        //Debug.Log("WInd up positino = " + windUpWidth / 2);
        windUpTimeIcon.localPosition = new Vector3((windUpWidth - totalWidth) / 2, 0, 0);
        //Debug.Log("Local pos = " + windUpTimeIcon.localPosition);
        returnTimeIcon.localPosition = new Vector3(0, 0, 0);
        //Debug.Log("New width = " + newWidth);
        //Debug.Log(rectTransform.position);

    }

    public void ActionFeinted()
    {
        //Debug.Log("Icon action feinted");
    }

    public void SetStatic()
    {
        //Debug.Log("set icon static");
        if (forwardsBtn != null) forwardsBtn.SetActive(false);
        if (backwardsBtn != null) backwardsBtn.SetActive(false);
    }
}
