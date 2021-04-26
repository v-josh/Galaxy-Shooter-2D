using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    //Private Variables
    private AudioSource _explosionSource;

    // Start is called before the first frame update
    void Start()
    {
        _explosionSource = GetComponent<AudioSource>();
        _explosionSource.Play();
        Destroy(this.gameObject, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
