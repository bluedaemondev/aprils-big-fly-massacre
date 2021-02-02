using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantController : MonoBehaviour
{
    public static MerchantController current;

    [Header("Menu de mejoras")]
    public GameObject prefabMenuShop;
    private GameObject menushop;


    [Space,Header("Para dialogos post y pre compra")]
    public TMPro.TextMeshProUGUI uiInScreen;
    public UnityEngine.Event OnPurchase;
    public UnityEngine.Event On;


    private void Awake()
    {
        MerchantController.current = this;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (menushop == null)
            this.menushop = Instantiate(prefabMenuShop, this.transform, true);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (menushop != null)
        {
            Destroy(this.menushop);
        }
    }
    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
