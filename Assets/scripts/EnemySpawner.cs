using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;
    public float invokeTime = 3;
    public int maxEnemyCountFromSpawn = 10; // maximo que puede tener en simultaneo para no desbordar
    public List<GameObject> currentEnemies;
    
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnEnemy",0,invokeTime);
    }

    void SpawnEnemy()
    {
        if(enemyPrefabs.Count > 1 && currentEnemies.Count < maxEnemyCountFromSpawn)
        {
            var rand = Random.Range(0, enemyPrefabs.Count);
            Instantiate(enemyPrefabs[rand], this.transform.position,Quaternion.identity);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
