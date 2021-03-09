using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSystem : MonoBehaviour
{
    [HideInInspector] private Dictionary<System.Type, PlayerState> states;
    [HideInInspector] private System.Type previous;
    [HideInInspector] private System.Type current;
    [HideInInspector] private Controller fixed_controller;
    [HideInInspector] private Controller controller;

    public bool in_caring_state()
    {
        return current.GetType() == typeof(CaringState);
    }
    public bool in_movement_state()
    {
        return current.GetType() == typeof(MovementState);
    }


    void Start()
    {
        fixed_controller = new Controller();
        controller = new Controller();
        states = new Dictionary<System.Type, PlayerState>();
        var cs = GetComponent<CaringState>();
        cs.enabled = false;
        var ms = GetComponent<MovementState>();
        states.Add(cs.GetType(), cs);
        states.Add(ms.GetType(), ms);
        current = typeof(MovementState);
        ms.enabled = true;
    }

    private bool is_transitioning()
    {
        return previous != current;
    }
    private void change_state()
    {
        states[previous].enabled = false;
        states[previous].exit();
        states[current].enabled = true;
        states[current].enter();
    }

    void dispatch()
    {
        previous = current;
        current = states[current].act(controller);
        if (is_transitioning())
            change_state();
    }

    float accumulator = 0.0f;
    
    void Update()
    {
        accumulator += Time.deltaTime;
        if(accumulator >= Time.fixedDeltaTime)
        {
            accumulator -= Time.fixedDeltaTime;

            // Every fixed step, reset
            fixed_controller.reset();
        }
        controller.update();
        fixed_controller.do_fixed_update(controller);
        dispatch();
    }

    private void FixedUpdate()
    {
        states[current].fixed_act(fixed_controller);
    }


    // Special stateless situations 
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Animal"))
            states[typeof(CaringState)].on_special(fixed_controller);
    }
}
