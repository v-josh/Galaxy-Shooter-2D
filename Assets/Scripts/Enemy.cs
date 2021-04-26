﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField]
    private float _enemySpeed = 4.0f;

    [SerializeField]
    private int _enemyScore = 10;

    [SerializeField]
    private GameObject _thePlayer;


    //Private Variables
    private Player _playerScript;
    private Animator _enemyAnim;
    private AudioSource _sourceEnemy;


    // Start is called before the first frame update
    void Start()
    {
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
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * Time.deltaTime * _enemySpeed);

        if (transform.position.y <= -5f)
        {
            transform.position = new Vector3(Random.Range(-10, 10), 7f, 0f);
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

            _enemyAnim.SetTrigger("OnEnemyDeath");
            _enemySpeed = 0f;
            _sourceEnemy.Play();
            Destroy(this.gameObject, 2.3f);
        }
        else if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            _playerScript.AddToScore(_enemyScore);

            _enemySpeed = 0f;
            _enemyAnim.SetTrigger("OnEnemyDeath");
            _sourceEnemy.Play();
            Destroy(this.gameObject, 2.3f);
        }
    }
}

