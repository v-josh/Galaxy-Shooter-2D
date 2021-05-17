using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    enum EnemyMovement { None = 0, Default = 1, ZigZag = 2, Rotate = 3, Side = 4, Ram = 5, Mine = 6 };
    private int _movementChoice;

    [SerializeField]
    private EnemyMovement _enemyMovement;

    [SerializeField]
    private float _enemySpeed = 4.0f;

    [SerializeField]
    private int _enemyScore = 10;

    [SerializeField]
    private GameObject _enemyLaser;

    [SerializeField]
    private GameObject _enemySingleLaser;

    [SerializeField]
    private GameObject _enemyShield;



    [SerializeField]
    private bool _isRotation = false;

    [SerializeField]
    private bool _avoidFire = false;



    //Private Variables
    private GameObject _thePlayer;
    private Player _playerScript;
    private Animator _enemyAnim;
    private AudioSource _sourceEnemy;

    private float _fireRate = 3.0f;
    private float _canFire = -1;
    private bool _activateShield = false;

    private float _powerFire = -1;



    //Side to Side
    private bool _selectedSide = false;
    private string _theSide = "";
    private int _sideNumber = 0;

    //Fire Upside
    private bool _fireUp = false;
    private float _upFire = -1f;

    //Mine Weapon
    private bool _mineActivate = false;
    

    // Start is called before the first frame update
    void Start()
    {
        _movementChoice = (int)_enemyMovement;
        
        if (!_thePlayer)
        {
            _thePlayer = GameObject.FindGameObjectWithTag("Player");
            _playerScript = _thePlayer.GetComponent<Player>();
        }
        else
        {
            _playerScript = _thePlayer.GetComponent<Player>();
        }
        

        _enemyAnim = GetComponent<Animator>();
        _sourceEnemy = GetComponent<AudioSource>();

        if (_enemyShield == null)
        {
            _enemyShield = this.transform.GetChild(0).gameObject;
            //_enemyShield.SetActive(false);
        }



        if(Random.value > 0.75f)
        {
            _activateShield = true;
            _enemyShield.SetActive(true);
        }
        else
        {
            _activateShield = false;
            _enemyShield.SetActive(false);
        }

        Physics2D.queriesStartInColliders = false;

    }


    private void FixedUpdate()
    {
        RaycastHit2D hit_down = Physics2D.Raycast(transform.position, Vector2.down, 10f);
        RaycastHit2D hit_up = Physics2D.Raycast(transform.position, Vector2.up, 10f);

        if (hit_down.collider != null && hit_down.collider.tag ==  "Powerup")
        {
            if(Time.time > _powerFire)
            {
                EnemyFire(false);
            }

        }

        if (hit_up.collider != null && hit_up.collider.tag == "Player")
        {
            FireUp();

        }


        if (_avoidFire)
        {
            Collider2D fireToAvoid = Physics2D.OverlapCircle(transform.position, 2f);

            if (fireToAvoid != null && fireToAvoid.tag == "Laser")
            {
                AvoidFire(fireToAvoid.gameObject);
            }
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1.5f);

    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if(Time.time > _canFire)
        {
            EnemyFire(true);
        }

    }


    void EnemyFire(bool _fromUpdate)
    {
        if (_fromUpdate)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;

            /*
            GameObject enemyLaser = Instantiate(_enemyLaser, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
            */
            CreateLasers();

        }
        else
        {
            _fireRate = Random.Range(0f, 2f);
            _powerFire = Time.time + _fireRate;

            /*
            GameObject enemyLaser = Instantiate(_enemyLaser, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
            */
            CreateLasers();

        }
    }

    void CreateLasers()
    {
        GameObject enemyLaser = null;
        if (!_fireUp)
        {
            enemyLaser = Instantiate(_enemyLaser, transform.position, Quaternion.identity);

        }
        else
        {
            enemyLaser = Instantiate(_enemySingleLaser, transform.position, Quaternion.identity);
        }

        if (!_mineActivate)
        {
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }
    }

    void FireUp()
    {
        if (Time.time > _upFire)
        {
            float ranUp = Random.Range(0f, 1f);
            _upFire = Time.time + ranUp;

            _fireUp = true;
            CreateLasers();
            _fireUp = false;
        }
        /*
        GameObject enemyLaser = Instantiate(_enemySingleLaser, transform.position, Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }
        */
    }



    void CalculateMovement()
    {
        switch(_movementChoice)
        {
            case 0: //No Movement
                break;
            case 1:   //Default movement
                DefaultMovement();
                break;
            case 2:   //Zig Zag
                break;
            case 3:     //Rotate
                _isRotation = true;
                RotateMovement();
                break;
            case 4:     //Side to Side
                SideMovement();
                break;
            case 5:     //Ram
                Ramming();
                break;
            case 6:     //Shooting Mines
                _mineActivate = true;
                SideMovement();
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {

            Player scrPlayer = other.GetComponent<Player>();
            if (scrPlayer != null)
            {
                scrPlayer.Damage();
            }

            //Combining the lines from above, but no null check
            //other.transform.GetComponent<Player>().Damage();

            if (!_activateShield)
            {
                _enemyAnim.SetTrigger("OnEnemyDeath");
                _enemySpeed = 0f;
                _sourceEnemy.Play();
                Destroy(this.gameObject, 2.3f);
            }
            else
            {
                _activateShield = false;
                _enemyShield.SetActive(false);
            }
        }
        else if (other.tag == "Laser")
        {
            if (!_activateShield)
            {
                Destroy(other.gameObject);
                _playerScript.AddToScore(_enemyScore);

                _enemySpeed = 0f;
                _enemyAnim.SetTrigger("OnEnemyDeath");
                _sourceEnemy.Play();
                Destroy(GetComponent<Collider2D>());
                Destroy(this.gameObject, 2.3f);
            }
            else
            {
                Destroy(other.gameObject);
                _activateShield = false;
                _enemyShield.SetActive(false);
            }
        }
    }

    void DefaultMovement()
    {
       transform.rotation = Quaternion.identity;
        
        transform.Translate(Vector3.down * Time.deltaTime * _enemySpeed);

        if (transform.position.y <= -5f && !_isRotation)
        {
            transform.position = new Vector3(Random.Range(-10, 10), 7f, 0f);
        }
    }

    void RotateMovement()
    {
        //DefaultMovement();


        //_xAngle = 20f * Mathf.Cos(_angle) * Time.deltaTime;
        //_yAngle = 20f * Mathf.Sin(_angle) * Time.deltaTime;
        //transform.position = new Vector3(_xAngle + transform.parent.position.x, _yAngle + transform.parent.position.y, 0f);
        //transform.position = new Vector2(_xAngle, _yAngle) * _angle * Time.deltaTime;
        //Debug.Log("Y is: " + y);
        //_angle += 20  * Time.deltaTime * Mathf.Rad2Deg;
        
        
        //_angle += (_enemySpeed / (20f * 2f * Mathf.PI)) * Time.deltaTime;
        //transform.position = new Vector3(_xAngle, _yAngle, 0f);

        //transform.Rotate(new Vector3(_xAngle, _yAngle, 0f), _angle);

        transform.RotateAround(transform.parent.position, Vector3.forward, Time.deltaTime * 360f);



    }

    void SideMovement()
    {
        
        if(!_selectedSide)
        {
            if(Random.value < 0.5f)
            {
                _theSide = "left";
                _selectedSide = true;
            }
            else
            {
                _theSide = "right";
                _selectedSide = true;
            }
        }
        else
        {

            if(_theSide == "left")
            {
                SideLeft();

            }
            else
            {
                SideRight();
            }
        }
        
    }

    void SideLeft()
    {
        _sideNumber = -1;
        transform.rotation = Quaternion.identity;
        transform.Translate(Vector3.left * Time.deltaTime * _enemySpeed);

        if (transform.position.x < -12)
        {
            transform.position = new Vector3(12f, Random.Range(-4f, 4f), 0f);
        }
    }

    void SideRight()
    {
        _sideNumber = -2;
        transform.rotation = Quaternion.identity;
        transform.Translate(Vector3.right * Time.deltaTime * _enemySpeed);

        if (transform.position.x > 12)
        {
            transform.position = new Vector3(-12f, Random.Range(-4f, 4f), 0f);
        }
    }

    public int TheMovement()
    {
        int numberGet = 0;
        if (_sideNumber < 0)
        {
            numberGet = _movementChoice;
        }
        else
        {
            numberGet = _sideNumber;
        }

        return numberGet;
    }

    void Ramming()
    {
        DefaultMovement();

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            if (Vector3.Distance(transform.position, _thePlayer.transform.position) < 3f)
            {
                //transform.Translate()
                transform.position = Vector3.MoveTowards(transform.position, _thePlayer.transform.position, _enemySpeed * Time.deltaTime);
            }

        }
    }

    void AvoidFire(GameObject avoidThis)
    {
        if (Vector3.Distance(transform.position, avoidThis.transform.position) < 3f)
        {
            transform.position = Vector3.MoveTowards(transform.position, avoidThis.transform.position, -3f * _enemySpeed * Time.deltaTime);
        }
    }

}

