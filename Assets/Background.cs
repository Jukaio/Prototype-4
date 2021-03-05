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
    [SerializeField] private Camera camera;
    void Start()
    {
        int lower = Mathf.FloorToInt(buffer_count / 2.0f);
        int upper = Mathf.CeilToInt(buffer_count / 2.0f);
        buffer = new GameObject[layers.Length];
        for(int i = 0; i < layers.Length; i++)
        {
            var layer = layers[i];
            var parent = new GameObject("Layer " + i.ToString());

            var offset_x = (layer.layer.sprite.rect.width / layer.layer.sprite.pixelsPerUnit);


            parent.transform.parent = transform;
            layer.layer.transform.parent = parent.transform;
            layer.layer.transform.localPosition += Vector3.right * (offset_x * -lower);

            for (int j = 1; j < buffer_count; j++)
            {
                var next_layer = Instantiate(layer.layer.gameObject);
                next_layer.transform.localPosition += Vector3.right * offset_x * j;
                next_layer.transform.parent = parent.transform;
            }
            buffer[i] = parent;
        }
    }
    private void LateUpdate()
    {

        for (int i = 0; i < layers.Length; i++)
        {
            var layer = layers[i];
            float x = camera.transform.position.x - (camera.transform.position.x - transform.position.x) * layer.speed;
            buffer[i].transform.localPosition = new Vector3(x, buffer[i].transform.localPosition.y, 0);
        }
    }
}
