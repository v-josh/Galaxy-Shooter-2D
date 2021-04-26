using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{

    [SerializeField]
    private float _laserSpeed = 8.0f;

    [SerializeField]
    private bool _isEnemyLaser = false;

    void Update()
    {
        if(!_isEnemyLaser)
        {
            PlayerLaser();
        }
        else
        {
            EnemyLaser();
        }
    }

    void PlayerLaser()
    {
        transform.Translate(Vector3.up * Time.deltaTime * _laserSpeed);
        if (transform.position.y >= 8)
        {

            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }

    void EnemyLaser()
    {
        transform.Translate(Vector3.down * Time.deltaTime * _laserSpeed);
        if (transform.position.y <= -8)
        {

            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && _isEnemyLaser)
        {
            Player ps = collision.GetComponent<Player>();
            ps.Damage();
        }
    }

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
    }


}
