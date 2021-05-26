using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{

    //Private Variables
    enum PowerType { tripleShot = 0, speed = 1, shield = 2, ammo = 3, health = 4, fireworks = 5, drain = 6, rockets = 7 }
    private int _selectedType;

    //Serialize Fields
    [SerializeField]
    private float _fallSpeed = 3.0f;

    [SerializeField]
    private AudioClip _clipPower;


    [SerializeField]
    private PowerType _powerUpType;


    [SerializeField]
    private GameObject _thePlayer;
    private Player _pS;
    
    

    // Start is called before the first frame update
    void Start()
    {
        _selectedType = (int)_powerUpType;
        
        if(!_thePlayer)
        {
            _thePlayer = GameObject.FindGameObjectWithTag("Player");
            _pS = _thePlayer.GetComponent<Player>();
        }
        
    }




    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * Time.deltaTime * _fallSpeed);

        if(Input.GetKey(KeyCode.C) && !_pS.CollectingPickups() && _pS.IsPlayerAlive() )
        {
            transform.position = Vector2.MoveTowards(transform.position, _thePlayer.transform.position, _fallSpeed * Time.deltaTime);

        }


        if(transform.position.y <= -4.5f)
        {
            
            Destroy(this.gameObject);
        }

        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            AudioSource.PlayClipAtPoint(_clipPower, transform.position);
            Player ps = collision.gameObject.GetComponent < Player> ();
            if(ps != null)
            {
                switch(_selectedType)
                {
                    case 0: //Triple Shot
                        ps.TripleShotActive();
                        break;
                    case 1: //Speed
                        ps.SpeedPowerActive();
                        break;
                    case 2: //Shield
                        ps.ActiveShield();
                        break;
                    case 3: //Ammo Refill
                        ps.AmmoRefill();
                        break;
                    case 4: //Health Refill
                        ps.HealthRefill();
                        break;
                    case 5: //Fireworks (secondary weapon)
                        ps.Fireworks();
                        break;
                    case 6: //Ammo Drain
                        ps.AmmoDrain();
                        break;
                    case 7: //Rockets
                        break;
                    default:
                        break;
                }
            }
            Destroy(this.gameObject);

        }
        else if(collision.gameObject.tag == "EnemyLaser")
        {
            Destroy(this.gameObject);
        }
    }



}
