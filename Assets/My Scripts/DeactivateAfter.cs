using System.Collections;
using UnityEngine;

public class DeactivateAfter : MonoBehaviour {

    [SerializeField]
    private float deactivateTimer;

	void Start () {
        StartCoroutine("Deactivate", deactivateTimer);
	}

    IEnumerator Deactivate(float t) {

        yield return new WaitForSeconds(t);
        gameObject.SetActive(false);

    }
	
}
