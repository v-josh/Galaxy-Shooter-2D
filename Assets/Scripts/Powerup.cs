using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{


    //Serialize Fields
    [SerializeField]
    private float _powerUpSpeed = 3.0f;


    // Start is called before the first frame update
    void Start()
    {
        
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
            Player ps = collision.gameObject.GetComponent < Player> ();
            if(ps != null)
            {
                ps.TripleShotActive();
            }
            Destroy(this.gameObject);

        }
    }



}
