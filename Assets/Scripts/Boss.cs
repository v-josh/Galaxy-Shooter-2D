using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{

    [Header("Boss Stats")]
    [SerializeField]
    private int _bossHealth = 100;

    [SerializeField]
    private float _bossSpeed;

    [SerializeField]
    private float _delayFire = 2f;

    [SerializeField]
    private Vector2 _initalSpot;

    [SerializeField]
    private GameObject _explosion;


    [Header("Boss Fire Location")]
    [SerializeField]
    private GameObject _leftFireLoc;

    [SerializeField]
    private GameObject _middleFireLoc;

    [SerializeField]
    private GameObject _rightFireLoc;

    [Header("Boss Ammo")]
    [SerializeField]
    private GameObject _mainWeapon;

    /*
    [SerializeField]
    private GameObject _mines;

    [SerializeField]
    private GameObject _conLasers;
    */
    [Header("Boss Laser")]

    [SerializeField]
    private GameObject _bossLeftLaser;

    [SerializeField]
    private GameObject _bossRightLaser;

    [SerializeField]
    private ParticleSystem _bossParticleLeft;

    [SerializeField]
    private ParticleSystem _bossParticleRight;


    [SerializeField]
    private float _bigLaserDelay = 10f;

    //Private Variables
    private float _canFire = -1;
    private float _laserFire = 10;


    private bool _startLaser = false;
    private int _laserChosen = 0;

    private Player _pS;
    private UIManager _uI;

    private bool _canStart = false;
    private bool _stillAlive = true;
    private MassiveLaser _massL;


    // Start is called before the first frame update
    void Start()
    {
        if(_bossLeftLaser != null)
        {
            _bossLeftLaser.SetActive(false);
        }

        if(_bossRightLaser != null)
        {
            _bossRightLaser.SetActive(false);
        }

        if(_bossParticleLeft != null)
        {
            _bossParticleLeft.Stop();
        }

        if(_bossParticleRight != null)
        {
            _bossParticleRight.Stop();
        }


        _pS = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _uI = GameObject.FindGameObjectWithTag("UI").GetComponent<UIManager>();

        _pS.BossSpawning(true);

    }

    // Update is called once per frame
    void Update()
    {
        if (_stillAlive)
        {
            if (!_canStart)
            {
                transform.position = Vector2.MoveTowards(transform.position, _initalSpot, _bossSpeed * 2f * Time.deltaTime);
                if (Vector2.Distance(transform.position, _initalSpot) < 0.5f)
                {
                    _canFire += Time.time;
                    _laserFire += Time.time;
                    _canStart = true;
                    _uI.SetBossVisible(true);
                    _pS.BossSpawning(false);
                }
            }
            else
            {

                transform.position = new Vector2(Mathf.Lerp(-5, 5, Mathf.PingPong(Time.time * _bossSpeed, 1)), Mathf.PingPong(Time.time, 3));
                /*
                if (_initialStart)
                {
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(Mathf.Lerp(-5, 5, Mathf.PingPong(Time.time * _bossSpeed, 1)), Mathf.PingPong(Time.time, 3)), _bossSpeed * Time.deltaTime);
                    _initialStart = false;
                }
                else
                {
                    transform.position = new Vector2(Mathf.Lerp(-5, 5, Mathf.PingPong(Time.time * _bossSpeed, 1)), Mathf.PingPong(Time.time, 3));
                }
                */

                if (Time.time > _canFire)
                {
                    RegularFire();
                }

                if (Time.time > _laserFire)
                {
                    LaserFire();
                }

                if(_bossHealth <= 0)
                {
                    _stillAlive = false;
                    OnDeath();
                }
            }
        }

    }

    void LaserFire()
    {
        if (!_startLaser)
        {
            _laserChosen = Random.Range(0, 2);
            _startLaser = true;
            StartCoroutine(LaserSpawn());

        }
    }

    void RegularFire()
    {
        int locRandom = Random.Range(0, 3);
        Vector2 locPos = Vector2.zero;
        switch(locRandom)
        {
            case 0:
                locPos = _middleFireLoc.transform.position;
                break;
            case 1:
                locPos = _rightFireLoc.transform.position;
                break;
            case 2:
                locPos = _leftFireLoc.transform.position;
                break;
            default:
                break;
        }

        float _fireRandom = Random.Range(0f, _delayFire);
        _canFire = Time.time + _fireRandom;


        //GameObject enemyLaser = Instantiate(_enemyLaser, transform.position, Quaternion.identity);



        GameObject enemyLaser = Instantiate(_mainWeapon, locPos, Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }
        

    }


    IEnumerator LaserSpawn()
    {
        while (_startLaser == true)
        {
            ParticleSystem ps;
            ParticleSystem.MainModule mPart;
            switch(_laserChosen)
            {
                case 0:
                    _bossParticleLeft.Play();
                    ps = _bossParticleLeft.GetComponent<ParticleSystem>();
                    mPart  = ps.main;
                    break;
                case 1:
                    _bossParticleRight.Play();
                    ps = _bossParticleRight.GetComponent<ParticleSystem>();
                    mPart = ps.main;
                    break;
                case 2:
                    break;
                default:
                    break;
            }

            mPart.simulationSpeed += 2f;
            yield return new WaitForSeconds(5f);
            
            switch (_laserChosen)
            {
                case 0:
                    _bossParticleLeft.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    _bossLeftLaser.SetActive(true);
                    _massL = _bossLeftLaser.GetComponent<MassiveLaser>();
                    break;
                case 1:
                    _bossParticleRight.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    
                    _bossRightLaser.SetActive(true);

                    _massL = _bossRightLaser.GetComponent<MassiveLaser>();
                    break;
                case 2:
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(10f);
            switch (_laserChosen)
            {
                case 0:
                    _bossLeftLaser.SetActive(false);
                    break;
                case 1:
                    _bossRightLaser.SetActive(false);
                    break;
                case 2:
                    break;
                default:
                    break;
            }
            _laserFire = Time.time + _bigLaserDelay;
            _startLaser = false;

        }

    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Laser" && _stillAlive)
        {
            _pS.AddToScore(10);
            _bossHealth--;
            _uI.BossHealth(_bossHealth);
            Destroy(collision.gameObject);
        }
    }

    void OnDeath()
    {
        StopCoroutine(LaserSpawn());
        _massL.StillAlive(false);
        _startLaser = false;
        _pS.BossSpawning(true);

        StartCoroutine(BossExplosion());

    }

    IEnumerator BossExplosion()
    {
        Instantiate(_explosion, _leftFireLoc.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        Instantiate(_explosion, _rightFireLoc.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        Instantiate(_explosion, _middleFireLoc.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(2f);
        Instantiate(_explosion, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        Destroy(this.gameObject);
    }
}
