using UnityEngine;
using UnityEngine.UI;
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

    //public void Centre()
    //{
    //    Debug.Log("centre");
    //    RectTransform[] cardTransforms = GetComponentsInChildren<RectTransform>();
    //    float totalWidth = 0;
    //    for (int i = 0; i < cardTransforms.Length; i++)
    //    {
    //        if (cardTransforms[i].gameObject.CompareTag("Card"))
    //        {
    //            totalWidth += cardTransforms[i].sizeDelta.x;
    //        }
    //    }
    //    Debug.Log(totalWidth);
    //    RectTransform rectTransform = GetComponent<RectTransform>();
    //    HorizontalLayoutGroup horizontalLayoutGroup = GetComponent<HorizontalLayoutGroup>();
    //    //rectTransform.localPosition = new Vector3(- (totalWidth / 2), transform.localPosition.y, transform.localPosition.z);
    //}

    public void OnPointerEnter(PointerEventData eventData)
    {
        selected = true;
        //Centre();
        StopAllCoroutines();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(WaitToTurnOffMenu());
    }

    private IEnumerator WaitToTurnOffMenu()
    {
        yield return new WaitForSeconds(0.3f);
        selected = false;
        gameObject.SetActive(false);
    }
}
