using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassiveLaser : MonoBehaviour
{

    private bool _bossAlive = true;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player" && _bossAlive)
        {
            Player ps = collision.gameObject.GetComponent<Player>();
            ps.Damage(1);
        }
    }

    public void StillAlive(bool x)
    {
        _bossAlive = x;
    }

}
