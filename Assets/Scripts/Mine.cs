using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{

    [SerializeField]
    private float _timeBeforeExplosion = 0.3f;

    [SerializeField]
    private float _waitForExplosion = 1.0f;

    [SerializeField]
    private float _speed = 2.0f;

    [SerializeField]
    private GameObject _explosion;

    [SerializeField]
    private GameObject _laserShrapnel;



    private bool _stopMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Moving());
    }

    // Update is called once per frame
    void Update()
    {
        if (!_stopMoving)
        {
            transform.Translate(Vector3.down * Time.deltaTime * _speed);
        }

        if(transform.position.y < -4.5f || transform.position.x < -11.3f || transform.position.x > 11.3f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Laser")
        {
            Destroy(collision.gameObject);
            Instantiate(_explosion, transform.position, Quaternion.identity);
            Destroy(this.gameObject, 0.2f);
        }
        else if (collision.tag == "Player")
        {
            Player ps = collision.gameObject.GetComponent<Player>();
            ps.Damage();
            Instantiate(_explosion, transform.position, Quaternion.identity);
            Destroy(this.gameObject, 0.2f);
        }


    }

    IEnumerator Moving()
    {

        yield return new WaitForSeconds(_timeBeforeExplosion);
        _stopMoving = true;
        yield return new WaitForSeconds(_waitForExplosion);
        Instantiate(_explosion, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        Instantiate(_laserShrapnel, transform.position, Quaternion.identity);
        Destroy(this.gameObject, 0.2f);

    }


}
