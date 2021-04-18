using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{

    [SerializeField]
    private float _laserSpeed = 8.0f;

    void Update()
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
}
