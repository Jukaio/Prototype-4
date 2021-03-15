using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Follow : MonoBehaviour
{
    [SerializeField] int delay_frames;
    [SerializeField] GameObject to_follow;
    private Queue<Vector3> positions = new Queue<Vector3>();

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        positions.Enqueue(to_follow.transform.position);
    }

    private void LateUpdate()
    {
        if (positions.Count > delay_frames)
        {
            var clamped_x = Mathf.Clamp(positions.Dequeue().x, -7.0f, 7.0f);
            transform.position = new Vector3(clamped_x, transform.position.y, transform.position.z);
        }
    }
}
