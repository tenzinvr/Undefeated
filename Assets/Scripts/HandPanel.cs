using System;
using System.Collections.Generic;
using UnityEngine;

public class HandPanel : MonoBehaviour
{
    //private RectTransform[] cardRectTransforms;
    private int currentIndex;
    [SerializeField] private int overlap = 1;
    [SerializeField] private int cardWidth = 75;
    [SerializeField] private float handPositionAdjuster = 300;
    [SerializeField] private float fanAngle = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RectTransform[] cardRectTransforms = GetComponentsInChildren<RectTransform>();
    }

    // public void IncreaseCards()
    // {
    //     //Debug.Log("increase points in time");
    //     Array.Resize<RectTransform>(ref cardRectTransforms, cardRectTransforms.Length + 5);
    // }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddCard(GameObject card)
    {
        RectTransform rectTransform = card.GetComponent<RectTransform>();
        card.transform.SetParent(transform, false);
        SetCardPositions();
    }

    private RectTransform[] GetCardRectTransforms()
    {
        Transform[] allRectTransforms = GetComponentsInChildren<Transform>();
        RectTransform[] cardRectTransforms = new RectTransform[transform.childCount];
        int j = 0;
        for (int i = 0; i < allRectTransforms.Length; i++)
        {
            if (allRectTransforms[i].gameObject.CompareTag("Card"))
            {
                cardRectTransforms[j++] = allRectTransforms[i].GetComponent<RectTransform>();
            }
        }
        //Debug.Log("Cards in hand = " + cardRectTransforms.Length);
        return cardRectTransforms;
    }

    public void SetCardPositions()
    {
        RectTransform[] cardRectTransforms = GetCardRectTransforms();

        int numberOfCards = cardRectTransforms.Length;
        float shift = -1 * numberOfCards / 2f;
        float log = MathF.Log(numberOfCards, 10);
        int adjustedOverlap = (int)(( 1 / log) * cardWidth) - overlap;
        float x, y = 0;
        float angleStep = fanAngle / (numberOfCards - 1);
        float startAngle = fanAngle / 2;
        for (int i = 0; i < numberOfCards; i++)
        {
            if (cardRectTransforms[i] != null)
            {
                float angle = startAngle - i * angleStep;
                float rad = Mathf.Deg2Rad * angle;
                
                x = (i + shift) * adjustedOverlap;
                y = MathF.Sqrt((float)(Math.Pow(handPositionAdjuster, 2) - Math.Pow(x, 2))) - handPositionAdjuster;
                cardRectTransforms[i].localPosition = new Vector3(x, y, 0);
                
                cardRectTransforms[i].localRotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }
}
