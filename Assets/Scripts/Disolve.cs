using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disolve : MonoBehaviour
{
    public interface Callback
    {
        public abstract void on_appear(GameObject context);
        public abstract void on_disappear(GameObject context);
    }

    Material material;

    bool isDisolving = false;
    float fade = 1f;

    void Start()
    {
        material = GetComponent<SpriteRenderer>().material;
    }

    public void disappear(GameObject context, Callback callback = null)
    {
        StartCoroutine(disappearing(callback, context));
    }

    public void appear(GameObject context, Callback callback = null)
    {
        StartCoroutine(appearing(callback, context));
    }

    IEnumerator disappearing(Callback callback, GameObject context)
    {
        // if already disolving
        if (isDisolving && fade > 0)
        {
            yield break;
        }

        // Set once flag
        isDisolving = true;

        // Slowly appear
        while (fade > 0f)
        {
            fade -= Time.deltaTime;
            material.SetFloat("_Fade", fade);
            yield return new WaitForEndOfFrame();
        }


        // Exit
        fade = 0f;
        isDisolving = false;
        if (callback != null) callback.on_disappear(context);
    }

    IEnumerator appearing(Callback callback, GameObject context)
    {
        // if already disolving
        if (isDisolving && fade < 1.0f) {
            yield break;
        }

        // Set once flag
        isDisolving = true;

        // Slowly appear
        while(fade < 1f) {
            fade += Time.deltaTime;
            material.SetFloat("_Fade", fade);
            yield return new WaitForEndOfFrame();
        }


        // Exit
        fade = 1f;
        isDisolving = false;
        if (callback != null) callback.on_appear(context);
    }
}
