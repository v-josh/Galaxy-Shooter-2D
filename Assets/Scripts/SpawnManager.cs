using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [SerializeField]
    private float _enemySpawnTime = 5.0f;

    [SerializeField]
    private GameObject _enemyPrefab;

    [SerializeField]
    private GameObject _enemyContainer;

    [SerializeField]
    private GameObject[] _powerUps;

    /*
    [SerializeField]
    private GameObject _tripleShotPrefab;

    [SerializeField]
    private GameObject _speedPrefab;
    */

    //Private Variables
    private bool _stopSpawning = false;


    // Start is called before the first frame update
    void Start()
    {

    }

    public void StartSpawning()
    {
        if (_enemyPrefab != null)
        {
            StartCoroutine(SpawnEnemyRoutine());
        }

        StartCoroutine(SpawnPowerupRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3f);


        while (!_stopSpawning)
        {
            //Generate an enemy at a Random number between -8 and 8 on the X axis, 7 on the Y, and 0 at Z
            //The rotation of the clone is the same as the original and attach it to the Enemy Container
            Instantiate(_enemyPrefab, new Vector3(Random.Range(-8, 8), 7f, 0f), Quaternion.identity, _enemyContainer.transform);
            yield return new WaitForSeconds(_enemySpawnTime);

            
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3f);


        while (_stopSpawning == false)
        {
            Vector3 postSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            //GameObject powerUpSpawn = RandomPowerUp();

            //Random Generate Power Ups
            Instantiate(_powerUps[Random.Range(0,_powerUps.Length)], postSpawn, Quaternion.identity);

            //Manual Spawning For Testing purposes only
            //Instantiate(_powerUps[0], postSpawn, Quaternion.identity);    //Triple Shot
            //Instantiate(_powerUps[1], postSpawn, Quaternion.identity);    //Speed
            //Instantiate(_powerUps[2], postSpawn, Quaternion.identity);    //Shield
            //Instantiate(_powerUps[3], postSpawn, Quaternion.identity);    //Ammo Refill

            yield return new WaitForSeconds(Random.Range(3, 8));
        }
    }


}
