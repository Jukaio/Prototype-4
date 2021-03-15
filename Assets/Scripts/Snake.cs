using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : AnimalSystem
{
    private Animator anim;
    private void become_hungry(InnerState state)
    {
        state.change(InnerState.Emotion.Hungry, 0.03f * Time.deltaTime);
        state.change(InnerState.Emotion.NotHungry, -0.03f * Time.deltaTime);
    }

    private void refresh_emotion(InnerState state, bool forced = false)
    {
        var sorted = state.get_emotions_sorted_by_intensity();
        var current = sorted.GetEnumerator();
        prev = emotion;
        if (current.MoveNext())
        {
            emotion = current.Current.Value;
            bool changed = prev != emotion;
            if (changed || forced)
                set_animation(emotion);
        }
    }
    private bool can_eat(AnimalSystem neighbour)
    {
        return neighbour.GetType() != typeof(Wolf) &&
               neighbour.GetType() != typeof(Snake) &&
               neighbour.GetType() != typeof(Stag);
    }
    private bool try_eat_neighbour(AnimalShelter shelter, InnerState state)
    {
        if (shelter.Count == 1) // if only animal
        {
            shelter.kill(this);
            return false;
        }

        var me = shelter.get_index(this);
        if (me == 0) // at edge 
        {
            var neighbour = shelter.get(me + 1);
            if (can_eat(neighbour))
            {
                shelter.kill(neighbour);
                return true;
            }
            else
            {
                shelter.kill(this);
                return false;
            }
        }
        else if (me == shelter.Count - 1) // at edge
        {
            var neighbour = shelter.get(me - 1);
            if (can_eat(neighbour))
            {
                shelter.kill(neighbour);
                return true;
            }
            else
            {
                shelter.kill(this);
                return false;
            }
        }
        else
        {
            var neighbour = shelter.get(me + 1);
            if (can_eat(neighbour))
            {
                shelter.kill(neighbour);
                return true;
            }
            else
            {
                neighbour = shelter.get(me - 1);
                if (can_eat(neighbour))
                {
                    shelter.kill(neighbour);
                    return true;
                }
                else
                {
                    shelter.kill(this);
                    return false;
                }
            }
        }
    }

    public override void on_pat(InnerState state)
    {

    }

    public override void on_start(InnerState state)
    {
        anim = GetComponent<Animator>();

        state.set(InnerState.Emotion.Hungry, 0.0f);
        state.set(InnerState.Emotion.NotHungry, 1.0f);
    }

    public InnerState.Emotion prev;
    public InnerState.Emotion emotion;
    public override void on_update(AnimalShelter shelter, InnerState state)
    {
        become_hungry(state);

        if (state.get(InnerState.Emotion.Hungry) > 0.9f) // Replace lonely with hunger
        {
            if (try_eat_neighbour(shelter, state))
            {
                state.set(InnerState.Emotion.Hungry, 0.0f);
                state.set(InnerState.Emotion.NotHungry, 1.0f);
            }

        }

        refresh_emotion(state);
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
        inner_state.set(InnerState.Emotion.Hungry, 0.0f);
        inner_state.set(InnerState.Emotion.NotHungry, 1.0f);
        refresh_emotion(state, true);
    }
}
