using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{

    [SerializeField]
    private float _laserSpeed = 8.0f;

    [SerializeField]
    private bool _isEnemyLaser = false;

    [SerializeField]
    private bool _enemyFireUp = false;

    [SerializeField]
    private bool _isFireworks = false;

    //Private Variable
    private bool _isHomingLaser = false;
    private GameObject _target;

    void Update()
    {
        if(!_isEnemyLaser)
        {
            if (!_isFireworks)
            {

                if (!_isHomingLaser)
                {

                    PlayerLaser();
                }

                else
                {
                    LockedOnEnemy();
                }
            }
            else
            {
                Fireworks();
            }
        }
        else
        {
            if (!_enemyFireUp)
            {
                if (!_isFireworks)
                {
                    EnemyLaser();
                }
                else
                {
                    Fireworks();
                }
            }
            else
            {
                PlayerLaser();
            }
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

    void Fireworks()
    {
        transform.Translate(Vector3.up * Time.deltaTime * _laserSpeed);
        RemoveLaser();

    }


    void RemoveLaser()
    {
        if (transform.position.y >= 11 || transform.position.y <= -11 || transform.position.x >= 11 || transform.position.x <= -11)
        {

            Destroy(this.gameObject);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && _isEnemyLaser)
        {
            Player ps = collision.GetComponent<Player>();
            ps.Damage(1);
            Destroy(this.gameObject);
        }
    }

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
    }

    public void HomingLaser(GameObject go)
    {
        _isHomingLaser = true;
        _target = go;

    }

    void LockedOnEnemy()
    {
        if (_target != null)
        {
            //transform.LookAt(_target.transform);
            transform.up = _target.transform.position - transform.position;
            transform.position = Vector2.MoveTowards(transform.position, _target.transform.position, _laserSpeed * Time.deltaTime);
            
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    


}
