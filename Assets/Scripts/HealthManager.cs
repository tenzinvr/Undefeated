using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [SerializeField] public static int maxHP = 100;
    [System.NonSerialized] public float currentHP;
    private int currentStagger = 0;
    [SerializeField] private int maxStagger = 25;
    [SerializeField] private TMP_Text healthTxt;
    [SerializeField] private TMP_Text staggerTxt;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image staggerBar;
    [SerializeField] private GameObject deathUI;

    private void Start()
    {
        //deathUI = GameObject.FindGameObjectWithTag("DeathUI");
        currentHP = maxHP;
    }

    public void IncreaseHealth(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;
        UpdateHealthBar();
    }
    
    public void DecreaseHealth(int amount)
    {
        //Debug.Log("Damge = " + amount);
        currentHP -= amount;
        if (currentHP <= 0) KnockOut();
        Stagger(amount);
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        healthBar.fillAmount = currentHP / maxHP;
        healthTxt.text = currentHP.ToString();
        //Debug.Log(healthBar.fillAmount);
    }

    private void UpdateStaggerBar()
    {
        staggerBar.fillAmount = currentStagger / maxStagger;
        staggerTxt.text = currentStagger.ToString();
    }

    private void Stagger(int damage)
    {
        currentStagger += damage;
        UpdateStaggerBar();
        if (currentStagger >= 25) KnockOut();
    }

    public void Breathe(int length)
    {
        currentStagger -= length;
        currentStagger = Mathf.Clamp(currentStagger, 0, maxStagger);
        UpdateStaggerBar();
    }

    private void KnockOut()
    {
        deathUI.SetActive(true);
    }
}
