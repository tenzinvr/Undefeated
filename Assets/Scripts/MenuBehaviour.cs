using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class MenuBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool selected;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        selected = true;
        StopAllCoroutines();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(WaitToTurnOffMenu());
    }

    private IEnumerator WaitToTurnOffMenu()
    {
        yield return new WaitForSeconds(0.5f);
        selected = false;
        gameObject.SetActive(false);
    }
}
