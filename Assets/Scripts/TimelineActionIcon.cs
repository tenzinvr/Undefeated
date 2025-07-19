using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Drawing;

public class TimelineActionIcon : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerEnterHandler
{
    public Action action;
    private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform windUpTimeIcon;
    [SerializeField] private RectTransform returnTimeIcon;
    [SerializeField] private TMP_Text text;
    private Transform returnTransform;
    private Vector3 differenceToPointer;
    private PlayManager playManager;
    public PointInTime pointInTime;
    private RectTransform rectTransform;
    [SerializeField] private float width = 30;
    public bool isMovable = true;

    private TimelineManager timelineManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playManager = GameObject.FindGameObjectWithTag("Timeline").GetComponent<PlayManager>();
        timelineManager = GameObject.FindGameObjectWithTag("Timeline").GetComponent<TimelineManager>();
        canvasGroup = GetComponent<CanvasGroup>();
        returnTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(rectTransform.position);
    }

    public void SetAction(Action _action)
    {
        action = _action;
        text.text = action.name;
    }

    public void SetTransform(PointInTime point, Action action)
    {
        Transform playerTransform = (action.player == PlayerType.Player ? point.playerTarget : point.opponentTarget).transform;
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
        returnTransform = playerTransform;
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

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("Begin drag");
        if (isMovable)
        {
            canvasGroup.blocksRaycasts = false;
            Vector3 pointerPos = new Vector3(eventData.position.x, transform.position.y, 0);
            differenceToPointer = transform.position - pointerPos;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isMovable)
        {
            //Debug.Log("dragging");

            //Collider2D targetCollider = Physics2D.OverlapBox(transform.position, new Vector2(1, 1), pointInTimeLayer);
            //if (targetCollider != null) Debug.Log(targetCollider.name);

            //RaycastHit2D hit;
            //hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.up, 1, pointInTimeLayer);
            //if (hit) Debug.Log(hit.collider.name);

            //RaycastHit raycastHit;
            //if (Physics.Raycast(transform.position, transform.forward, out raycastHit, 1, pointInTimeLayer)) Debug.Log(raycastHit.collider.name);

            //Vector3 pointerPos = new Vector3(eventData.position.x, eventData.position.y, 0);
            transform.position = new Vector3(eventData.position.x + differenceToPointer.x, transform.position.y, transform.position.z);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Vector3 worldPos;
        //RectTransformUtility.ScreenPointToWorldPointInRectangle(
        //    GetComponent<RectTransform>(), // your dragged UI element
        //    eventData.position,
        //    eventData.pressEventCamera,    // null in Screen Space - Overlay
        //    out worldPos
        //);

        //// Perform OverlapCircle in world space
        //Collider2D target = Physics2D.OverlapCircle(worldPos, 0.2f, pointInTimeLayer);

        //if (target != null)
        //{
        //    Debug.Log("Snapped to: " + target.name);

        //    // Convert world position back to anchored position on the UI
        //    Vector2 anchoredPos;
        //    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //        transform.parent as RectTransform,
        //        Camera.main.WorldToScreenPoint(target.transform.position),
        //        null, // no camera needed for Overlay canvas
        //        out anchoredPos
        //    );

        //    // Snap the UI element to the target’s screen position
        //    GetComponent<RectTransform>().anchoredPosition = anchoredPos;
        //}
        //else
        //{
        //    Debug.Log("No target found");
        //}

        //RaycastHit2D hit;
        //hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), -1 * transform.forward, 1, pointInTimeLayer);
        //Debug.Log(hit.collider.name);
        //if (hit.collider.CompareTag("Target"))
        //{
        //    Debug.Log("set to target");
        //    transform.SetParent(eventData.pointerCurrentRaycast.gameObject.transform, false);
        //    PointInTime newPoint = eventData.pointerCurrentRaycast.gameObject.GetComponent<PointInTime>();
        //    if (newPoint != null)
        //    {
        //        playManager.ChangeActionPointInTime(action, newPoint, action.timeOfEffect);
        //        returnTransform = transform.parent.GetComponent<Transform>();
        //        pointInTime = newPoint;
        //    }
        //    else Debug.Log("Target not found");
        //}
        //if (eventData.pointerCurrentRaycast.gameObject.CompareTag("Target"))
        //{
        //    Debug.Log("set to target");
        //    transform.SetParent(eventData.pointerCurrentRaycast.gameObject.transform, false);
        //    PointInTime newPoint = eventData.pointerCurrentRaycast.gameObject.GetComponent<PointInTime>();
        //    if (newPoint != null)
        //    {
        //        playManager.ChangeActionPointInTime(action, newPoint, action.timeOfEffect);
        //        returnTransform = transform.parent.GetComponent<Transform>();
        //        pointInTime = newPoint;
        //    }
        //    else Debug.Log("Target not found");
        //}
        //else
        //{
        //    transform.SetParent(returnTransform, false);
        //}
        canvasGroup.blocksRaycasts = true;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("pointer enter");
    }
}
