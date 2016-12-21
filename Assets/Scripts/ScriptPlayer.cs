using UnityEngine;
using System.Collections;

public class ScriptPlayer : MonoBehaviour {

    //public GameObject arma;

    private float movementSpeed = 5.0f;
    
	void Update () {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * Time.deltaTime * movementSpeed;
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * Time.deltaTime * movementSpeed;
        }
    }
}
