using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{

    //Private Variables
    enum PowerType { tripleShot = 0, speed = 1, shield = 2 }
    private int _selectedType;

    //Serialize Fields
    [SerializeField]
    private float _powerUpSpeed = 3.0f;

    [SerializeField]
    private AudioClip _clipPower;


    [SerializeField]
    private PowerType _powerUpType;
    
    
    

    // Start is called before the first frame update
    void Start()
    {
        _selectedType = (int)_powerUpType;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * Time.deltaTime * _powerUpSpeed);
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
                    default:
                        break;
                }
            }
            Destroy(this.gameObject);

        }
    }



}
