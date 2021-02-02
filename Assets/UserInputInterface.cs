using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserInputInterface : MonoBehaviour
{
    TMPro.TextMeshProUGUI bossHealth;
    TMPro.TextMeshProUGUI miaTimer;
    public GameObject loseHud;
    private GameObject loseHudInstance;

    public GameObject winHud;
    private GameObject winHudInstance;

    public GameObject missingHud;
    private GameObject missingHudInstance;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 1;
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            StopAllCoroutines();
        }
        if (loseHud && Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1;
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            StopAllCoroutines();
        }
    }

    private void Start()
    {
        PlayerController.current.OnDeathEvent.AddListener(ShowLoseHUD);
        PlayerController.current.OnMiaEvent.AddListener(ShowMissingHUD);

        GameObject.FindObjectOfType<BossController>().OnDeathEvent.AddListener(ShowWinHUD);
        
    }

    public void LoadScene(int idx)
    {
        SceneManager.LoadSceneAsync(idx);
    }
    public void ShowWinHUD()
    {
        if (this.winHudInstance == null)
        {
            this.winHudInstance = Instantiate(winHud, Vector3.zero, Quaternion.identity);
            StartCoroutine(ResetScene());
        }
    }

    public void ShowLoseHUD()
    {
        if (this.loseHudInstance == null)
        {
            this.loseHudInstance = Instantiate(loseHud, Vector3.zero, Quaternion.identity);
            this.bossHealth = this.loseHudInstance.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            bossHealth.text = "boss health : " + GameObject.FindObjectOfType<BossController>().currentLife;
            StartCoroutine(ResetScene());
        }
    }

    public void ShowMissingHUD()
    {
        if (this.missingHudInstance == null)
        {
            

            this.missingHudInstance = Instantiate(missingHud, Vector3.zero, Quaternion.identity);
            this.miaTimer = this.missingHudInstance.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            StartCoroutine(ResetSceneOnPlayerMissing());
        }
        else
        {
            this.missingHudInstance.SetActive(true);
            StartCoroutine(ResetSceneOnPlayerMissing());
        }

    }

    IEnumerator ResetSceneOnPlayerMissing()
    {
        var timer = 5f;

        while (timer > 0)
        {

            this.miaTimer.text = "Restarting mission in " + Mathf.FloorToInt(timer);
            timer -= Time.deltaTime;
            yield return null;
        }
        if (Vector3.Distance(PlayerController.current.transform.position, BossController.current.transform.position) >= 15
            &&
            (PlayerController.current.transform.position.x <= BossController.current.transform.position.x))
        {
            //||
            //PlayerController.current.transform.position.x)

            print("Matando por MIA!");
            StopAllCoroutines();
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);

            //ResetScene(0.2f);
        }
        else
        {
            //OKA
            //this.miaTimer = null;
            //Destroy(missingHudInstance);
            this.missingHudInstance.SetActive(false);
        }

        //var prevScene = SceneManager.GetActiveScene().buildIndex;
        //if (SceneManager.GetActiveScene().buildIndex == prevScene)
        //    SceneManager.LoadSceneAsync(prevScene);
    }
    IEnumerator ResetScene(float t = 3)
    {
        var prevScene = SceneManager.GetActiveScene().buildIndex;
        Time.timeScale = 0.1f;
        PlayerController.current.damage = 0;
        yield return new WaitForSecondsRealtime(t);
        Time.timeScale = 1;
        if (SceneManager.GetActiveScene().buildIndex == prevScene)
            SceneManager.LoadSceneAsync(prevScene);

    }
}
