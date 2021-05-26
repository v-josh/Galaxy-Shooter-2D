using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [Header("Enemies Info")]




    [SerializeField]
    private GameObject _enemyContainer;

    [SerializeField]
    private int _eneimesToStartWave = 10;

    [SerializeField]
    private int _additionalEnemiesPerWave = 10;

    [SerializeField]
    private float _enemySpawnTime = 5.0f;

    [SerializeField]
    private GameObject[] _enemyPrefab;

    [SerializeField]
    private float[] _enemyProbability;

    [Header("Power Up Info")]
    [SerializeField]
    private GameObject[] _powerUps;

    [SerializeField]
    private float[] _powerUpProbability = new float[10];

    [Header("Boss Info")]
    [SerializeField]
    private GameObject _theBoss;

    [SerializeField]
    private int _totalEnemyWave = 5;

    /*
    [SerializeField]
    private GameObject _tripleShotPrefab;

    [SerializeField]
    private GameObject _speedPrefab;
    */

    //Private Variables
    private bool _stopSpawning = false;
    private bool _enemySpin = true;
    private bool _randomSpin = true;
    private int _theRandomNumber;

    private float _healthOdds = 0f;
    private float _ammoOdds = 0f;

    private UIManager _uI;
    private bool _waveChange = false;
    private int _enemiesLeft;
    private int _currentWaves = 1;
    private int _totalEnemiesSpawn;
    private bool _waveComplete = false;

    private int _enemiesSpawned = 0;
    private bool _spawnBoss = false;
    private bool _bossSpawned = false;

    


    // Start is called before the first frame update
    void Start()
    {
        _enemiesLeft = _eneimesToStartWave;
        _totalEnemiesSpawn = _enemiesLeft;
        if (_uI == null)
        {
            _uI = GameObject.FindWithTag("UI").GetComponent < UIManager >();
        }

        _uI.EnemiesLeft(_enemiesLeft);
        _uI.CurrentWave(_currentWaves);
    }

    public void StartSpawning()
    {
        _uI.ShowWaveInfo();

        if (_enemyPrefab != null)
        {
            StartCoroutine(SpawnEnemyRoutine());
        }

        StartCoroutine(SpawnPowerupRoutine());

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


            EnemiesSpawned();

            int randomGenerator = Random.Range(0, _enemyPrefab.Length);
            while (_enemySpin)
            {
                float enemyRandom = Random.value;
                float enemyValue = _enemyProbability[randomGenerator] * _currentWaves;
                //Debug.Log("Picked " + enemyRandom + " While probability at: " + (1f- enemyValue));
                if (enemyRandom > (1f - enemyValue))
                {
                    _enemySpin = false;
                }
                else
                {
                    randomGenerator = Random.Range(0, _enemyPrefab.Length);
                }
            }

            Enemy eScript = _enemyPrefab[randomGenerator].GetComponent<Enemy>();
            int movementNumber = eScript.TheMovement();

            if (_waveComplete == false)
            {
                if (_enemiesLeft > 0)
                {
                    if (movementNumber >= 0)
                    {

                        //Generate an enemy at a Random number between -8 and 8 on the X axis, 7 on the Y, and 0 at Z
                        //The rotation of the clone is the same as the original and attach it to the Enemy Container
                        Instantiate(_enemyPrefab[randomGenerator], new Vector3(Random.Range(-8, 8), 7f, 0f), Quaternion.identity, _enemyContainer.transform);
                    }
                    else if (movementNumber == -1)
                    {
                        Instantiate(_enemyPrefab[randomGenerator], new Vector3(-12f, Random.Range(-4f, 4f)), Quaternion.identity, _enemyContainer.transform);
                    }
                    else
                    {
                        Instantiate(_enemyPrefab[randomGenerator], new Vector3(12f, Random.Range(-4f, 4f)), Quaternion.identity, _enemyContainer.transform);

                    }
                    _enemySpin = true;
                }
            }
            
            else
            {
                if (_enemiesLeft == 0)
                {
                    //Debug.Log("Changing Waves inside CoRoutine");
                    ChangingWave();
                }
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

            //Decreasing the spawn of the secondary weapon
            while (_randomSpin)
            {
                float randomValue = Random.value;

                float probValue = _powerUpProbability[_theRandomNumber];

                switch (_theRandomNumber)
                {
                    case 3:
                        probValue += _ammoOdds;

                        if(probValue > 1f)
                        {
                            probValue = 1f;
                        }
                        else if(probValue < 0f)
                        {
                            probValue = _powerUpProbability[_theRandomNumber];
                        }
                        break;
                    case 4:
                        probValue += _healthOdds;

                        if(probValue > 1f)
                        {
                            probValue = 1f;
                        }
                        else if(probValue < 0f)
                        {
                            probValue = _powerUpProbability[_theRandomNumber];
                        }

                        break;
                    default:
                        break;
                }

                if (randomValue > (1f - probValue) )
                {
                    _randomSpin = false;
                }
                else
                {
                    _theRandomNumber = Random.Range(0, _powerUps.Length);
                    yield return new WaitForSeconds(0.1f);
                }
            }
            

            //Random Generate Power Ups
            Instantiate(_powerUps[_theRandomNumber], postSpawn, Quaternion.identity);
            _randomSpin = true;
            //Manual Spawning For Testing purposes only
            //Instantiate(_powerUps[0], postSpawn, Quaternion.identity);    //Triple Shot
            //Instantiate(_powerUps[1], postSpawn, Quaternion.identity);    //Speed
            //Instantiate(_powerUps[2], postSpawn, Quaternion.identity);    //Shield
            //Instantiate(_powerUps[3], postSpawn, Quaternion.identity);    //Ammo Refill
            //Instantiate(_powerUps[4], postSpawn, Quaternion.identity);    //Health Refill
            //Instantiate(_powerUps[5], postSpawn, Quaternion.identity);    //Fireworks
            //Instantiate(_powerUps[6], postSpawn, Quaternion.identity);    //Ammo Drain
            yield return new WaitForSeconds(Random.Range(3, 8));
        }
    }

    public void HealthOdds(float f)
    {
        _healthOdds = f;
    }

    public void AmmoOdds(float f)
    {
        _ammoOdds = f;
    }

    IEnumerator WaveChange()
    {
        //Debug.Log("Inside WaveChange CoRoutine");
        yield return new WaitForSeconds(5.0f);

        while (_waveChange)
        {
            //Debug.Log("Inside WaveChange");
            _enemiesLeft = _eneimesToStartWave + (_currentWaves * _additionalEnemiesPerWave);
            _totalEnemiesSpawn = _enemiesLeft;
            _currentWaves++;
            _uI.EnemiesLeft(_enemiesLeft);
            _uI.CurrentWave(_currentWaves);
            _enemiesSpawned = 0;
            _waveChange = false;
            _waveComplete = false;
        }


        yield return new WaitForSeconds(1.0f);
    }

    void ChangingWave()
    {
        if (_enemiesLeft == 0)
        {

            _waveChange = true;
            //Debug.Log("WaveChange ChangingWave: " + _waveChange);

            StartCoroutine(WaveChange());
            StopCoroutine(WaveChange());

        }
    }

    public void EnemiesLeft()
    {
        _enemiesLeft--;
        if (_enemiesLeft >= 0)
        {
            
            DisplayEnemyLeft();

        }
        /*
        else
        {
            if (!_spawnBoss)
            {
                Debug.Log("Inside Inverse of Spawn boss");
                //ChangingWave();
                _waveChange = true;
                // ChangingWave();
            }
        }
        */
    }

    void DisplayEnemyLeft()
    {
        _uI.EnemiesLeft(_enemiesLeft);
    }


    void EnemiesSpawned()
    {
        if (!_waveComplete)
        {
            
            _totalEnemiesSpawn--;
            Debug.Log("Total Enemies Left: " + _totalEnemiesSpawn);

            if (_totalEnemiesSpawn > 0)
            {
                
                _enemiesSpawned++;
                //Debug.Log("Enemies left: " + _totalEnemiesSpawn + " while Total at: " + _enemiesSpawned);
            }
            else
            {
                if (_currentWaves < _totalEnemyWave)
                {
                    //Debug.Log("WaveChange Inside EnemiesSpawned: " + _waveChange);
                    _waveComplete = true;
                    //_waveChange = true;
                    //_stopSpawning = true;
                }
                else
                {
                    if (!_bossSpawned)
                    {
                        _spawnBoss = true;
                        FinalBoss();
                    }
                }
                        
            }
        }
        /*
        else
        {
            if(_enemiesLeft < 1)
            {
                _waveComplete = true;
                //Debug.Log("Changing Waves inside Enemies Spawned");
                //ChangingWave();
            }
        }
        */
    }

    void FinalBoss()
    {
        Instantiate(_theBoss, _theBoss.transform.position, Quaternion.identity);
        _bossSpawned = true;
    }





}
