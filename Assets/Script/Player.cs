using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    void Update()
    {
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.gameObject.layer == LayerMask.NameToLayer("DeathZone"))
        {
            clickManager.Instance.GameOver();
        }
    }
}
