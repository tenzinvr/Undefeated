using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPreview : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private GameObject model;
    [SerializeField] private GameObject player;
    private string[] animationNames = new string[10];
    private int currentIndex;
    private int playingIndex;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PreviewTurn()
    {
        //animationNames.Clear();
        //for (int i = 0; i < names.Length; i++)
        //{
        //    animationNames.Enqueue(names[i]);
        //}
        playingIndex = 0;
        animator.speed = 1;
        transform.position = player.transform.position;
        model.SetActive(true);
        NextAnimation();
    }



    public void AddToAnimations(string newAnimation)
    {
        animationNames[currentIndex++] = newAnimation;
    }

    public void ClearAnimations()
    {
        currentIndex = 0;
        for (int i = 0; i < animationNames.Length; i++)
        {
            animationNames[i] = null;
        }
    }

    public void NextAnimation()
    {
        if (animationNames[playingIndex] != null) animator.SetTrigger(animationNames[playingIndex++]);
        else
        {
            model.SetActive(false);
        }
    }

    public void PausePreview()
    {
        animator.speed = 0;
    }
}
