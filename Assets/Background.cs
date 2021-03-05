using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LayerWithSpeed
{
    public SpriteRenderer layer;
    public float speed;
}

public class Background : MonoBehaviour
{
    [SerializeField] int buffer_count;
    [SerializeField] private LayerWithSpeed[] layers;
    [SerializeField] private GameObject[] buffer;
    private GameObject[][] children;
    private float[] offsets;
    [SerializeField] private Camera camera;
    void Start()
    {
        int lower = Mathf.FloorToInt(buffer_count / 2.0f);
        int upper = Mathf.CeilToInt(buffer_count / 2.0f);
        buffer = new GameObject[layers.Length];
        children = new GameObject[layers.Length][];

        offsets = new float[layers.Length];

        for (int i = 0; i < layers.Length; i++)
        {
            var layer = layers[i];
            var parent = new GameObject("Layer " + i.ToString());

            children[i] = new GameObject[buffer_count];
            var child_holder = children[i];

            var offset_x = (layer.layer.sprite.rect.width / layer.layer.sprite.pixelsPerUnit);
            offsets[i] = offset_x;

            parent.transform.parent = transform;
            layer.layer.transform.parent = parent.transform;
            layer.layer.transform.localPosition += Vector3.right * (offset_x * -lower);

            for (int j = 1; j < buffer_count; j++)
            {
                var next_layer = Instantiate(layer.layer.gameObject);
                next_layer.transform.localPosition += Vector3.right * offset_x * j;
                next_layer.transform.parent = parent.transform;
                child_holder[j] = next_layer;
            }
            buffer[i] = parent;
        }
    }
    private void Update()
    {

        for (int i = 0; i < layers.Length; i++)
        {
            var layer = layers[i];
            float x = camera.transform.position.x - (camera.transform.position.x - transform.position.x) * layer.speed;
            buffer[i].transform.localPosition = new Vector3(x, buffer[i].transform.localPosition.y, 0);
        }

        for (int i = 0; i < layers.Length; i++)
        {
            // Currrent position
            var x = camera.transform.position.x;
            var offset = offsets[i];
            int lower = Mathf.FloorToInt(buffer_count / 2.0f);
            int upper = Mathf.CeilToInt(buffer_count / 2.0f);
            var offset_lower = offset * -lower;
            var offset_upper = offset * upper;

            

            var layer = layers[i];
            var item = buffer[i];
            
        }
    }
}
