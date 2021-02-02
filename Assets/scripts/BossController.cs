using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossController : MonoBehaviour
{
    [Header("DONT TOUCH")]
    public static BossController current;

    BoxCollider2D damageCol;
    Animator animator;
    [Space, Header("Vida")]
    public float maxLife = 1000;
    public float currentLife;

    [Space, Header("Propiedades de ataque")]
    public UnityEngine.UI.Slider healthBar;
    public int enemiesToSpawn = 0;
    public List<GameObject> enemyPrefabs;
    public GameObject spikesPrefab;
    public GameObject enemySpawnerObject; // celda para spawnear moscas en el ataque
    
    [Space, Header("Visual feedback")]
    public Camera cam;
    public List<Color> bgColors;

    public UnityEvent OnDeathEvent;
    private int damage = 13;
    private int damageMultipTotal = 1;

    private void Awake()
    {
        BossController.current = this;

        if (OnDeathEvent == null)
            this.OnDeathEvent = new UnityEvent();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.damageCol = this.GetComponent<BoxCollider2D>();
        //this.maxLife = 1000;
        this.currentLife = maxLife;
        this.cam = Camera.main;

        damage = 13;
        damageMultipTotal = 1;

        this.CheckPrefs();
        this.ConfigHUD();

        //evento de ataque cada 10 segundos, partiendo a los 10 segundos de juego
        InvokeRepeating("Attack", 10, 10);
    }
    void CheckPrefs()
    {
        if (!PlayerPrefs.HasKey("bossMaxLife"))
        {
            PlayerPrefs.SetFloat("bossMaxLife", this.maxLife);
        }
        else
        {
            this.maxLife = PlayerPrefs.GetFloat("bossMaxLife");
        }

        if (!PlayerPrefs.HasKey("bossCurrentLife"))
        {
            PlayerPrefs.SetFloat("bossCurrentLife", this.currentLife);
        }
        else
        {
            this.currentLife = PlayerPrefs.GetFloat("bossCurrentLife");
        }
        PlayerPrefs.Save();
    }

    void ConfigHUD()
    {
        this.healthBar.maxValue = this.maxLife;
        this.healthBar.minValue = 0;
        this.healthBar.value = this.currentLife;
    }

    float DecreaseLife(float value)
    {
        this.currentLife -= value;
        PlayerPrefs.SetFloat("bossCurrentLife", this.currentLife);
        PlayerPrefs.Save();

        //this.cam.backgroundColor = new Color(Mathf.Clamp(this.currentLife / 10, 40, 255),
        //                                     180,
        //                                     80);
        //ui update
        this.healthBar.value = Mathf.Clamp(this.currentLife, this.healthBar.minValue, this.healthBar.maxValue);

        if (this.currentLife <= 0)
            OnDeathEvent.Invoke();

        //ataque media vida
        if (this.currentLife < (0.6f * this.maxLife) && this.currentLife >= (0.3f * this.maxLife))
            this.enemiesToSpawn = 5;
        //ataque poca vida
        else if (this.currentLife > 0)
            this.enemiesToSpawn = 7;
        //muerto o full vida
        else
            enemiesToSpawn = 0;

        StartCoroutine(FindObjectOfType<CameraShake>().CinemachineShake(0.1f, PlayerController.current.getDamage()));

        return this.currentLife;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("bullet"))
        {
            DecreaseLife(collision.gameObject.GetComponent<bulletController>().damage);
            //if (collision.gameObject)
            //    Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController.current.DecreaseLife(this.getDamage());
        }
    }
    private void Attack()
    {
        if (this.enemiesToSpawn != 0)
            print("spawning " + this.enemiesToSpawn);
            //this.SpawnEnemies();
        if(this.currentLife < this.maxLife * 0.3 && this.currentLife > 0)
            this.InvokeSpikes();
    }
    private void InvokeSpikes()
    {
        print("SPIKE!!");
    }
    private void SpawnEnemies()
    {
        if(this.currentLife < (0.6f * this.maxLife) && this.currentLife > 0) //alive and attacking
        {
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                int rand = UnityEngine.Random.Range(0, enemyPrefabs.Count);
                GameObject fly = Instantiate(enemyPrefabs[rand], CalculateEmptyCell(), Quaternion.identity);
            }
        }


    }

    private Vector3 CalculateEmptyCell()
    {
        Vector3 newPos = Vector3.zero;
        if(Physics2D.BoxCast(this.enemySpawnerObject.transform.position, Vector2.one, 0, Vector2.zero))
        {
            newPos = this.enemySpawnerObject.transform.position + new Vector3(1, 0, 0); //busca el adyacente + cercano
        } 
        return newPos;
    }

    public float getDamage()
    {
        return this.damage * damageMultipTotal;
    }
}
