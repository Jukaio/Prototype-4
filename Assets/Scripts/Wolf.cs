using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : AnimalSystem
{
    private Animator anim;

    public void play_random_idle()
    {
        if (Random.Range(0, 5) == 0)
            anim.Play("wolf_idle1");
    }

    public override void on_pat(InnerState state)
    {
        inner_state.change(InnerState.Emotion.Loved, 1.0f);
        inner_state.set(InnerState.Emotion.Lonely, 0.0f);
    }

    public override void on_start(InnerState state)
    {
        anim = GetComponent<Animator>();

        state.set(InnerState.Emotion.Loved, 0.5f);
        state.set(InnerState.Emotion.Lonely, 0.0f);
    }
    public InnerState.Emotion prev;
    public InnerState.Emotion emotion;
    public override void on_update(AnimalShelter shelter, InnerState state)
    {
        state.change(InnerState.Emotion.Lonely, 0.1f * Time.deltaTime);
        state.change(InnerState.Emotion.Loved, -0.1f * Time.deltaTime);

        var sorted = state.get_emotions_sorted_by_intensity();
        var current = sorted.GetEnumerator();
        prev = emotion;
        if (current.MoveNext())
        {
            emotion = current.Current.Value;
            if(prev != emotion)
                set_animation(emotion);
        }

        if (state.get(InnerState.Emotion.Lonely) > 0.8f) // Replace lonely with hunger
        {
            if(shelter.Count == 1) // if only animal
            {
                shelter.kill(this);
                return;
            }

            var me = shelter.get_index(this);
            if(me == 0) // at edge 
            {
                var neighbour = shelter.get(me + 1);
                if(neighbour.GetType() != typeof(Wolf)) {
                    shelter.kill(neighbour);
                }
                else{
                    shelter.kill(this);
                }
            }
            else if (me == shelter.Count - 1) // at edge
            {
                var neighbour = shelter.get(me - 1);
                if (neighbour.GetType() != typeof(Wolf)) {
                    shelter.kill(neighbour);
                }
                else {
                    shelter.kill(this);
                }
            }
            else {
                var neighbour = shelter.get(me + 1);
                if (neighbour.GetType() != typeof(Wolf)) {
                    shelter.kill(neighbour);
                }
                else{
                    neighbour = shelter.get(me - 1);
                    if (neighbour.GetType() != typeof(Wolf)) {
                        shelter.kill(neighbour);
                    }
                    else{
                        shelter.kill(this);
                    }
                }
            }

            state.set(InnerState.Emotion.Loved, 0.5f);
            state.set(InnerState.Emotion.Lonely, 0.0f);
        }
    }

    public override void on_shelter_enter(AnimalShelter shelter, InnerState state)
    {

    }

    public override void on_shelter_exit(AnimalShelter shelter, InnerState state)
    {

    }

    public override void on_waiting(InnerState state)
    {
        set_animation(emotion);
    }

    public override void on_animation(Vector3 velocity)
    {
        anim.SetBool("moving", velocity.magnitude > 0.0f);
    }

    public override void on_feed(InnerState state)
    {

    }
}