using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [System.NonSerialized] public Animator animator;
    public HealthManager healthManager;
    [SerializeField] private TMP_Text damageTxt;

    public PlayerType playerType;
    public int health = 100;
    public int timeModifier = 0;
    public int damageModifier;
    public bool isBlocking;
    public Distance distance;
    public Stance stance;
    public int lastPointEvaluated = 0;
    public int lastPointToEvaluate = 0;
    public int position;
    public Queue<Action> currentCombo;
    public List<Queue<Action>> combos;

    private TimelineManager timelineManager;
    private PlayManager playManager;
    private DeckManager deckManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        timelineManager = GameObject.FindGameObjectWithTag("Timeline").GetComponent<TimelineManager>();
        playManager = GameObject.FindGameObjectWithTag("Timeline").GetComponent<PlayManager>();
        deckManager = GetComponentInChildren<DeckManager>();
    }

    public void StartAnimation(string name)
    {
        //Debug.Log("play animation");
        if (name != null) animator.SetTrigger(name);
    }

    public void PauseAnimation()
    {
        animator.speed = 0;
    }

    public void PlayAnimation()
    {
        animator.speed = 1;
    }

    public int GetDamageModifier()
    {
        int mod = damageModifier;
        damageModifier = 0;
        return mod;
    }

    // public void StepIn()
    // {
    //     if (playerType == PlayerType.Player)
    //     {
    //         SetPosition(position + 1);
    //     }
    //     else SetPosition(position - 1);
    // }

    // public void StepOut()
    // {
    //     if (playerType == PlayerType.Player)
    //     {
    //         SetPosition(position - 1);
    //     }
    //     else SetPosition(position + 1);
    // }

    // public void SetPosition(int newPosition)
    // {
    //     position = newPosition; 
    //     transform.localPosition = new Vector3(playManager.positions[newPosition], transform.localPosition.y, transform.localPosition.z);
    // }

    public void Breathe(int length)
    {
        deckManager.DrawCards(length);
        healthManager.Breathe(length);
        animator.SetTrigger("Breathe");
    }

    public void Hit(int damage, bool blocked)
    {
        //Debug.Log(playerType + " hit for " + damage);
        damage = (blocked ? damage / 2 : damage);
        healthManager.DecreaseHealth(damage);
        StartAnimation("Hit");
        DisplayDamage(damage);
    }

    private void DisplayDamage(int damage)
    {
        damageTxt.text = "-" + damage.ToString();
        StartCoroutine(TurnOffDamageText());
    }

    private IEnumerator TurnOffDamageText()
    {
        yield return new WaitForSeconds(5);
        damageTxt.text = "";
    }
}
