using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    //Seralized Private Variables
    [SerializeField]
    private int _playerSpeed = 2;

    [SerializeField]
    private int _playerLives = 3;

    [SerializeField]
    private float _fireRate = 0.5f;

    [SerializeField]
    private GameObject _laser;

    [SerializeField]
    private GameObject _spwMan;

    //Private Variables
    private float _canFire = -1f;
    private SpawnManager _spwScr;

    void Start()
    {
        //transform.position = new Vector3(0, 0, 0);

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
        Instantiate(_laser, new Vector3(transform.position.x, transform.position.y + 0.8f, transform.position.z), Quaternion.identity);
    }

    public void Damage()
    {
        _playerLives--;

        //Debug.Log("Player's Lives is now at: " + _playerLives);

        if(_playerLives <= 0)
        {
            _spwScr.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }
}
