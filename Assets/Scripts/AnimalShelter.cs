using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimalShelter : MonoBehaviour
{
    public class AnimalKiller : Disolve.Callback
    {
        AnimalShelter shelter;

        public AnimalKiller(AnimalShelter context)
        {
            shelter = context;
        }

        public void on_appear(GameObject context)
        {

        }

        public void on_disappear(GameObject context)
        {
            var animal = context.GetComponent<AnimalSystem>();
            animal.set_collidable(false);
            shelter.abadondon(animal);
            animal.gameObject.SetActive(false);
        }
    }
    public class AnimalAdder : Disolve.Callback
    {
        AnimalShelter shelter;

        public AnimalAdder(AnimalShelter context)
        {
            shelter = context;
        }

        public void on_appear(GameObject context)
        {

        }

        public void on_disappear(GameObject context)
        {
            var animal = context.GetComponent<AnimalSystem>();
            shelter.animals.Add(animal);
            animal.appear(animal.gameObject);
        }
    }
    
    AnimalAdder adder = null;
    AnimalKiller killer = null;
    [SerializeField] private List<AnimalSystem> animals = new List<AnimalSystem>();
    public int Count { get{ return animals.Count; } }

    public AnimalSystem get(int index)
    {
        return animals[index];
    }
    public bool has_animal(int at)
    {
        if (empty())
            return false;

        if (at < 0)
            return false;

        return animals[at] != null;
    }

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
        animal.disappear(animal.gameObject, adder);
        animal.on_adopt(this);
    }

    public void abadondon(AnimalSystem animal)
    {
        animal.on_abandon(this);
        animals.Remove(animal);
    }
    public void abadondon(int index)
    {
        animals[index].on_abandon(this);
        animals.RemoveAt(index);
    }

    public void kill(int index)
    {
        if (!has_animal(index))
            return;

        animals[index].disappear(animals[index].gameObject, killer);
    }
    public void kill(AnimalSystem animal)
    {
        kill(animals.IndexOf(animal));
    }

    public bool already_adopted(AnimalSystem animal)
    {
        return animals.Contains(animal);
    }

    public Vector3 get_position_of(int index)
    {
        if (!has_animal(index))
            return Vector3.zero;
        return animals[index].transform.position;
    }
    public Vector3 get_final_position(int index)
    {
        if (!has_animal(index))
            return Vector3.zero;
        return animals[index].get_last_position();
    }

    public void on_move(int index, Vector3 target)
    {
        if (index < 0 || !has_animal(index)) {
            return;
        }

        animals[index].on_move(target);
    }
    private void Start()
    {
        adder = new AnimalAdder(this);
        killer = new AnimalKiller(this);
    }

    private void Update()
    {
        Stack<int> to_remove = new Stack<int>();
        for(int i = 0; i < animals.Count; i++)
        {
            if (animals[i] != null) {
                animals[i].update(this);
            }
            else{
                to_remove.Push(i);
            }
        }
        // Clean up if dirty nulls
        foreach(int i in to_remove) {
            animals.RemoveAt(i);
        }


        if(Input.GetKeyDown(KeyCode.K))
        {
            kill(animals[0]);
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            abadondon(animals[0]);
        }
    }

}
