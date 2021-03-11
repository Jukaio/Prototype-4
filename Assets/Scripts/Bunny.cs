using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunny : AnimalSystem
{
    private Animator anim;

    public void play_random_idle()
    {
        if (Random.Range(0, 5) == 0)
            anim.Play("bunny_idle1");
        else if (Random.Range(0, 8) == 0)
            anim.Play("bunny_idle2");
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
    }
    public InnerState.Emotion emotion;
    public override void on_update(AnimalShelter shelter, InnerState state)
    {
        state.change(InnerState.Emotion.Lonely, 0.01f * Time.deltaTime);
        state.change(InnerState.Emotion.Loved, -0.01f * Time.deltaTime);

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

    public override void on_animation(Vector3 velocity)
    {
        anim.SetBool("moving", velocity.magnitude > 0.0f);
    }

    public override void on_feed(InnerState state)
    {

    }
}
