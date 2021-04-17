using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField]
    private float _enemySpeed = 4.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * Time.deltaTime * _enemySpeed);

        if(transform.position.y <= -5f)
        {
            transform.position = new Vector3(Random.Range(-10,10), 7f, 0f);
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {

            Player scrPlayer = other.GetComponent<Player>();
            if (scrPlayer != null)
            {
                scrPlayer.Damage();
            }

            //Combining the lines from above, but no null check
            //other.transform.GetComponent<Player>().Damage();

            Destroy(this.gameObject);
        }
        else if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }
    }
}
