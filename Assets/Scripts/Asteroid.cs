using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{

    [SerializeField]
    private float _rotateSpeed = 3.0f;

    [SerializeField]
    private GameObject _explosion;


    //Private Variables
    private Animator _theExplosion;
    private SpawnManager _spMan;

    // Start is called before the first frame update
    void Start()
    {
        if(_explosion)
        {
            _theExplosion = _explosion.GetComponent<Animator>();
        }

        _spMan = GameObject.FindGameObjectWithTag("Spawn Manager").GetComponent<SpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0f, 0f, _rotateSpeed) * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Laser")
        {
            Destroy(collision.gameObject);
            Instantiate(_explosion, transform.position, Quaternion.identity);
            _spMan.StartSpawning();
            Destroy(this.gameObject, 0.2f);
        }
    }

}
