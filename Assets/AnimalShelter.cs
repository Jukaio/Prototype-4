using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalShelter : MonoBehaviour
{
    [SerializeField] private List<AnimalSystem> animals = new List<AnimalSystem>();
    public int Count { get{ return animals.Count; } }

    public bool occupied()
    {
        return Count > 0;
    }
    public bool empty()
    {
        return !occupied();
    }
    public void adopt(AnimalSystem animal)
    {
        animals.Add(animal);
    }
    public bool already_adopted(AnimalSystem animal)
    {
        return animals.Contains(animal);
    }

    public Vector3 get_position_of(int index)
    {
        return animals[index].transform.position;
    }
    public Vector3 get_final_position(int index)
    {
        return animals[index].get_last_position();
    }
    public void set_position_of(int index, Vector3 to)
    {
        animals[index].transform.position = to;
    }

    public void on_move(int index, Vector3 target)
    {
        animals[index].on_move(target);
    }

}
