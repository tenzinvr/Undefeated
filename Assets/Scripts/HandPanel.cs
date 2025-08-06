using System;
using System.Collections.Generic;
using UnityEngine;

public class HandPanel : MonoBehaviour
{
    [SerializeField] private int overlap = 1;
    [SerializeField] private int cardWidth = 75;
    [SerializeField] private float handPositionAdjuster = 300;
    [SerializeField] private float fanAngle = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RectTransform[] cardRectTransforms = GetComponentsInChildren<RectTransform>();
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

        if (numberOfCards == 0)
            return;

        float shift = -1 * numberOfCards / 2f;

        // Avoid log(0) or log(1)
        float log = numberOfCards > 1 ? MathF.Log(numberOfCards, 10) : 1f;

        int adjustedOverlap = (int)((1 / log) * cardWidth) - overlap;
        float angleStep = numberOfCards > 1 ? fanAngle / (numberOfCards - 1) : 0f;
        float startAngle = fanAngle / 2;

        for (int i = 0; i < numberOfCards; i++)
        {
            if (cardRectTransforms[i] != null)
            {
                float angle = startAngle - i * angleStep;
                float rad = Mathf.Deg2Rad * angle;

                float x = (i + shift) * adjustedOverlap;

                // Clamp x to valid range to prevent sqrt of negative number
                float maxX = handPositionAdjuster - 1f; // avoid x >= handPositionAdjuster
                x = Mathf.Clamp(x, -maxX, maxX);

                float ySqr = Mathf.Pow(handPositionAdjuster, 2) - Mathf.Pow(x, 2);
                float y = Mathf.Sqrt(Mathf.Max(0, ySqr)) - handPositionAdjuster;

                cardRectTransforms[i].localPosition = new Vector3(x, y, 0);
                cardRectTransforms[i].localRotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }
}
