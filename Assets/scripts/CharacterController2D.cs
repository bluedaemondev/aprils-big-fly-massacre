using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
    [SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;

    private bool goingDown;
    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;
    public bool m_wasCrouching = false;
    public bool isSliding = false;
    public float slideSpeed = 10f;

    private static PlayerController currentPlayer;
    private float slideTime = 0.8f;

    private void Awake()
    {
        currentPlayer = GameObject.FindObjectOfType<PlayerController>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();

        OnCrouchEvent.AddListener(Dash);

    }

    IEnumerator stopSlide()
    {
        PlayerController.current.animator.SetBool("isCrouching", true);

        yield return new WaitForSecondsRealtime(slideTime);


        //this.GetComponent<Animator>().Play("headbang1");
        PlayerController.current.animator.SetBool("isCrouching", false);
        // Enable the collider when not crouching

        if (!Input.GetKey(KeyCode.S) && 
            Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround) &&
            m_CrouchDisableCollider != null)
        {
          print("verga");

            //if ()
            m_Rigidbody2D.AddForce((Vector3)(m_Rigidbody2D.velocity.y >= 0 ? Vector2.one * 3 : -Vector2.one * 3), ForceMode2D.Impulse);
            m_CrouchDisableCollider.enabled = true;
            print("overlaping addd formceasdgasdasd  "+ m_Rigidbody2D.velocity);
        }
        else if(Input.GetKey(KeyCode.S))
        {
            print("eval");
            m_CrouchDisableCollider.enabled = false;
            print("shouldStayDisabled??");
            //m_Rigidbody2D.AddForce((Vector3)(m_Rigidbody2D.velocity.y >= 0 ? -Vector2.one * 3 : Vector2.one * 3), ForceMode2D.Impulse);
        }


        this.isSliding = false;

    }
    /// <summary>
    /// Realiza un impulso estando agachado hacia adelante. recorre 4 tiles
    /// </summary>
    /// <param name="val"></param>
    private void Dash(bool val)
    {

        // Disable one of the colliders when crouching
        if (val)
        {
            //if (m_CrouchDisableCollider != null)
            //    m_CrouchDisableCollider.enabled = false;
            StartCoroutine(stopSlide());
            this.isSliding = true;

            if (transform.localScale.x < 0)
            {
                //facing left
                m_Rigidbody2D.AddForce(Vector2.left * slideSpeed, ForceMode2D.Impulse);
            }
            else
            {
                m_Rigidbody2D.AddForce(Vector2.right * slideSpeed, ForceMode2D.Impulse);
            }
            
        }
        //if (val)
        //{
        //    //print("Dashing!");
        //    Vector3 targetVelocity =
        //        new Vector2(currentPlayer.minDashSpeed * Mathf.Sign(m_Rigidbody2D.velocity.x), 0f); 
        //    print("m_Rigidbody2D.velocity.x " + m_Rigidbody2D.velocity.x);

        //    var vel = new Vector3(
        //        Mathf.Clamp(
        //            m_Rigidbody2D.velocity.x,
        //            targetVelocity.x <= 0 && !m_FacingRight ? targetVelocity.x : -targetVelocity.x,
        //            //targetVelocity.x,
        //            targetVelocity.x >= 0 && m_FacingRight ? targetVelocity.x : -targetVelocity.x),
        //            //currentPlayer.minDashSpeed * -1,
        //            //currentPlayer.minDashSpeed),
        //        m_Rigidbody2D.velocity.y);
        //    print(vel);

        //    var damped = Vector3.SmoothDamp(vel, targetVelocity*4.5f, ref m_Velocity, m_MovementSmoothing*2);

        //    m_Rigidbody2D.velocity = damped;
        //    //print(damped);
        //   // m_Rigidbody2D.AddForce(new Vector2(0, -10));
        //}

    }

    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;


        if (m_Rigidbody2D.velocity.y < 0f && m_Rigidbody2D.velocity.y >= -10f && !m_Grounded)
        {
            //print(Mathf.Cos(m_Rigidbody2D.velocity.x));
            var target = new Vector2(0,
                    (1 + Mathf.Cos(m_Rigidbody2D.velocity.x)) * m_Rigidbody2D.velocity.y);
            //print("prev_Velocity " + m_Rigidbody2D.velocity);
            if (Mathf.Cos(m_Rigidbody2D.velocity.x) != 1)
                m_Rigidbody2D.AddForce(Vector3.SmoothDamp(m_Rigidbody2D.velocity, target, ref m_Velocity, m_MovementSmoothing));
            //print("post_Velocity " + m_Rigidbody2D.velocity);

        }

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                currentPlayer.canDoubleJump = true;

                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }
    }


    public void Move(float move, bool crouch, bool jump, bool flip)
    {
        // If crouching, check to see if the character can stand up
        if (!crouch)
        {
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
            {
                crouch = true;
            }
        }

        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {

            // If crouching
            if (crouch)
            {
                if (!m_wasCrouching)
                {
                    m_wasCrouching = true;
                    OnCrouchEvent.Invoke(true);
                }

                // Reduce the speed by the crouchSpeed multiplier
                //move *= m_CrouchSpeed;

                // Disable one of the colliders when crouching
                //if (m_CrouchDisableCollider != null)
                //    m_CrouchDisableCollider.enabled = false;
            }
            else
            {
                // Enable the collider when not crouching
                //if (m_CrouchDisableCollider != null)
                //    m_CrouchDisableCollider.enabled = true;

                if (m_wasCrouching)
                {
                    m_wasCrouching = false;
                    OnCrouchEvent.Invoke(false);
                }
            }

            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
            // And then smoothing it out and applying it to the character
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight && flip)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight && flip)
            {
                // ... flip the player.
                Flip();
            }
        }
        // If the player should jump...
        if (m_Grounded && jump)
        {
            // Add a vertical force to the player.
            m_Grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce), ForceMode2D.Impulse);
        }
        else if (!m_Grounded && jump && currentPlayer.canDoubleJump)
        {
            // Add a vertical force to the player.
            m_Grounded = false;
            currentPlayer.canDoubleJump = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce * 0.3f), ForceMode2D.Impulse);
        }
    }


    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
