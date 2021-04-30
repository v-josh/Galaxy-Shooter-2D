using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Player : MonoBehaviour
{

    //Seralized Private Variables
    [SerializeField]
    private float _playerSpeed = 2.0f;

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


    //Private Variables
    private float _canFire = -1f;
    private SpawnManager _spwScr;
    private bool _shieldActive = false;
    private UIManager _uiManager;
    private Text _gameOverText;
    private AudioSource _sourcePlayer;
    private int _ammoCount = 15;

    //Thrusters Variables
    private float _initialAcceleration;
    private bool _thrustCoolDown = false;

    //Shield Variables
    private Renderer _rendShield;
    private Color _colorShield;

    

    void Start()
    {

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

        _rendShield = _shieldChild.GetComponent<Renderer>();
        if (_rendShield == null)
        {
            Debug.Log("Renderer for the shield is null");
        }
        else
        {
            _colorShield = _rendShield.material.color;

        }

    }

    // Update is called once per frame
    void Update()
    {
        //Player Movement
        
        CalculateMovement();

        //If the player press the Spacebar AND has surprassed the cooldown timer
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            Fire();
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
                Instantiate(_laser, new Vector3(transform.position.x, transform.position.y + 1.05f, transform.position.z), Quaternion.identity);
                _uiManager.AmmoText(_ammoCount);
            }
            else
            {
                _sourcePlayer.PlayOneShot(_ammoEmpty);
            }
        }        
    }

    public void Damage()
    {
        if (_shieldActive)
        {
            ShieldHealth();
        }
        else
        {
            _playerLives--;
            _uiManager.UpdateLives(_playerLives);
            if (_playerLives >= 1)
            {
                int engineNumber = Random.Range(0, _engineFire.Count);
                _engineFire[engineNumber].SetActive(true);
                _engineFire.RemoveAt(engineNumber);
            }
        }

        if(_playerLives <= 0)
        {
            
            _spwScr.OnPlayerDeath();
            _uiManager.UpdateLives(_playerLives);
            Destroy(this.gameObject);
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
        }
    }


}
