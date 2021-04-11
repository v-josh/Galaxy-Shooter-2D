using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int _playerSpeed = 2;

    void Start()
    {
        //transform.position = new Vector3(0, 0, 0);

    }

    // Update is called once per frame
    void Update()
    {

        CalculateMovement();

    }

    void CalculateMovement()
    {
        //Variables
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalAxis, verticalAxis, 0);


        //Vertical Clamp
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0f), 0f);

        //Horizontal Clamp
        if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, transform.position.z);
        }

        //Move the player
        transform.Translate(direction * Time.deltaTime * _playerSpeed);
    }
}
