using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletController : MonoBehaviour
{
    public float lifespan = 3f;
    public float damage = 100;
    public string direction;
    SpriteRenderer sprRend;
    ParticleSystem particles; 
    // Start is called before the first frame update
    public void Start()
    {
        this.sprRend = GetComponent<SpriteRenderer>();
        //this.particles = GetComponent<ParticleSystem>();
        //this.particles.Play();

        this.damage = PlayerController.current.getDamage();

        var rigidbody = this.GetComponent<Rigidbody2D>();
        switch (direction)
        {
            case "Top":
                //this.sprRend.
                rigidbody.AddForce(new Vector2(0, 5),ForceMode2D.Impulse);
                this.sprRend.flipY = false;
                this.transform.rotation = new Quaternion(0, 0, 90, 1);
                break;
            case "Front":
                this.sprRend.flipX = false;
                rigidbody.AddForce(new Vector2(5,0), ForceMode2D.Impulse);
                break;
            case "Back":
                this.sprRend.flipX = true;
                rigidbody.AddForce(new Vector2(-5, 0), ForceMode2D.Impulse);
                break;
            case "Bot":
                rigidbody.AddForce(new Vector2(0,-5), ForceMode2D.Impulse);
                this.sprRend.flipY = true;
                this.transform.rotation = new Quaternion(0, 0, 90, 1);
                break;
        }
        //print("direction : " + direction + " || Vel: " + rigidbody.velocity);
        Destroy(this.gameObject, lifespan);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.GetComponent<PlayerController>())
            Destroy(this.gameObject);
    }
}
