using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Commandable
{
    public abstract void on_move(Vector3 position);
}

public class AnimalSystem : MonoBehaviour, Commandable
{
    private struct TimestampMove
    {
        public TimestampMove(Vector3 pos, float t)
        {
            time = t;
            position = pos;
        }
        public Vector3 position;
        public float time;
    }

    private List<TimestampMove> positions = new List<TimestampMove>();

    [SerializeField] private float reaction_delay = 0.5f;
    float threshhold = 1.0f;

    public Vector3 get_last_position()
    {
        return positions[positions.Count - 1].position;
    }

    private TimestampMove[] find_pair_at(float time)
    {
        // 0 = from; 1 = to
        TimestampMove[] to_return = new TimestampMove[2];
        for(int i = 0; i < positions.Count; i++) {
            if (positions[i].time > time) {
                to_return[0] = positions[i];
                to_return[1] = positions[i + 1];
                return to_return;
            }
        }
        return to_return;
    }

    void Update()
    {
        float my_time = Time.time - reaction_delay;

        if (positions.Count > 1)
        {
            if (positions[positions.Count - 2].time > my_time)
            {
                var positions = find_pair_at(my_time);
                var from = positions[0];
                var to = positions[1];

                float full = to.time - from.time;
                float current = my_time - from.time;

                float t = current / full;
                transform.position = Vector3.Lerp(from.position, to.position, t);
            }
        }

        positions.RemoveAll((TimestampMove move) => { 
            return move.time < Time.time - threshhold; 
        });
    }

    public void on_move(Vector3 position)
    {
        positions.Add(new TimestampMove(position, Time.time));
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            collision.GetComponent<CaringState>().on_meeting(this);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            collision.GetComponent<CaringState>().on_leaving(this);
    }
}
