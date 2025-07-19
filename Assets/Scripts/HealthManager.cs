using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public static int maxHP = 100;
    public float currentHP;
    public TMP_Text text;
    public Image healthBar;
    [SerializeField] private GameObject deathUI;

    private void Start()
    {
        deathUI = GameObject.FindGameObjectWithTag("DeathUI");
        currentHP = maxHP;
        //Debug.Log("DeathUI is null?" + (deathUI == null));
    }

    public void IncreaseHealth(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;
        UpdateHealthBar();
    }
    
    public void DecreaseHealth(int amount)
    {
        //Debug.Log("Decrease health by " + amount);
        currentHP -= amount;
        if (currentHP <= 0) Death();
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        healthBar.fillAmount = currentHP / maxHP;
        text.text = currentHP.ToString();
        //Debug.Log(healthBar.fillAmount);
    }

    private void Death()
    {
        Debug.Log("Death");
        //deathUI.SetActive(true);
    }
}
