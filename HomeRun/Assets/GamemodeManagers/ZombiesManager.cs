using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiesManager : MonoBehaviour
{
    public float spawnFrequency = 1;
    public float spawnLineWidth = 20;
    public GameObject zombiePrefab;
    // Start is called before the first frame update

    private void Start()
    {
        StartCoroutine(SpawnCycle());
    }

    IEnumerator SpawnCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnFrequency);
            SpawnZombie();
        }
    }

    void SpawnZombie()
    {
        Instantiate(zombiePrefab, transform.position + transform.right * Random.Range(-spawnLineWidth / 2, spawnLineWidth / 2), transform.rotation);
    }
}
