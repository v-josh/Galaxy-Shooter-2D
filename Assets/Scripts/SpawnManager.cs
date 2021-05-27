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
    private int _maxEnemySpawn;
    private bool _waveComplete = false;

    private int _enemiesSpawned = 0;
    private bool _spawnBoss = false;
    private bool _bossSpawned = false;

    


    // Start is called before the first frame update
    void Start()
    {
        _enemiesLeft = _eneimesToStartWave;
        _maxEnemySpawn = _enemiesLeft;



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

    public void OnPlayerActive()
    {
        _stopSpawning = false;
    }

    IEnumerator SpawnEnemyRoutine()
    {
        while (!_stopSpawning)
        {
            //=========================================================
            //     Picking Which Enemy to Spawn
            //=========================================================

            int valueRandom = Random.Range(0, _enemyPrefab.Length);     //Generate a value between 0 and the Enemy Prefab Array's length
            while(_enemySpin)                                           //Check to see if the value falls between the probabilty to summon the enemy
            {
                float enemyRandom = Random.value;                       //Generate a float between 0 and 1
                float enemyProb = _enemyProbability[valueRandom] * _currentWaves;   //Take the enemy's probabilty and multiply it by the current Wave's value

                //If the random value generated is greater than the probability,
                if(enemyRandom > (1f - enemyProb) )
                {
                    //Then set the enemy spin bool to false to break the loop, thus accepting the enemy chosen
                    _enemySpin = false;
                }
                else
                {
                    //Else, generate another random number again to see which enemy to choose
                    valueRandom = Random.Range(0, _enemyPrefab.Length);
                }
            }

            //=========================================================
            //     Choosing Where to Spawn the Enemy
            //=========================================================

            if (!_waveComplete && _enemiesSpawned < _maxEnemySpawn)
            {
                Enemy eScript = _enemyPrefab[valueRandom].GetComponent<Enemy>();    //Get the Enemy Script from the gameObject that's going to be spawn
                int movementNumber = eScript.TheMovement();                         //Determine if the enemy is moving "side to side" or "up to down"

                //=========================================================
                //     Spawn the Enemy
                //=========================================================

                if(movementNumber >= 0)
                {
                    _enemiesSpawned++;
                    //Normal Enemy, so Spawn above screen
                    Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7f, 0f);
                    Instantiate(_enemyPrefab[valueRandom], posToSpawn, Quaternion.identity, _enemyContainer.transform);
                }
                else if (movementNumber == -1)
                {
                    _enemiesSpawned++;
                    //This Enemy is moving from left to right, so spawn this enemy on the left of the screen
                    Vector3 posToSpawn = new Vector3(-12f, Random.Range(-4f, 4f), 0f);
                    Instantiate(_enemyPrefab[valueRandom], posToSpawn, Quaternion.identity, _enemyContainer.transform);
                }
                else if (movementNumber == -2)
                {
                    _enemiesSpawned++;
                    //This Enemy is moving from right to left, so spawn this enemy on the right of the screen
                    Vector3 posToSpawn = new Vector3(12f, Random.Range(-4f, 4f), 0f);
                    Instantiate(_enemyPrefab[valueRandom], posToSpawn, Quaternion.identity, _enemyContainer.transform);
                }

                _enemySpin = true;

            }

            
            yield return new WaitForSeconds(_enemySpawnTime);
        }
        
    }

    IEnumerator WaveChange()
    {
        yield return new WaitForSeconds(5.0f);

        while (_waveComplete)
        {
            _enemiesLeft = _eneimesToStartWave + (_currentWaves * _additionalEnemiesPerWave);
            _maxEnemySpawn = _enemiesLeft;
            _currentWaves++;
            _uI.EnemiesLeft(_enemiesLeft);
            _uI.CurrentWave(_currentWaves);
            _enemiesSpawned = 0;
            _waveChange = false;
            _waveComplete = false;
        }

        yield return new WaitForSeconds(1.0f);
    }

    public void EnemiesLeft()
    {
        _enemiesLeft--;
        DisplayEnemyLeft();
        
        if(_enemiesLeft <= 0)
        {

            if (_currentWaves < _totalEnemyWave)
            {
                _waveComplete = true;
                StartCoroutine(WaveChange());
            }
            else
            {
                FinalBoss();
            }
        }
        
    }

    void DisplayEnemyLeft()
    {
        _uI.EnemiesLeft(_enemiesLeft);
    }


    void FinalBoss()
    {
        Instantiate(_theBoss, _theBoss.transform.position, Quaternion.identity);
        _bossSpawned = true;
    }

    /// 
    /// POWER UPS & PLAYER RELATED METHODS
    /// 

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

                        if (probValue > 1f)
                        {
                            probValue = 1f;
                        }
                        else if (probValue < 0f)
                        {
                            probValue = _powerUpProbability[_theRandomNumber];
                        }
                        break;
                    case 4:
                        probValue += _healthOdds;

                        if (probValue > 1f)
                        {
                            probValue = 1f;
                        }
                        else if (probValue < 0f)
                        {
                            probValue = _powerUpProbability[_theRandomNumber];
                        }

                        break;
                    default:
                        break;
                }

                if (randomValue > (1f - probValue))
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
            //Instantiate(_powerUps[7], postSpawn, Quaternion.identity);    //Rockets
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


}
