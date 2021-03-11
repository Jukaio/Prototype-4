using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;


// If the view changes, other relevant cmaeras have to change too
public class CameraCorrection : MonoBehaviour
{
    [SerializeField] List<Camera> after;
    bool is_running;
    [SerializeField] float frequency = 1.0f;

    private void OnEnable()
    {
        is_running = true;
        StartCoroutine(refresh_camera(frequency));
    }

    IEnumerator refresh_camera(float frequency)
    {
        while (is_running)
        {
            float set_to = GetComponent<Camera>().orthographicSize;
            foreach (var cam in after) {
                cam.orthographicSize = set_to;
            }
            yield return new WaitForSeconds(frequency);
        }
    }

    private void OnDisable()
    {
        StopCoroutine(refresh_camera(frequency));
        is_running = false;
    }
}

