using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    //Seralized Private Variables
    [SerializeField]
    private float _playerSpeed = 2.0f;

    [SerializeField]
    private int _playerHealth = 100;

    [SerializeField]
    private int _playerLives = 3;

    [SerializeField]
    private int _scoreTotal = 0;

    [SerializeField]
    private float _fireRate = 0.5f;

    [SerializeField]
    private GameObject _laser;

    [SerializeField]
    private GameObject _spwMan;

    [SerializeField]
    private GameObject _managerUI;

    [SerializeField]
    private GameObject _explosion;

    [SerializeField]
    private GameObject _playerThrust;




    [Header("Triple Shot Power Up")]
    [SerializeField]
    private float _tripleShotCooldown = 5.0f;


    [SerializeField]
    private GameObject _tripleShot;

    [SerializeField]
    private bool _isTripleShotActive = false;

    [Header("Speed Boost")]
    [SerializeField]
    private float _speedMultiplier = 1.5f;

    [SerializeField]
    private float _speedLast = 5.0f;

    [SerializeField]
    private bool _speedBoostActive = false;

    [Header("Player Shield")]
    [SerializeField]
    private GameObject _shieldChild;

    [SerializeField]
    private int _shieldHealth = 2;

    [Header("Engine Fire")]
    [SerializeField]
    private List<GameObject> _engineFire;

    [Header("Thrusters Specs")]
    [SerializeField]
    private float _accelerationRate = 1.0f;

    [SerializeField]
    private float _maxAcceleration = 5.0f;

    [Header("Ammo Refill and Reload")]
    [SerializeField]
    private AudioClip _ammoEmpty;

    [Header("Fireworks: Secondary Weapon")]
    [SerializeField]
    private GameObject _weaponFireworks;

    [Header("Final Boss")]
    [SerializeField]
    private Vector2 _finalPosition;




    //Private Variables
    private float _canFire = -1f;
    private SpawnManager _spwScr;
    private bool _shieldActive = false;
    private UIManager _uiManager;
    private Text _gameOverText;
    private AudioSource _sourcePlayer;
    private int _ammoCount = 15;
    private int _maxHealth;


    //Thrusters Variables
    private float _initialAcceleration;
    private bool _thrustCoolDown = false;

    //Shield Variables
    private Renderer _rendShield;
    private Color _colorShield;

    //Random Engine
    private int _firstEngine;
    private int _secondEngine;

    //Fireworks show
    private bool _startFireworks = false;

    //Pickup Collection
    private bool _collectionCooldown = false;
    private float _collectionInitialAcceleration = 1.0f;
    private float _maxCollectionAcceleration = 4.0f;
    private float _colAcceleration = 1.0f;

    private bool _spawningBoss = false;
    private bool _playerStillAlive = true;


    void Start()
    {

        _maxHealth = _playerHealth;

        if(_spwMan != null)
        {
            _spwScr = _spwMan.GetComponent<SpawnManager>();
        }
        else
        {
            _spwMan = GameObject.FindWithTag("Spawn Manager");
            _spwScr = _spwMan.GetComponent<SpawnManager>();
        }

        if(_spwScr == null)
        {
            Debug.Log("Cannot Find Spanw Manger script!");
        }

        if(_shieldChild == null)
        {
            _shieldChild = this.transform.GetChild(0).gameObject;
            _shieldChild.SetActive(false);
        }
        else
        {
            _shieldChild.SetActive(false);
        }

        if(!_managerUI)
        {
            _uiManager = GameObject.FindGameObjectWithTag("UI").GetComponent<UIManager>();

        }
        else
        {
            _uiManager = _managerUI.GetComponent<UIManager>();
        }

        _sourcePlayer = GetComponent<AudioSource>();
        _initialAcceleration = _accelerationRate;
        _collectionInitialAcceleration = _colAcceleration;

        _rendShield = _shieldChild.GetComponent<Renderer>();
        if (_rendShield == null)
        {
            Debug.Log("Renderer for the shield is null");
        }
        else
        {
            _colorShield = _rendShield.material.color;

        }
        //Choosing Engine at random
        _firstEngine = Random.Range(0, _engineFire.Count);
        if(_firstEngine == 0)
        {
            _secondEngine = 1;
        }
        else
        {
            _secondEngine = 0;
        }

        for(int i = 0; i < _engineFire.Count; i++)
        {
            _engineFire[i].SetActive(false);
        }

    }






    // Update is called once per frame
    void Update()
    {
        //Player Movement

        if (!_spawningBoss)
        {
            if (_playerStillAlive)
            {
                CalculateMovement();

                CollectionPickup();

                //If the player press the Spacebar AND has surprassed the cooldown timer
                if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
                {
                    if (!_startFireworks)
                    {
                        Fire();
                    }
                }
            }
        }
        else
        {
            MovePlayerForBoss();
        }
    }

    void CalculateMovement()
    {


        //Variables
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalAxis, verticalAxis, 0);

        ThrustersActivate();

        //Vertical Clamp
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0f), 0f);

        //Horizontal Clamp
        if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, transform.position.z);
        }

        //Move the player
        transform.Translate(direction * Time.deltaTime * _playerSpeed * _accelerationRate);
    }

    void ThrustersActivate()
    {
        ///START OF THRUSTERS IMPLEMENTATION
        if (Input.GetKey(KeyCode.LeftShift) && !_thrustCoolDown)
        {
            if (_accelerationRate < _maxAcceleration)
            {
                _accelerationRate += (_accelerationRate * Time.deltaTime);
                _uiManager.ThrustText("Using");
            }
            else
            {
                _thrustCoolDown = true;
            }
        }
        //Helps decrease the thrusters
        else if (Input.GetKeyUp(KeyCode.LeftShift) || _accelerationRate > _initialAcceleration)
        {
            _accelerationRate -= (_initialAcceleration * Time.deltaTime);
            if (!_thrustCoolDown)
            {
                _uiManager.ThrustText("Decreasing");
            }
            else
            {
                _uiManager.ThrustText("COOLING OFF");
            }
        }

        //Reset the acceleration Rate to a whole number
        if (_accelerationRate > _maxAcceleration)
        {
            _accelerationRate = _maxAcceleration;
        }
        else if (_accelerationRate < _initialAcceleration)
        {
            _accelerationRate = _initialAcceleration;
            _uiManager.ThrustText("Ready");
            _thrustCoolDown = false;
        }

        _uiManager.ThrustUI(_accelerationRate); //Change the Thruster bar
        ///END THRUSTER IMPLEMENTATION
    }

    void Fire()
    {

        _canFire = Time.time + _fireRate;   //_canFire is now the current Time + cooldown time
        if (_isTripleShotActive)
        {
            Instantiate(_tripleShot, transform.position, Quaternion.identity);

        }
        else
        {
            if (_ammoCount > 0)
            {
                _ammoCount--;
                float theOdds = 1f - (_ammoCount / 15f) ;
                _spwScr.AmmoOdds(theOdds);
                Instantiate(_laser, new Vector3(transform.position.x, transform.position.y + 1.05f, transform.position.z), Quaternion.identity);
                _uiManager.AmmoText(_ammoCount);
            }
            else
            {
                _sourcePlayer.PlayOneShot(_ammoEmpty);
            }
        }        
    }

    public void Damage(int damage)
    {

        _uiManager.CameraShake();

        if (_shieldActive)
        {
            ShieldHealth();
        }
        else
        {
            _playerHealth -= damage;
            _uiManager.PlayerHealth(_playerHealth);
            _spwScr.HealthOdds(0.3f);
            if (_playerHealth >= 1)
            {
                EngineOnFire(true);
            }
        }




        if(_playerHealth < 1)
        {
            _playerLives--;
            if (_playerLives > 0)
            {
                
                _uiManager.UpdateLives(_playerLives);
                _playerStillAlive = false;
                _engineFire[_firstEngine].SetActive(false);
                _engineFire[_secondEngine].SetActive(false);
                StartCoroutine(RestartPlayer());
            }
            else
            {
                //StopCoroutine(RestartPlayer());
                _playerStillAlive = false;
                _uiManager.UpdateLives(_playerLives);
                _spwScr.OnPlayerDeath();
                //_uiManager.UpdateLives(_playerLives);
                Destroy(this.gameObject);
            }
        }
    }

    void RestartLives()
    {
        _playerHealth = _maxHealth;
        _ammoCount = 15;
        _uiManager.PlayerHealth(_playerHealth);
        _uiManager.AmmoText(_ammoCount);


    }


    IEnumerator RestartPlayer()
    {
        while (!_playerStillAlive)
        {
            Instantiate(_explosion, transform.position, Quaternion.identity);

            //this.gameObject.SetActive(false);

            BoxCollider2D boxPlayer = this.gameObject.GetComponent<BoxCollider2D>();
            boxPlayer.enabled = false;

            SpriteRenderer playerSR = this.gameObject.GetComponent<SpriteRenderer>();
            playerSR.enabled = false;

            SpriteRenderer thrustSR = _playerThrust.gameObject.GetComponent<SpriteRenderer>();
            thrustSR.enabled = false;
            yield return new WaitForSeconds(1f);

            //MovePlayerForBoss();
            this.transform.position = _finalPosition;
            yield return new WaitForSeconds(1f);
            //this.gameObject.SetActive(true);
            RestartLives();
            playerSR.enabled = true;
            thrustSR.enabled = true;
            
            yield return new WaitForSeconds(1f);

            boxPlayer.enabled = true;
            _playerStillAlive = true;
        }
    }




    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotCooldown());
    }

    IEnumerator TripleShotCooldown()
    {
        yield return new WaitForSeconds(_tripleShotCooldown);
        _isTripleShotActive = false;
    }

    public void SpeedPowerActive()
    {
        _speedBoostActive = true;
        StartCoroutine(SpeedBoost());
    }

    IEnumerator SpeedBoost()
    {
        while (_speedBoostActive)
        {
            _playerSpeed *= _speedMultiplier;               //Multiply Current speed with the speed mulitplier
            yield return new WaitForSeconds(_speedLast);    //Wait for X seconds (For instance, 5 seconds)
            _playerSpeed /= _speedMultiplier;               //Divide Current speed with the speed multiplier
            _speedBoostActive = false;                      //Set the speed Boost Active bool to false to turn it off
        }
    }

    public void ActiveShield()
    {
        //RESET SHIELD
        _colorShield.a = 1f;
        _rendShield.material.color = _colorShield;
        _shieldHealth = 2;

        //ACTIVATE SHIELD
        _shieldActive = true;
        _shieldChild.SetActive(true);


    }

    public void AddToScore(int playerScore)
    {
        _scoreTotal += playerScore;
        _uiManager.UpdateScore(_scoreTotal);
        //_spwScr.EnemiesLeft();
    }

    /// START SHIELD HEALTH IMPLEMENTATION
    void ShieldHealth()
    {
        if (_shieldHealth > 0)
        {
            _shieldHealth--;
            _colorShield.a -= 0.33f;
            _rendShield.material.color = _colorShield;
            return;
        }
        else
        {
            _shieldChild.SetActive(false);
            _shieldActive = false;
            return;
        }
    }

    ///END SHIELD HEALTH IMPLEMENTATION
    
    public void AmmoRefill()
    {
        if(_ammoCount < 15)
        {
            _ammoCount = 15;
            _uiManager.AmmoText(_ammoCount);
            _spwScr.AmmoOdds(-15f);
        }
    }

    public void HealthRefill()
    {
        if(_playerHealth < _maxHealth)
        {
            _playerHealth++;
            //_uiManager.UpdateLives(_playerHealth);
            _uiManager.PlayerHealth(_playerHealth);
            EngineOnFire(false);
            _spwScr.HealthOdds(-0.3f);
        }
    }

    void EngineOnFire(bool b)
    {
        if(b)
        {

            /*
            if(_playerHealth == 2)
            {
                _engineFire[_firstEngine].SetActive(true);
            }
            else if(_playerHealth == 1)
            {
                _engineFire[_secondEngine].SetActive(true);
            }
            */

            //if(_playerHealth < )


            if(_playerHealth < 90 && _playerHealth > 40)
            {
                _engineFire[_firstEngine].SetActive(true);
            }
            else if(_playerHealth < 40 && _playerHealth > 0)
            {
                _engineFire[_secondEngine].SetActive(true);
            }


            
            /*
            // Initial Random Engine Choice Logic
            int engineNumber = Random.Range(0, _engineFire.Count);
            _engineFire[engineNumber].SetActive(true);
            _engineFire.RemoveAt(engineNumber);
            */
        }
        else
        {
            /*
            if(_playerHealth == 3)
            {
                _engineFire[_firstEngine].SetActive(false);
            }
            else if(_playerHealth == 2)
            {
                _engineFire[_secondEngine].SetActive(false);
            }
            */
            if (_playerHealth > 90)
            {
                _engineFire[_firstEngine].SetActive(false);
            }
            else if (_playerHealth > 40)
            {
                _engineFire[_secondEngine].SetActive(false);
            }

        }
    }

    public void Fireworks()
    {
        _startFireworks = true;
        StartCoroutine(TheFireworksShow());
        StartCoroutine(StopFireworks());
    }

    IEnumerator TheFireworksShow()
    {
        
        while(_startFireworks)
        {
            Instantiate(_weaponFireworks, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
            
        }
    }

    IEnumerator StopFireworks()
    {
        yield return new WaitForSeconds(5.0f);
        _startFireworks = false;
        StopCoroutine(TheFireworksShow());
    }

    void CollectionPickup()
    {

        if (Input.GetKey(KeyCode.C) && !_collectionCooldown)
        {
            if (_colAcceleration < _maxCollectionAcceleration)
            {
                _colAcceleration += (_colAcceleration * Time.deltaTime);
                _uiManager.CollectionText("Using");
            }
            else
            {
                _collectionCooldown = true;
            }
        }
        else if (Input.GetKeyUp(KeyCode.C) || _colAcceleration > _collectionInitialAcceleration)
        {
            _colAcceleration -= (_collectionInitialAcceleration * Time.deltaTime);
            if (!_collectionCooldown)
            {
                _uiManager.CollectionText("Releasing");
            }
            else
            {
                _uiManager.CollectionText("RECHARGING");
            }
        }

        if (_colAcceleration > _maxCollectionAcceleration)
        {
            _colAcceleration = _maxCollectionAcceleration;
        }
        else if (_colAcceleration < _collectionInitialAcceleration)
        {
            _colAcceleration = _collectionInitialAcceleration;
            _uiManager.CollectionText("Ready");
            _collectionCooldown = false;
        }

        _uiManager.CollectionUI(_colAcceleration);

    }

    public bool CollectingPickups()
    {
        return _collectionCooldown;
    }

    public void SubtractEnemy()
    {
        _spwScr.EnemiesLeft();
    }

    public void BossSpawning(bool x)
    {
        _spawningBoss = x;
        _playerStillAlive = !x;
    }

    void MovePlayerForBoss()
    {
        transform.position = Vector2.MoveTowards(transform.position, _finalPosition, _playerSpeed * Time.deltaTime);
    }

    public bool IsPlayerAlive()
    {
        return _playerStillAlive;
    }

}
