using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrow : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") )
        {
            cube c = other.gameObject.GetComponent<cube>();
            if (c.ifFront == true)
            {
                clickManager.Instance.add(c);
                Destroy(this.gameObject);
            }
        }
    }
}
