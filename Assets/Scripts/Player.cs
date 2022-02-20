using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public GameController gameController;

	void OnTriggerEnter(Collider other) {

		if (other.gameObject.CompareTag ("Pick up")) {
			if (gameController != null) {
                other.gameObject.GetComponent<AudioSource>().Play();
                gameController.EnergyPickedUp(other);
            }
		} 
        else if (other.gameObject.CompareTag("Finish"))
        {
            if (gameController != null)
            {
                other.gameObject.GetComponent<AudioSource>().Play();
                gameController.GameFinished(other);
            }
        }
    }
}
