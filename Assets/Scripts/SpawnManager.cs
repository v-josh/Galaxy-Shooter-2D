using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [SerializeField]
    private float _enemySpawnTime = 5.0f;

    [SerializeField]
    //private GameObject _enemyPrefab;

    private GameObject[] _enemyPrefab;

    [SerializeField]
    private GameObject _enemyContainer;

    [SerializeField]
    private float _delaySpawn = 5f;

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
    private float _canSpawn = -1f;
    private bool _randomSpin = true;
    private int _theRandomNumber;


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
            int randomGenerator = Random.Range(0, _enemyPrefab.Length);
            Enemy eScript = _enemyPrefab[randomGenerator].GetComponent<Enemy>();
            int movementNumber = eScript.TheMovement();

            if (movementNumber < 0)
            {

                //Generate an enemy at a Random number between -8 and 8 on the X axis, 7 on the Y, and 0 at Z
                //The rotation of the clone is the same as the original and attach it to the Enemy Container
                Instantiate(_enemyPrefab[randomGenerator], new Vector3(Random.Range(-8, 8), 7f, 0f), Quaternion.identity, _enemyContainer.transform);
            }
            else if(movementNumber == -1)
            {
                Instantiate(_enemyPrefab[randomGenerator], new Vector3(-12f, Random.Range(-4f, 4f)), Quaternion.identity, _enemyContainer.transform);
            }
            else
            {
                Instantiate(_enemyPrefab[randomGenerator], new Vector3(12f, Random.Range(-4f, 4f)), Quaternion.identity, _enemyContainer.transform);

            }
            yield return new WaitForSeconds(_enemySpawnTime);  
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3f);
        

        while (_stopSpawning == false)
        {
            _theRandomNumber = Random.Range(0, _powerUps.Length);
            Vector3 postSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);

            if(_theRandomNumber == 5)
            {
                _randomSpin = true;
            }
            else
            {
                _randomSpin = false;
            }

            
            //Decreasing the spawn of the secondary weapon
            while (_randomSpin)
            {
                yield return new WaitForSeconds(0.5f);
                if (_theRandomNumber == 5)
                {
                    if (Time.time >= _canSpawn)
                    {
                        _canSpawn = Time.time + _delaySpawn;
                        _randomSpin = false;
                    }
                    else
                    {
                        _theRandomNumber = Random.Range(0, _powerUps.Length);
                    }
                }
                else
                {
                    _randomSpin = false;
                }
            }
            

            //Random Generate Power Ups
            Instantiate(_powerUps[_theRandomNumber], postSpawn, Quaternion.identity);

            //Manual Spawning For Testing purposes only
            //Instantiate(_powerUps[0], postSpawn, Quaternion.identity);    //Triple Shot
            //Instantiate(_powerUps[1], postSpawn, Quaternion.identity);    //Speed
            //Instantiate(_powerUps[2], postSpawn, Quaternion.identity);    //Shield
            //Instantiate(_powerUps[3], postSpawn, Quaternion.identity);    //Ammo Refill
            //Instantiate(_powerUps[4], postSpawn, Quaternion.identity);    //Health Refill
            //Instantiate(_powerUps[5], postSpawn, Quaternion.identity);    //Fireworks
            yield return new WaitForSeconds(Random.Range(3, 8));
        }
    }


}
