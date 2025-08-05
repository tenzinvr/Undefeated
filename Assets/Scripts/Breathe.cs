using UnityEngine;
using UnityEngine.EventSystems;

public class Breathe : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private PlayManager playManager;
    private TimelineManager timelineManager;
    [SerializeField] private Action action;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playManager = GameObject.FindGameObjectWithTag("Timeline").GetComponent<PlayManager>();
        timelineManager = GameObject.FindGameObjectWithTag("Timeline").GetComponent<TimelineManager>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = new Vector3(1.3f, 1.3f, 1);
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, -1); 
        timelineManager.PreviewBreatheIcon(action);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        timelineManager.TurnOffPreviewActionIcon();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        playManager.AddActionToTurn(action);
    }
}
