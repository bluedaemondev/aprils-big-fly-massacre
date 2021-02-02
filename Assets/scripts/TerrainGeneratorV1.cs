using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TerrainGeneratorV1 : MonoBehaviour
{
    public static TerrainGeneratorV1 current;
    public GameObject levelRoot;

    public List<GameObject> terrainPrefabs;
    public float xDistanceOffset = 24f;


    public GameObject lastSpawned;
    public GameObject lastEndpointSpawned;

    Vector3 lastPos;
    public float maxDeltaX = 30f;

    public List<GameObject> currentWorldTiles;
    public int maxTiles = 10;

    private void Awake()
    {
        TerrainGeneratorV1.current = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (levelRoot == null)
            this.levelRoot = GameObject.Find("levelRoot");

        lastPos = levelRoot.transform.position;
        
        for (int i = 0; i < 5; i++)
            GenerateBasedOnLast(lastPos);

    }

    GameObject GenerateBasedOnLast(Vector3 currentPos)
    {

        int randVal = Random.Range(0, this.terrainPrefabs.Count);
        GameObject newTerr = Instantiate(terrainPrefabs[randVal], currentPos, Quaternion.identity);
        var tc = newTerr.GetComponent<TerrainController>();
        tc.Initialize();

        float deltaDistance = 0;

        if (lastSpawned != null)
        {
            deltaDistance = Mathf.Sign(currentPos.x - tc.endPoint.position.x) *
                                       (currentPos.x - tc.endPoint.position.x); //distancia desde el root al current endpoint

            deltaDistance += Mathf.Sign(tc.transform.position.x - tc.startPoint.position.x) *
                                       (tc.transform.position.x - tc.startPoint.position.x); //distancia desde el nuevo root al nuevo startpoint
        }

        newTerr.transform.position += new Vector3(deltaDistance, 0, 0);

        lastSpawned = newTerr;
        lastPos = newTerr.transform.position;
        currentWorldTiles.Add(lastSpawned);

        return newTerr;
    }

    void Update()
    {
        //aca deberia instanciar una parte grande de nivel en vez de generar aleatorio, donde haya un cuarto con la bazuka
        if (Vector3.Distance(lastPos, PlayerController.current.maxDistanceForward.transform.position) <= maxDeltaX &&
          this.currentWorldTiles.Count < this.maxTiles)
        {
            GenerateBasedOnLast(lastPos);
        }
        //else
        //print(Vector3.Distance(lastPos, PlayerController.current.maxDistanceForward.transform.position));
    }
}
