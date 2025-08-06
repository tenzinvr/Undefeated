using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject exitMenu;
    [SerializeField] private Animator playerAnimator;
    private string[] animations = { "Jab", "Cross", "Lead Hook", "Rear Hook", "Lead Uppercut", "Rear Uppercut", "Slip", "Bob" };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu") RandomAnimation();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // If the game is paused, resume it
            if (Time.timeScale == 0)
            {
                exitMenu.SetActive(true);
                Time.timeScale = 1;
            }
            // If the game is running, pause it
            else
            {
                exitMenu.SetActive(false);
                Time.timeScale = 0;
            }
        }
    }

    public void ExitGame()
    {
        // Save any necessary game state here
        Debug.Log("Exiting game...");
        Application.Quit();
    }

    public void MainMenuClick()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void TutorialClick()
    {
        SceneManager.LoadScene("TutorialScene");
    }

    public void StartClick()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ResumeClick()
    {
        exitMenu.SetActive(false);
        Time.timeScale = 1;
    }

    private void RandomAnimation()
    {
        int random = UnityEngine.Random.Range(0, animations.Length);
        playerAnimator.SetTrigger(animations[random]);
        StartCoroutine(WaitForAnimation());
    }

    private IEnumerator WaitForAnimation()
    {
        yield return new WaitForSeconds(1f);
        RandomAnimation();
    }
}
