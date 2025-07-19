using UnityEngine;

public class Player : MonoBehaviour
{
    [System.NonSerialized] public Animator animator;
    [SerializeField] private HealthManager healthManager;

    public PlayerType playerType;
    public int health = 100;
    public int timeModifier = 0;
    public bool isBlocking;
    public Distance distance;
    public Stance stance;
    public int lastPointEvaluated = 0;
    public int lastPointToEvaluate = 0;
    private string[] animationNames = new string[10];

    private TimelineManager timelineManager;
    private PlayManager playManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        timelineManager = GameObject.FindGameObjectWithTag("Timeline").GetComponent<TimelineManager>();
        playManager = GameObject.FindGameObjectWithTag("Timeline").GetComponent<PlayManager>();
    }

    public void StartAnimation(string name)
    {
        if (name != null) animator.SetTrigger(name);
    }

    private void NextAnimation()
    {

    }

    public void Hit(int damage, bool blocked)
    {
        damage = (blocked ? damage / 2 : damage);
        healthManager.DecreaseHealth(damage);
        StartAnimation("Hit");
    }
}
