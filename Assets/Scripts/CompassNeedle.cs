using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassNeedle : MonoBehaviour
{

    Vector2 lookDirection;

    // Start is called before the first frame update
    void Start()
    {
        lookDirection = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        lookDirection = SignalFire.Instance.transform.position - Player.Instance.transform.position;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, lookDirection);
    }
}
