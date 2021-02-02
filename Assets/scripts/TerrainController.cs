using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainController : MonoBehaviour
{
    public float maxDeltaX = 180f;
    public bool justSpawned;
    public float lifetime = 40;
    public GameObject maxBehindPlayer;
    public GameObject maxForwardPlayer;

    public Transform startPoint;
    public Transform endPoint;

    public void Initialize()
    {
        foreach (var item in GetComponentsInChildren<TerrainPoint>())
        {
            if (item.name.Contains("endPoint"))
                this.endPoint = item.transform;
            else
                this.startPoint = item.transform;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        

        this.justSpawned = true;
        this.maxBehindPlayer = GameObject.Find("maxDistance_Behind");
    }

    void Update()
    {
        if (Vector2.Distance(maxBehindPlayer.transform.position, this.transform.position) >= maxDeltaX) //(Mathf.Abs(maxBehindPlayer.transform.position.x - this.transform.position.x) >= maxDeltaX)
        {
            TerrainGeneratorV1.current.currentWorldTiles.Remove(this.gameObject);
            Destroy(this.gameObject);
        }
    }
}
