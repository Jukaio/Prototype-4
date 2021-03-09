using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunny : AnimalSystem
{
    public override void on_pat(InnerState state)
    {
        inner_state.change(InnerState.Emotion.Loved, 1.0f);
        inner_state.set(InnerState.Emotion.Lonely, 0.0f);
    }

    public override void on_start(InnerState state)
    {
        state.set(InnerState.Emotion.Loved, 0.5f);
    }
    public InnerState.Emotion emotion;
    public override void on_update(AnimalShelter shelter, InnerState state)
    {
        state.change(InnerState.Emotion.Lonely, 0.1f * Time.deltaTime);
        state.change(InnerState.Emotion.Loved, -0.1f * Time.deltaTime);

        var sorted = state.get_emotions_sorted_by_intensity();
        var current = sorted.GetEnumerator();
        if (current.MoveNext())
        {
            emotion = current.Current.Value;
            set_animation(emotion);
        }

        if (state.get(InnerState.Emotion.Lonely) > 0.8f)
            shelter.kill(this);
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
}
