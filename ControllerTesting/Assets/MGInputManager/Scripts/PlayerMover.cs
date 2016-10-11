using UnityEngine;
using System.Collections;
using MindGrown;

public class PlayerMover : MonoBehaviour
{	
	// Update is called once per frame
	void Update ()
    {
        CheckForMovement();	
	}

    void CheckForMovement()
    {
        float forwardAxis = InputManager.GetAxis(ActionName.MoveForeward);
        float backwardAxis = InputManager.GetAxis(ActionName.MoveBackward);
        float leftAxis = InputManager.GetAxis(ActionName.StrafeLeft);
        float rightAxis = InputManager.GetAxis(ActionName.StrafeRight);
        //Debug.Log("F: " + forwardAxis + " B: " + backwardAxis);
        if (forwardAxis > 0)
            transform.position = new Vector3(transform.position.x, transform.position.y + forwardAxis * 10f * Time.deltaTime, transform.position.z);
        if (backwardAxis > 0)
            transform.position = new Vector3(transform.position.x, transform.position.y - backwardAxis * 10f * Time.deltaTime, transform.position.z);
        if (leftAxis > 0)
            transform.position = new Vector3(transform.position.x - leftAxis * 10f * Time.deltaTime, transform.position.y, transform.position.z);
        if (rightAxis > 0)
            transform.position = new Vector3(transform.position.x + rightAxis * 10f * Time.deltaTime, transform.position.y, transform.position.z);
    }
}
