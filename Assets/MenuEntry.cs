using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuEntry : MonoBehaviour
{
    public virtual void use(AnimalSystem selection)
    {
        Debug.Log("Using...");
    }
}
