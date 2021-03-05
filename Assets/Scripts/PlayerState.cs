using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Button
{
    Left,
    Right,
    Up,
    Down
}
public interface IController
{
    public abstract bool is_pressed(Button button);

    // onKeyDown
    public abstract bool is_down(Button button);
}

public class Controller : Object, IController
{
    private bool[] down = new bool[4];
    private bool[] state = new bool[4];

    public void assign(Controller from)
    {
        for(int i = 0; i < 4; i++)
        {
            down[i] = from.down[i];
            state[i] = from.state[i];
        }
    }

    public Controller copy()
    {
        return (Controller) this.MemberwiseClone();
    }

    public void reset()
    {
        for (int i = 0; i < 4; i++)
        {
            down[i] = false;
            state[i] = false;
        }
    } 

    private void refresh_down(Button button, KeyCode code)
    {
        down[(int)button] = Input.GetKeyDown(code);
    }
    private void refresh_state(Button button, KeyCode code)
    {
        state[(int)button] = Input.GetKey(code);

    }
    public bool is_pressed(Button button)
    {
        return state[(int)button];
    }
    
    // onKeyDown
    public bool is_down(Button button)
    {
        return down[(int)button];
    }

    public void do_fixed_update(Controller update)
    {
        for (int i = 0; i < 4; i++)
        {
            down[i] = update.down[i] || down[i];
            state[i] = update.state[i] || state[i];
        }
    }

    public void update()
    {
        refresh_down(Button.Left, KeyCode.A);
        refresh_down(Button.Right, KeyCode.D);
        refresh_down(Button.Down, KeyCode.S);
        refresh_down(Button.Up, KeyCode.W);

        refresh_state(Button.Left, KeyCode.A);
        refresh_state(Button.Right, KeyCode.D);
        refresh_state(Button.Down, KeyCode.S);
        refresh_state(Button.Up, KeyCode.W);
    }
}

// Interface for player-related states
public abstract class PlayerState : MonoBehaviour
{
    public abstract void enter();
    public abstract void exit();
    public abstract System.Type act(IController controller);
    public virtual void fixed_act(IController controller) { }
    public virtual void on_special(IController controller) { }

}


