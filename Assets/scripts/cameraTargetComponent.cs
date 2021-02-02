using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraTargetComponent : MonoBehaviour
{
    public float xSpeed = 5;
    public bool active = true;

    public Pathfinding.GridGraph graph;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (active) { 
            this.transform.position += new Vector3(xSpeed * Time.deltaTime, 0);
            //graph.center = 
        }
    }
}
