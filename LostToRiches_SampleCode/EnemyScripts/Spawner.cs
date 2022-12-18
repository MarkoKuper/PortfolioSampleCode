using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour
{
    public List<GameObject> enemyTypes;

    public List<GameObject> eliteEnemyTypes;
    [Range(0, 100)] public float eliteEnemyChance = 0;      // percentage

    public GameObject spawnVFX;
    public float spawnRadius;
    List<Vector3> spawnLocations;

    int enemyNumber;

    //private void Start()
    //{
    //    StartCoroutine(SetSpawnLocations(1));
    //}

    public IEnumerator SetSpawnLocations(int numberOfEnemies)
    {
        spawnLocations = new List<Vector3>();
        enemyNumber = 0;

        for(int i = 0; i < numberOfEnemies; i++)
        {
            Vector3 spawnPosition = Random.insideUnitCircle * spawnRadius;
            spawnPosition = new Vector3(transform.position.x - spawnPosition.x, 0, transform.position.z - spawnPosition.y);

            // if the spawn location is near a trap, repeat the loop until it's not
            bool isAvoidingTraps = true;
            RaycastHit[] hits = Physics.SphereCastAll(spawnPosition, 2.5f, Vector3.up);
            foreach (RaycastHit hit in hits)
            {
                NavMeshHit navHit;
                if (hit.transform.tag == "Trap" && !NavMesh.SamplePosition(spawnPosition,out navHit,1,0))
                {
                    isAvoidingTraps = false;
                }
            }
            if(!isAvoidingTraps)
            {
                i--;
                continue;
            }

            GameObject spawnInstance = Instantiate(spawnVFX);

            spawnInstance.transform.position = spawnPosition;
            spawnInstance.transform.rotation = Quaternion.Euler
                (new Vector3(spawnInstance.transform.rotation.eulerAngles.x, Random.Range(0, 360), spawnInstance.transform.rotation.eulerAngles.z));

            spawnInstance.GetComponent<SpawnVFX>().SetSpawner(this);

            spawnLocations.Add(spawnPosition);

            yield return new WaitForSeconds(0.5f);
        }
    }

    public void Spawn()
    {

        GameObject enemyInstance;
        if (Random.Range(0,100) < eliteEnemyChance)
        {
            int randomNum = Random.Range(0, eliteEnemyTypes.Count);
            enemyInstance = Instantiate(eliteEnemyTypes[randomNum]);
        }
        else
        {
            int randomNum = Random.Range(0, enemyTypes.Count);
            enemyInstance = Instantiate(enemyTypes[randomNum]);
        }

        enemyInstance.GetComponent<NavMeshAgent>().Warp(spawnLocations[enemyNumber]);

        enemyNumber++;

        // vfx
        enemyInstance.GetComponent<VFX_Handler>().Spawn_SpawningParticle();

        // Audio
        AudioManager.instance.PlaySound("Enemy_Spawn", enemyInstance.transform.position, true);

        SeeThrough_Manager.instance.Update_SeeThroughObjects();
    }
}
