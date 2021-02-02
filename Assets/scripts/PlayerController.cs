using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


public class PlayerController : MonoBehaviour
{
    public static PlayerController current;
    public CharacterController2D characterController;
    public CameraShake cameraShake;

    [Space]
    public float xSpeed = 12f;
    public float currentVelocityX = 0;

    public void AddCash()
    {
        this.currentCash++;
        PlayerPrefs.SetInt("coins", this.currentCash);
        this.coinCounter.text = "x" + currentCash;
    }

    public float maxVelocityX = 15f;


    public float minDashSpeed = 25 * 8f;
    public float jumpMaxForce = 400f;
    public float dragMaxTime = 0.3f;
    [Space, Header("Player atributes")]
    public float lifeMax = 100;
    public float lifeActual = 100;
    public int rageMax = 100;
    public int rageActual = 0;
    public float regen = 1;
    public float regenMultip = 0.05f;

    [Space]
    public int currentCash = 0;



    //[Space]
    public Animator animator;
    BoxCollider2D collBox;
    Rigidbody2D rigidBody;
    BoxCollider2D hitBox;
    public SpriteRenderer spriteRend;




    [Space]
    public List<GameObject> weapons;
    public GameObject bulletPrefab;
    public float damage = 1f;
    public float damageMultipTotal = 1f;
    public float bulletSpeed = 50f;

    [Space, Header("HUD")]
    public UnityEngine.UI.Slider healthBar;
    public UnityEngine.UI.Slider rageBar;
    public UnityEngine.UI.Image avatar;
    public TMPro.TextMeshProUGUI coinCounter;
    public GameObject particlePlayer;

    private GameObject currentParticle;


    [Space]
    public bool canDoubleJump = false;
    public bool grounded = false;
    public bool crouching = false;
    public bool canShoot = true;

    public bool loadHud = false;


    public List<LayerMask> collisionMasks;
    public LayerMask enemyMask;

    public UnityEvent OnDeathEvent;
    public UnityEvent OnMiaEvent;


    public GameObject maxDistanceBehind;
    public GameObject maxDistanceForward;
    public Sprite crouchSprite;
    public Sprite standSprite;

    private void Awake()
    {
        PlayerController.current = this;

        if (OnDeathEvent == null)
            this.OnDeathEvent = new UnityEvent();
        if (OnMiaEvent == null)
            this.OnMiaEvent = new UnityEvent();
    }
    // Start is called before the first frame update
    void Start()
    {
        this.maxDistanceBehind = GameObject.Find("maxDistance_Behind");
        this.maxDistanceForward = GameObject.Find("maxDistance_Forward");

        //if (animator == null)
        //this.animator = GetComponent<Animator>();

        this.spriteRend = GetComponent<SpriteRenderer>();
        this.rigidBody = GetComponent<Rigidbody2D>();
        this.collBox = GetComponent<BoxCollider2D>();
        this.hitBox = GetComponents<BoxCollider2D>().Where(c => c.isTrigger).Single();
        this.GetComponent<CharacterController2D>().OnCrouchEvent.AddListener(this.Crouch);

        CheckPrefs();
        if (loadHud)
            this.ConfigHUD();

        this.standSprite = this.spriteRend.sprite;
    }
    public void CheckPrefs()
    {
        if (!PlayerPrefs.HasKey("coins"))
        {
            PlayerPrefs.SetInt("coins", this.currentCash);
        }
        else
        {
            this.currentCash = PlayerPrefs.GetInt("coins");
            print("cash saving " + PlayerPrefs.GetInt("coins"));
        }

        if (!PlayerPrefs.HasKey("lifeMax"))
        {
            PlayerPrefs.SetFloat("lifeMax", this.lifeMax);
        }
        else
        {
            this.lifeMax = PlayerPrefs.GetFloat("lifeMax");
        }

        if (!PlayerPrefs.HasKey("damagePoints"))
        {
            PlayerPrefs.SetFloat("damagePoints", this.damage);
        }
        else
        {
            this.damage = PlayerPrefs.GetFloat("damagePoints");
        }
        PlayerPrefs.Save();

    }
    void ConfigHUD()
    {
        this.healthBar.maxValue = this.lifeMax;
        this.healthBar.minValue = 0;
        this.healthBar.value = Mathf.Clamp(this.lifeActual, this.healthBar.minValue, this.healthBar.maxValue);
        this.rageBar.maxValue = this.rageMax;
        this.rageBar.minValue = 0;
        this.rageBar.value = Mathf.Clamp(this.rageActual, this.rageBar.minValue, this.rageBar.maxValue);
        this.coinCounter.text = "x" + this.currentCash;
    }

    public float getDamage()
    {
        print("player pref dmg = " + PlayerPrefs.GetFloat("damagePoints") + " , actual : " + this.damage);
        return this.damage * damageMultipTotal;
    }

    private List<GameObject> getLateralGunpoints()
    {
        return this.weapons
                   .Where(element => element.name.Split('_')[1] == "Front" ||
                          element.name.Split('_')[1] == "Back")
                   .ToList();
    }
    private List<GameObject> getDoubleJumpGunpoints()
    {
        //var lst = new List<GameObject>();
        //lst.AddRange(getLateralGunpoints());
        //lst.AddRange(this.weapons.Where())
        return this.weapons;
        //                .Where(element => element.name == "gunOriginFront" ||
        //                       element.name == "gunOriginBack")
        //                .ToList();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        SpriteRenderer enemyrend;

        GameObject enemy = other.gameObject;
        enemy.TryGetComponent<SpriteRenderer>(out enemyrend);

        if (enemyrend?.renderingLayerMask == enemyMask.value)
        {
            var x = DecreaseLife(enemy.GetComponent<FlyController>() ?
                        enemy.GetComponent<FlyController>().getDamage() :
                         enemy.GetComponent<BossController>().getDamage());



        }
    }

    //private void OnCollisionEnter2D(Collision2D other)
    //{
    //    GameObject enemy = other.gameObject;
    //    if (enemy.GetComponent<SpriteRenderer>().renderingLayerMask == enemyMask.value)
    //    {
    //        if (DecreaseLife(enemy.GetComponent<FlyController>().damage) <= 0)
    //        { //murió
    //            this.OnDeathEvent.Invoke();
    //        }
    //    }
    //}

    public float DecreaseLife(float value)
    {
        //print("verga " + value);
        this.lifeActual -= value;
        //this.healthBar.value = Mathf.Clamp(this.lifeActual, this.healthBar.minValue, this.healthBar.maxValue);
        if (loadHud)
            ConfigHUD();
        /// check if enemy or boss
        if (lifeActual <= 0)
        { //murió
            this.OnDeathEvent.Invoke();
        }

        StartCoroutine(cameraShake.CinemachineShake(0.3f, 10));

        return this.lifeActual;
    }
    void CheckIfMia()
    {
        if (Vector3.Distance(this.transform.position, BossController.current.transform.position) >= 15
            &&
            (this.transform.position.x <= BossController.current.transform.position.x))
        {
            OnMiaEvent.Invoke();
        }
    }

    void Update()
    {
        bool jump = Input.GetKeyDown(KeyCode.W) | Input.GetKeyDown(KeyCode.UpArrow) | Input.GetKeyDown(KeyCode.Space);
        bool crouch = Input.GetKeyDown(KeyCode.S) | Input.GetKeyDown(KeyCode.DownArrow);
        bool doubleJumped = !this.grounded && jump && canDoubleJump;

        var boss = GameObject.FindObjectOfType<BossController>();
        if (boss != null)
        {
            CheckIfMia();

            GameObject rabbit = GameObject.Find("cameraRabbit");
            float dst = Vector3.Distance(rabbit.transform.position, transform.position);

            if (dst > 35)
            {
                Vector3 vect = rabbit.transform.position - transform.position;
                vect = vect.normalized;
                vect *= (dst - 35);
                transform.position += vect;
            }
        }
        characterController.Move(Input.GetAxisRaw("Horizontal"), crouch, jump, true);


        //if (Vector3.Distance(transform.position, BossController.current.transform.position) <= 35 &&
        //BossController.current.transform.position.x < transform.position.x)
        //else
        //    characterController.Move(-Input.GetAxisRaw("Horizontal"), crouch, jump, false);
        //    //print("no tengo idea que puede haber aca");


        ///Animator status
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            this.animator.SetBool("isRunning", true);
            Destroy(currentParticle);
        }
        else //idle
        {
            this.animator.SetBool("isRunning", this.rigidBody.velocity.x != 0 ? true : false);


            if (this.lifeActual < lifeMax)
            {
                this.lifeActual += this.regen * this.regenMultip;
                if (loadHud)
                    this.healthBar.value = lifeActual;
            }

            if (this.currentParticle == null)
            {
                this.currentParticle = Instantiate(particlePlayer, this.transform.position, Quaternion.identity); //test
                //this.gameObject.
            }
        }

        //if (crouch)
        //{
        //    this.animator.SetBool("isCrouching", true);
        //}


        if (jump)
            Jump(doubleJumped);
    }

    public void Crouch(bool val)
    {
        //if (!val && this.spriteRend.sprite == this.crouchSprite)
        //    this.spriteRend.sprite = this.standSprite;

        //else
        //    this.spriteRend.sprite = this.crouchSprite;

        //this.animator.SetBool("isCrouching", val);

        //print(val);
        //this.spriteRend.sprite = this.crouchSprite;
        if (canShoot)
        {
            foreach (var gunpoint in getLateralGunpoints())
            {
                var timeBetweenShots = 0f;

                for (int i = 0; i < 3; i++)
                {
                    StartCoroutine(ShootWithDeelay(timeBetweenShots, gunpoint));
                    timeBetweenShots += 0.1f * i;
                }
            }
            StartCoroutine(ResetCooldownWeapon(0.8f));
        }
    }

    private IEnumerator ShootWithDeelay(float dTime, GameObject gunpoint)
    {
        yield return new WaitForSecondsRealtime(dTime);

        GameObject bullet;
        string reversed = "";
        reversed = gunpoint.name.Split('_')[1] == "Front" ? "Back" : "Front";

        bullet = Instantiate(this.bulletPrefab, gunpoint.transform.position, Quaternion.identity);
        var bc = bullet.AddComponent<bulletController>();

        if (this.transform.localScale.x < 0)
            bc.direction = reversed;
        else
            bc.direction = gunpoint.name.Split('_')[1];


        float bulletRotation = reversed == "Back" && transform.localScale.x < 0 ? 180 : 0;

        if (bulletRotation == 0 && gunpoint.name.Contains("Top"))
            bulletRotation = 90;
        else if (gunpoint.name.Contains("Bot"))
            bulletRotation = -90;

        bullet.transform.localRotation = new Quaternion(0, 0, bulletRotation, 1);
        //le paso la direccion a la que va, dependiendo el origen

    }


    public IEnumerator ResetCooldownWeapon(float t)
    {
        this.canShoot = false;
        yield return new WaitForSecondsRealtime(t);
        this.canShoot = true;
    }

    public void Jump(bool doubleJumped)
    {
        if (canShoot)
        {
            var timeBetweenShots = 0f;

            foreach (var gunpoint in doubleJumped ? getLateralGunpoints() : getDoubleJumpGunpoints())
            {
                StartCoroutine(ShootWithDeelay(timeBetweenShots, gunpoint));
                timeBetweenShots += 0.1f;
            }
            StartCoroutine(ResetCooldownWeapon(0.95f));
        }
    }

    public bool IncreaseMaxLife(float valToAdd, int price)
    {
        if (this.currentCash >= price)
        {
            this.currentCash -= price;
            this.lifeMax += valToAdd;
            PlayerPrefs.SetFloat("lifeMax", this.lifeMax);
            PlayerPrefs.SetInt("coins", this.currentCash);
            PlayerPrefs.Save();
            if (loadHud)
                this.ConfigHUD();

            return true;
        }
        else
        {
            return false;
        }
    }
    public bool IncreaseDamage(int valToAdd, int price)
    {
        if (this.currentCash >= price)
        {
            this.currentCash -= price;
            this.damage += valToAdd;
            PlayerPrefs.SetFloat("damagePoints", this.damage);
            PlayerPrefs.SetInt("coins", this.currentCash);
            PlayerPrefs.Save();

            //this.ConfigHUD();         //deberia actualizar si tengo esto en algun lado
            return true;
        }
        else
        {
            return false;
        }
    }



}
