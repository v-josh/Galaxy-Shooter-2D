﻿using System.Collections;
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
    private int _shieldHealth = 3;

    [Header("Engine Fire")]
    [SerializeField]
    private List<GameObject> _engineFire;


    /*
    [Header("Sounds & Audio")]
    [SerializeField]
    private AudioClip _laserSound;

    [SerializeField]
    private AudioClip _explosionSound;
    */

    //Private Variables
    private float _canFire = -1f;
    private SpawnManager _spwScr;
    private bool _shieldActive = false;
    private UIManager _uiManager;
    private Text _gameOverText;

    //private AudioSource _sourceLaser;
    private AudioSource _sourcePlayer;
    

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

        /*
        if(_laser)
        {
            _sourceLaser = _laser.GetComponent<AudioSource>();
        }
        */

        _sourcePlayer = GetComponent<AudioSource>();

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
        transform.Translate(direction * Time.deltaTime * _playerSpeed);

        
    }

    void Fire()
    {

        _canFire = Time.time + _fireRate;   //_canFire is now the current Time + cooldown time
        //Debug.Log("Time.time is now at: " + Time.time + " and _canFire is now at: " + _canFire);
        if (_isTripleShotActive)
        {
            Instantiate(_tripleShot, transform.position, Quaternion.identity);

        }
        else
        {
            Instantiate(_laser, new Vector3(transform.position.x, transform.position.y + 1.05f, transform.position.z), Quaternion.identity);
        }

        //PlayAudioClip(_laserSound);

        //_sourceLaser.Play();

        
    }

    public void Damage()
    {
        if(_shieldActive)
        {
            _shieldActive = false;
            _shieldChild.SetActive(false);
            return;
        }

        _playerLives--;
        _uiManager.UpdateLives(_playerLives);

        //Debug.Log("Player's Lives is now at: " + _playerLives);

        if(_playerLives <= 0)
        {
            //PlayAudioClip(_explosionSound);

            _spwScr.OnPlayerDeath();
            Destroy(this.gameObject);
        }
        else
        {
            int engineNumber = Random.Range(0, _engineFire.Count);
            //Debug.Log("EngineFire.Count is: " + _engineFire.Count + " and Random Number is: " + engineNumber);
            _engineFire[engineNumber].SetActive(true);
            _engineFire.RemoveAt(engineNumber);

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
        _shieldActive = true;
        _shieldChild.SetActive(true);
    }

    public void AddToScore(int playerScore)
    {
        //PlayAudioClip(_explosionSound);
        _scoreTotal += playerScore;
        _uiManager.UpdateScore(_scoreTotal);
    }

    /*
    void PlayAudioClip(AudioClip ac)
    {
        _sourcePlayer.clip = ac;
        _sourcePlayer.Play();
    }
    */

}
