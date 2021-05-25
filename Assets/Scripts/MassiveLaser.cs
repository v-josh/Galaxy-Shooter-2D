using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassiveLaser : MonoBehaviour
{

    [SerializeField]
    private float _speed = 2f;


    private bool _canMove = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_canMove)
        {
            MoveLaser();
        }

        if(this.transform.position.y < -15f)
        {
            Destroy(this.gameObject);
        }
    }

    void MoveLaser()
    {
        transform.Translate(Vector2.down * Time.deltaTime * _speed);
    }

    public void ActivateMovement()
    {
        StartCoroutine(CanMove());
    }

    IEnumerator CanMove()
    {
        yield return new WaitForSeconds(20f);
        _canMove = true;
    }

}
