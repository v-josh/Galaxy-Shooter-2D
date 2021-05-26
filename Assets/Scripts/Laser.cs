﻿using System.Collections;
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

    void Update()
    {
        if(!_isEnemyLaser)
        {
            if (!_isFireworks)
            {
                PlayerLaser();
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

    


}
