using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Shop
{
    public class shopController : MonoBehaviour
    {
        public static shopController current;

        public AudioSource messi;

        [Header("Todos los precios")]
        public TextMeshProUGUI damageBuyPrice;
        public TextMeshProUGUI lifeBuyPrice;
        public TextMeshProUGUI falopaBuyPrice;

        public TextMeshProUGUI currentCash;

        [Space]
        public int lifeMaxPrice = 5;
        public int damageMaxPrice = 5;

        [Space]
        public float capitalistRatio = 1.1f;
        // al comprar una mejora, sube el precio para la proxima


        private void Awake()
        {
            shopController.current = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            messi = GameObject.Find("noo messi").GetComponent<AudioSource>();
            CheckPrefs();
            ConfigHUD();
        }
        private void LateUpdate()
        {
            ConfigHUD();
        }

        public void CheckPrefs()
        {
            if (!PlayerPrefs.HasKey("damageMaxPrice"))
            {
                PlayerPrefs.SetInt("damageMaxPrice", this.damageMaxPrice);
            }
            else
            {
                this.damageMaxPrice = PlayerPrefs.GetInt("damageMaxPrice");
            }

            if (!PlayerPrefs.HasKey("lifeMaxPrice"))
            {
                PlayerPrefs.SetInt("lifeMaxPrice", this.lifeMaxPrice);
            }
            else
            {
                this.lifeMaxPrice = PlayerPrefs.GetInt("lifeMaxPrice");
            }


            if (!PlayerPrefs.HasKey("capitalistRatio"))
            {
                PlayerPrefs.SetFloat("capitalistRatio", this.capitalistRatio);
            }
            else
            {
                this.capitalistRatio = PlayerPrefs.GetFloat("capitalistRatio");
            }

            PlayerPrefs.Save();

        }
        public void ConfigHUD()
        {
            this.damageBuyPrice.text = "$ " + this.damageMaxPrice;
            this.lifeBuyPrice.text = "$ " + this.lifeMaxPrice;
            this.falopaBuyPrice.text = "$ falopa";

            currentCash.text = "$  " + PlayerPrefs.GetInt("coins");

        }

        public void BuyItem(string itemName)
        {
            print(itemName);

            switch (itemName)
            {
                case "vidamax":
                    if (PlayerController.current.IncreaseMaxLife(10, lifeMaxPrice))
                    {
                        Convert.ToInt32(Mathf.Abs(this.lifeMaxPrice * this.capitalistRatio));
                        PlayerPrefs.SetInt("lifeMaxPrice", this.lifeMaxPrice);
                        currentCash.text = "$  " + PlayerPrefs.GetInt("coins");
                        print(PlayerPrefs.GetInt("coins"));
                    }
                    else
                        StartCoroutine(FindObjectOfType<CameraShake>().CinemachineShake(0.2f, 5));
                    break;
                case "damagemax":
                    if (PlayerController.current.IncreaseDamage(10, damageMaxPrice))
                    {
                        this.damageMaxPrice = Convert.ToInt32(Mathf.Abs(this.damageMaxPrice * this.capitalistRatio));
                        PlayerPrefs.SetInt("damageMaxPrice", this.damageMaxPrice);
                        currentCash.text = "$  " + PlayerPrefs.GetInt("coins");
                        print(PlayerPrefs.GetInt("coins"));
                    }
                    else
                        StartCoroutine(FindObjectOfType<CameraShake>().CinemachineShake(0.2f, 5));
                    break;
                default:
                    messi.Play();
                    break;
            }
            PlayerPrefs.SetFloat("capitalistRatio", this.capitalistRatio);
            PlayerPrefs.Save();

        }

    }
}