using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class TimelineManager : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private Vector3 currentPointInTimeTransform;
    [SerializeField] private GameObject timelinePrefab;
    [SerializeField] private GameObject timelinePiecesHolder;
    [SerializeField] private GameObject iconPrefab;
    [SerializeField] private GameObject stunIconPrefab;
    [SerializeField] private GameObject breatheIconPrefab;
    [SerializeField] private GameObject feintIconPrefab;
    [SerializeField] private GameObject indicator;
    [System.NonSerialized] public List<GameObject> allIcons = new List<GameObject>();
    private GameObject previewIcon;
    private GameObject breathePreviewIcon;
    public bool isPlaying;
    private float delta;
    private Vector3 pointInTimePosition;
    private float timeScale;
    private RectTransform rectTransform;
    [SerializeField] private float pointInTimeWidth = 30;
    private Player player1;
    private Player player2; 

    private PlayManager playManager;
    private DeckManager deckManagerPlayer;
    private DeckManager deckManagerOpponent;

    private void Start()
    {
        playManager = GetComponent<PlayManager>();
        rectTransform = GetComponent<RectTransform>();
        InstantiateTimeline();
        timeScale = pointInTimeWidth / (50.0f / 1000.0f);
        player1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<Player>();
        player2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<Player>();
        deckManagerPlayer = player1.GetComponentInChildren<DeckManager>();
        deckManagerOpponent = player2.GetComponentInChildren<DeckManager>();
        currentPointInTimeTransform = rectTransform.localPosition;
        previewIcon = GameObject.Instantiate(iconPrefab);
        previewIcon.SetActive(false); 
        breathePreviewIcon = GameObject.Instantiate(breatheIconPrefab);
        breathePreviewIcon.SetActive(false);
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (isPlaying)
        {
            delta = Time.deltaTime * timeScale;
            transform.position = Vector3.MoveTowards(transform.position, pointInTimePosition, delta);
            if (transform.position == pointInTimePosition) PauseTimeline();
        }
    }
    
    public void PlayTimeline()
    {
        //Debug.Log("Play timeline");
        isPlaying = true;
        float newPos = transform.position.x - pointInTimeWidth;
        pointInTimePosition = new Vector3(newPos, transform.position.y, transform.position.z);
        player1.PlayAnimation();
        player2.PlayAnimation();
    }

    public void PauseTimeline()
    {
        //Debug.Log("pause timeline");
        currentPointInTimeTransform = rectTransform.localPosition;
        isPlaying = false;
        player1.PauseAnimation();
        player2.PauseAnimation();
    }

    private IEnumerator MovingTimeline(float time)
    {
        yield return new WaitForSeconds(time);
        PauseTimeline();
    }

    private void InstantiateTimeline()
    { 
        GameObject newPiece = GameObject.Instantiate(timelinePrefab, timelinePiecesHolder.transform); 
        PointInTime[] newPointsInTime = newPiece.GetComponentsInChildren<PointInTime>();
        playManager.InstantiatePoints(newPointsInTime);
    }

    public void IncreaseTimeline()
    {
        //Debug.Log("Increase timeline");
        GameObject newPiece = GameObject.Instantiate(timelinePrefab, timelinePiecesHolder.transform);
        PointInTime[] newPointsInTime = newPiece.GetComponentsInChildren<PointInTime>();
        playManager.IncreasePointsInTime(newPointsInTime);
    }

    public GameObject AddActionIcon(PointInTime point, Action action)
    {
        //Debug.Log("Add action icon");
        GameObject newIcon = GameObject.Instantiate(iconPrefab);
        newIcon.SetActive(true);
        TimelineActionIcon timelineActionIcon = newIcon.GetComponent<TimelineActionIcon>();
        timelineActionIcon.SetAction(action);

        //Debug.Log(point.time);
        timelineActionIcon.pointInTime = point;
        timelineActionIcon.SetTransform(point, action);
        action.icon = newIcon;
        allIcons.Add(newIcon);
        return newIcon;
    }

    //public void CancelAction(Action action)
    //{
    //    //Image iconImage = action.icon.GetComponent<Image>();
    //    Debug.Log(action.icon.transform.parent.name);
    //    action.icon.SetActive(false);
    //    TimelineActionIcon icon = action.icon.GetComponent<TimelineActionIcon>();

    //    allIcons.Remove(action.icon);
    //    //iconImage.color = new Color(1, 1, 1, 0.5f);
    //}

    public void PreviewActionIcon(Action action)
    {
        //Debug.Log("Preview action " + action.name);
        TimelineActionIcon timelineActionIcon = previewIcon.GetComponent<TimelineActionIcon>();
        timelineActionIcon.SetAction(action);

        PointInTime point = playManager.GetActionPointInTime(action);
        timelineActionIcon.pointInTime = point;
        timelineActionIcon.SetTransform(point, action);
        previewIcon.SetActive(true);
    }

    public void PreviewBreatheIcon(Action action)
    {
        TimelineActionIcon timelineActionIcon = breathePreviewIcon.GetComponent<TimelineActionIcon>();
        timelineActionIcon.SetAction(action);

        PointInTime point = playManager.GetActionPointInTime(action);
        timelineActionIcon.pointInTime = point;
        timelineActionIcon.SetTransform(point, action);
        breathePreviewIcon.SetActive(true);
    }

    public void AddStunIcon(PlayerType player, PointInTime point, Action stunAction)
    {
        GameObject newStunIcon = GameObject.Instantiate(stunIconPrefab);
        TimelineActionIcon timelineActionIcon = newStunIcon.GetComponent<TimelineActionIcon>();
        timelineActionIcon.action = stunAction;
        timelineActionIcon.action.playerType = player;
        timelineActionIcon.pointInTime = point;
        stunAction.icon = newStunIcon;
        //Debug.Log("add stun to point = " + point.time);
        timelineActionIcon.SetTransform(point, stunAction);
    }

    public void AddBreatheIcon(PlayerType player, PointInTime point, Action breatheAction)
    {
        //Debug.Log("add breathe icon");
        GameObject newBreatheIcon = GameObject.Instantiate(breatheIconPrefab);
        TimelineActionIcon timelineActionIcon = newBreatheIcon.GetComponent<TimelineActionIcon>();
        timelineActionIcon.action = breatheAction;
        timelineActionIcon.action.playerType = player;
        timelineActionIcon.pointInTime = point;
        breatheAction.icon = newBreatheIcon;
        //Debug.Log("add stun to point = " + point.time);
        timelineActionIcon.SetTransform(point, breatheAction);
    }

    public void AddFeintIcon(PlayerType player, PointInTime point, Action feintAction)
    {
        //Debug.Log("Add feint icon");
        GameObject newFeintheIcon = GameObject.Instantiate(feintIconPrefab);
        TimelineActionIcon timelineActionIcon = newFeintheIcon.GetComponent<TimelineActionIcon>();
        timelineActionIcon.action = feintAction;
        timelineActionIcon.action.playerType = player;
        timelineActionIcon.pointInTime = point;
        feintAction.icon = newFeintheIcon;
        //Debug.Log("add stun to point = " + point.time);
        timelineActionIcon.SetTransform(point, feintAction);
    }

    public void RemoveIcon(PlayerType playerType, PointInTime point)
    {
        if (point.actionStart)
        {
            Debug.Log("remove icon at point " + point.index + " for " + playerType);
            GameObject icon = playerType == PlayerType.Player ? point.playerAction.icon : point.opponentAction.icon;
            //Debug.Log("Removing icon for player action " + point.playerAction.name);
            Debug.Log("Icon null? " + (icon == null));
            if (icon != null) icon.SetActive(false);
        }
    }

    public void MoveIcon(Action action, PointInTime newPoint)
    {
        GameObject icon = action.icon;
        TimelineActionIcon iconBehaviour = icon.GetComponent<TimelineActionIcon>();
        iconBehaviour.SetTransform(newPoint, action);
    }

    public void TurnOffPreviewActionIcon()
    {
        //Debug.Log("turn off preview");
        previewIcon.SetActive(false);
        breathePreviewIcon.SetActive(false);
    }

    public void SetIconsStatic()
    {
        //Debug.Log("set icons static");
        foreach (GameObject icon in allIcons)
        {
            TimelineActionIcon timelineActionIcon = icon.GetComponent<TimelineActionIcon>();
            timelineActionIcon.SetStatic();
        }
    }

    //public void OnDrag(PointerEventData eventData)
    //{
    //    Vector3 pointerPos = new Vector3(eventData.position.x, eventData.position.y, 0);
    //    transform.position = new Vector3(pointerPos.x + differenceToPointer.x, transform.position.y, 0);
    //}

    //public void OnBeginDrag(PointerEventData eventData)
    //{
    //    Vector3 pointerPos = new Vector3(eventData.position.x, transform.position.y, 0);
    //    differenceToPointer = transform.position - pointerPos;
    //}

    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    rectTransform.localPosition = currentPointInTimeTransform;
    //}
}
