using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapAnimals : MenuEntry
{
    public enum Direction
    {
        Right,
        Left
    }

    [SerializeField] PlayerSystem player;
    [SerializeField] Direction direction;
    [SerializeField] bool block_wrap_swap;
    private AnimalShelter shelter;
    private CaringState care_state;
    private MovementState ms;



    private int increment(int index, int size)
    {
        if (block_wrap_swap) 
            return Mathf.Clamp(index + 1, 0, size - 1);
        return (index + 1) % size;
    }

    private int decrement(int index, int size)
    {
        if (block_wrap_swap) 
            return Mathf.Clamp(index - 1, 0, size - 1);
        return (index + size - 1) % size;
    }

    private void Start()
    {
        shelter = player.GetComponent<AnimalShelter>();
        care_state = player.GetComponent<CaringState>();
        ms = player.GetComponent<MovementState>();
    }

    public override void use(AnimalSystem selection, int index)
    {
        if (shelter.empty())
            return;

        var swap_to = -1;
        switch (direction)
        {
            case Direction.Right:
                swap_to = ms.LookDirIndex > 0 ?
                          decrement(index, shelter.Count) :
                          increment(index, shelter.Count);
                break;
            case Direction.Left:
                swap_to = ms.LookDirIndex > 0 ?
                          increment(index, shelter.Count) :
                          decrement(index, shelter.Count);
                break;
            default: // On error, just return
                return;
        }

        shelter.swap(index, swap_to);
        care_state.set_current_selection_index(swap_to);
    }
}
