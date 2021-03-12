using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : AnimalSystem
{
    private Animator anim;


    public void play_random_idle()
    {
        if (Random.Range(0, 4) == 0)
            anim.Play("bird_idle1");
    }
    public override void on_animation(Vector3 velocity)
    {
        anim.SetBool("moving", velocity.magnitude > 0.0f);
    }

    public override void on_feed(InnerState state)
    {
    }

    public override void on_pat(InnerState state)
    {
    }

    public override void on_shelter_enter(AnimalShelter shelter, InnerState state)
    {
    }

    public override void on_shelter_exit(AnimalShelter shelter, InnerState state)
    {

    }

    public override void on_start(InnerState state)
    {
        anim = GetComponent<Animator>();
    }

    public override void on_update(AnimalShelter shelter, InnerState state)
    {
       
    }

    public override void on_waiting(InnerState state)
    {

    }
}
