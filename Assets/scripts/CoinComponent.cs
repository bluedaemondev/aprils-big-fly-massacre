using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinComponent : MonoBehaviour
{
    //public Collider2D col;
    public LayerMask playerMask;

    private void Start()
    {
        Destroy(this.gameObject, 2f);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        { //player col
            PlayerController.current.AddCash();
            Destroy(this.gameObject);
        }
        
    }
}
