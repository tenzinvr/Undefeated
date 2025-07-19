using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

public class TimelineManager : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private Vector3 currentPointInTimeTransform;
    [SerializeField] private GameObject timelinePrefab;
    [SerializeField] private GameObject timelinePiecesHolder;
    [SerializeField] private GameObject iconPrefab;
    [SerializeField] private GameObject stunIconPrefab;
    [SerializeField] private GameObject indicator;
    private List<GameObject> icons = new List<GameObject>();
    private GameObject previewIcon;
    private Vector3 differenceToPointer;
    public bool isPlaying;
    private bool isPreviewing;
    private float delta;
    private Vector3 moveTowards;
    private Vector3 pointInTimePosition;
    private float timeScale;
    private RectTransform rectTransform;
    [SerializeField] private float pointInTimeWidth = 100;
    private Player player1;
    private Player player2; 

    private PlayManager playManager;

    private void Start()
    {
        playManager = GetComponent<PlayManager>();
        rectTransform = GetComponent<RectTransform>();
        InstantiateTimeline();
        timeScale = pointInTimeWidth / (50.0f / 1000.0f);
        player1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<Player>();
        player2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<Player>();
        currentPointInTimeTransform = rectTransform.localPosition;
        previewIcon = GameObject.Instantiate(iconPrefab);
        previewIcon.SetActive(false);
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (isPlaying)
        {
            //Debug.Log(rectTransform.localPosition);
            delta = Time.deltaTime * timeScale;
            transform.position = Vector3.MoveTowards(transform.position, pointInTimePosition, delta);
            if (transform.position == pointInTimePosition) PauseTimeline();
            //Debug.Log("Move " + deltaPosition);
            //rectTransform.localPosition = new Vector3(transform.localPosition.x - deltaPosition, rectTransform.localPosition.y, rectTransform.localPosition.z);
        }
        else if (isPreviewing)
        {
            indicator.transform.position = Vector3.MoveTowards(indicator.transform.position, pointInTimePosition, Time.deltaTime * timeScale);
            if (indicator.transform.position == pointInTimePosition) { isPreviewing = false; }
        }
    }

    public void PreviewAction(Vector3 _pointInTimePosition)
    {
        //Debug.Log(_pointInTimePosition);
        //pointInTimePosition = _pointInTimePosition;
        isPreviewing = true;
    }

    //public void PlayTimeline(float time)
    //{
    //    Debug.Log("Stop playing at " + Time.time);
    //    isPlaying = true;
    //    player1.animator.speed = 1;
    //    player2.animator.speed = 1;
    //    StartCoroutine(MovingTimeline(time));
    //}
    
    public void PlayTimeline(Vector3 _pointPosition)
    {
        isPlaying = true;
        pointInTimePosition = new Vector3(transform.position.x - _pointPosition.x, transform.position.y, transform.position.z);
        //Debug.Log("Point in time pos " + pointInTimePosition);
        //player1.animator.speed = 1;
        //player2.animator.speed = 1;

        //StartCoroutine(MovingTimeline(time));
    }

    public void PauseTimeline()
    {
        //Debug.Log("pause timeline");
        currentPointInTimeTransform = rectTransform.localPosition;
        //player1.animator.speed = 0;
        //player2.animator.speed = 0;
        isPlaying = false;
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
        Debug.Log("Increase timeline");
        GameObject newPiece = GameObject.Instantiate(timelinePrefab, timelinePiecesHolder.transform);
        PointInTime[] newPointsInTime = newPiece.GetComponentsInChildren<PointInTime>();
        playManager.IncreasePointsInTime(newPointsInTime);
    }

    public void AddActionIcon(PointInTime point, Action action)
    {
        GameObject newIcon = GameObject.Instantiate(iconPrefab);
        TimelineActionIcon timelineActionIcon = newIcon.GetComponent<TimelineActionIcon>();
        timelineActionIcon.SetAction(action);
        timelineActionIcon.pointInTime = point;
        timelineActionIcon.SetTransform(point, action);
    }

    public void CancelAction(Action action)
    {
        Image iconImage = action.icon.GetComponent<Image>();
        action.icon.SetActive(false);
        iconImage.color = new Color(1, 1, 1, 0.5f);
    }

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

    public void AddStunIcon(PlayerType player, PointInTime point, Action stunAction)
    {
        GameObject newStunIcon = GameObject.Instantiate(stunIconPrefab);
        TimelineActionIcon timelineActionIcon = newStunIcon.GetComponent<TimelineActionIcon>();
        timelineActionIcon.action = stunAction;
        timelineActionIcon.action.player = player;
        timelineActionIcon.pointInTime = point;
        Debug.Log("add stun to point = " + point.time);
        timelineActionIcon.SetTransform(point, stunAction);
    }

    public void RemoveIcons()
    {

    }

    public void TurnOffPreviewActionIcon()
    {
        previewIcon.SetActive(false);
    }

    public void SetIconsStatic()
    {
        foreach (GameObject icon in icons)
        {
            TimelineActionIcon timelineActionIcon = icon.GetComponent<TimelineActionIcon>();
            timelineActionIcon.isMovable = false;
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
