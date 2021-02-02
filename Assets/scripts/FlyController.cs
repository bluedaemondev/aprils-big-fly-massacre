using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlyController : MonoBehaviour
{
    [Header("Type : runner, jumper")]
    public string type;
    [Space, Header("Drop / Coins")]
    public List<GameObject> dropPrefabs;

    [Space]
    [Range(0, 1f)]
    public float xSpeed = 1;
    public float damage = 10;
    public int damageMultipTotal = 1;

    public float maxLife = 3;
    public float currentLife = 3;

    public Vector2 headingTo;
    public bool hasPath;

    public BoxCollider2D hitBox;
    public BoxCollider2D agroBox;

    private Rigidbody2D m_Rigidbody2D;
    private Vector3 m_Velocity = Vector3.zero;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.




    public void Move()
    {
        //transform.position -= new Vector3(xSpeed * Time.deltaTime, 0);
        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(-xSpeed * 10f, m_Rigidbody2D.velocity.y);
        // And then smoothing it out and applying it to the character
        m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        #region flip depr
        // If the input is moving the player right and the player is facing left...
        //if (xSpeed > 0 && !m_FacingRight)
        //{
        //    // ... flip the player.
        //    //Flip();
        //}
        //// Otherwise if the input is moving the player left and the player is facing right...
        //else if (xSpeed < 0 && m_FacingRight)
        //{
        //    // ... flip the player.
        //    //Flip();
        //}
        #endregion
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject go = collision.gameObject;
        if (collision.gameObject.CompareTag("bullet"))
        {
            this.currentLife -= PlayerController.current.getDamage();
            //print("fly current life : " + this.currentLife);
            if (this.currentLife <= 0)
            {
                var rand = Random.Range(0, this.dropPrefabs.Count);
                Instantiate(this.dropPrefabs[rand], this.transform.position, Quaternion.identity);
                //print("DEAD fly current life : " + this.currentLife);

                Destroy(this.gameObject, 0.1f);
            }
        }
    }

    private void Start()
    {
        //this.hitBox = GetComponents<BoxCollider2D>().Where(c => c.isTrigger && c.name == ).Single();
        this.m_Rigidbody2D = GetComponent<Rigidbody2D>();
        //InvokeRepeating("")
    }

    private void Update()
    {
        var pp = PlayerController.current.transform.position;
        var isInAgro = Physics2D.BoxCast(transform.position, this.agroBox.size, 0, Vector2.zero, 0, LayerMask.GetMask("Player"));

        if (isInAgro.collider)
        {
            //print("Agro player");
        }
        //RaycastHit2D rchL = Physics2D.Raycast(transform.position, Vector2.left);
        //RaycastHit2D rchR = Physics2D.Raycast(transform.position, Vector2.right);

        //if (Physics2D.BoxCast(this.headingTo, Vector2.one * 1.5f, 0, Vector2.zero))
        //{
        //    this.headingTo = new Vector2();
        //    this.hasPath = false;
        //}

        //if (!hasPath && rchL.transform.CompareTag("ground"))
        //{
        //    this.xSpeed *= -1;
        //    this.hasPath = true;
        //    this.headingTo = Physics2D.Raycast(transform.position, Vector2.right * 5).transform.position;
        //}
        //if (!hasPath && rchR.transform.CompareTag("ground"))
        //{
        //    this.xSpeed *= -1;
        //    this.hasPath = true;
        //    this.headingTo = Physics2D.Raycast(transform.position, Vector2.left * 5).transform.position;
        //}
    }
    void FixedUpdate()
    {
        //Move();
    }

    public float getDamage()
    {
        return this.damage * damageMultipTotal;
    }
}
