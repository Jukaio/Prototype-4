using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementState : PlayerState
{
    [SerializeField] private float speed;
    [SerializeField] private Vector2 animal_offset;
    public int LookDirIndex 
    { 
        get 
        { 
            return curr_look_dir.x == 0 ? 1 : curr_look_dir.x > 0 ? 1 : -1;  
        } 
    }
    private Vector3 prev_look_dir = Vector3.left;
    private Vector3 curr_look_dir = Vector3.left;
    private AnimalShelter animals;

    private SpriteRenderer sr;
    private Animator anim;
    

    void Start()
    {
        animals = GetComponent<AnimalShelter>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    private Vector3 handle_movement(IController controller)
    {
        Vector3 direction = Vector3.zero;
        direction = controller.is_pressed(Button.Left) ? direction + Vector3.left * Time.deltaTime : direction;
        direction = controller.is_pressed(Button.Right) ? direction + Vector3.right * Time.deltaTime : direction;
        return direction;
    }
    public void move(Vector3 input, float dt)
    {
        transform.position += input * dt * speed;
        curr_look_dir = input != Vector3.zero ? input.normalized : curr_look_dir;
    }
    public void pull_animals()
    {
        for (int i = 0; i < animals.Count; i++)
        {
            var at = transform.position + (((Vector3)animal_offset * (i + 1)) * (-curr_look_dir.x));
            //animals.set_position_of(i, at);
            animals.on_move(i, at);

        }
    }
    public void play_random_blink()
    {
        if (Random.Range(0, 5) == 0)
            anim.Play("reaper_idle1");
    }

    static float easing(float t)
    {
        return t * t * t;
    }

    private float transition_time = 1.25f;
    private float timer = 0.0f;
    private bool is_side_transitioning = false;
    private List<Vector3> from_positions = new List<Vector3>();
    private int animal_interpolate_count;
    private bool has_side_switched()
    {
        return prev_look_dir != curr_look_dir;
    }
    private bool in_side_transition()
    {
        return is_side_transitioning;
    }
    private void enter_side_transition()
    {
        from_positions.Clear();
        for (int i = 0; i < animals.Count; i++)
        {
            var to = transform.position + (((Vector3)animal_offset * (i + 1)) * (-curr_look_dir.x));
            from_positions.Add(animals.get_final_position(i));
        }
        animal_interpolate_count = animals.Count;
        timer = 0.0f;
        is_side_transitioning = true;
    }  
    private bool run_side_transition()
    {
        timer += Time.deltaTime;
        for (int i = 0; i < animal_interpolate_count; i++)
        {
            var t = timer / transition_time;
            var to = transform.position + (((Vector3)animal_offset * (i + 1)) * (-curr_look_dir.x));
            //var at = Vector3.Lerp(from_positions[i], to, easing(t));
            var at = Vector3.Lerp(from_positions[i], to, easing(t));
            animals.on_move(i, at);
        }
        return timer >= transition_time;
    }
    private void exit_side_transition()
    {
        is_side_transitioning = false;
    }

    public override System.Type act(IController controller, float dt)
    {
        if (controller.is_down(Button.Down) && 
            animals.occupied() &&
            !in_side_transition()) 
            return typeof(CaringState);

        var use = handle_movement(controller);

        anim.SetBool("moving", use.magnitude != 0.0f);

        prev_look_dir = curr_look_dir;
        move(use, dt);

        if (has_side_switched()) {
            sr.flipX = curr_look_dir.x > 0.0f;
            enter_side_transition();
        }

        if (in_side_transition()) {
            if(run_side_transition()) {
                exit_side_transition();
            }
        }
        else {
            pull_animals();
        }

        return GetType();
    }


    public override void enter()
    {

    }

    public override void exit()
    {
        
    }
}
