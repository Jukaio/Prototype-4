using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaringState : PlayerState
{
    private enum State
    {
        close,
        selection,
        menu,
        count
    }

    [SerializeField] private GameObject arrow;
    [SerializeField] private GameObject menu;
    [SerializeField] private Sprite unhovered;
    [SerializeField] private Sprite hovered;
    [SerializeField] private Vector2 offset;

    private SpriteRenderer[] menu_sprites = null;
    private AnimalShelter animals;
    private MovementState ms;
    private SpriteRenderer sr;
    private List<AnimalSystem> animals_around_player = new List<AnimalSystem>();

    private State prev_level = 0;
    private State curr_level = 0;
    private int prev_index = 0;
    private int curr_index = 0;
    private int prev_tool = 0;
    private int curr_tool = 0;

    private int ToolCount { get{ return menu.transform.childCount; } }

    const int max = (int)State.count - 1;

    void Start()
    {
        animals = GetComponent<AnimalShelter>();
        ms = GetComponent<MovementState>();
        sr = GetComponent<SpriteRenderer>();
        arrow.SetActive(false);
        menu.SetActive(false);

        menu_sprites = new SpriteRenderer[menu.transform.childCount];
        var children = menu.transform.GetComponentsInChildren<SpriteRenderer>();
        for(int i = 0; i < children.Length; i++) {
            menu_sprites[i] = children[i];
            menu_sprites[i].sprite = unhovered;
        }
        menu_sprites[0].sprite = hovered;
    }

    private void increment_index()
    {
        curr_index = (curr_index + 1) % animals.Count;
    }

    private void decrement_index()
    {
        curr_index = (curr_index + animals.Count - 1) % animals.Count;
    }
    private void increment_tool()
    {
        curr_tool = (curr_tool + 1) % ToolCount;
    }

    private void decrement_tool()
    {
        curr_tool = (curr_tool + ToolCount - 1) % ToolCount;
    }

    private void handle_inputs(IController controller)
    {
        prev_level = curr_level;
        if (controller.is_down(Button.Down)) curr_level++;
        if (controller.is_down(Button.Up)) curr_level--;
        curr_level = (State)Mathf.Clamp((int)curr_level, 0, max);

        if (is_selecting())
        {
            prev_index = curr_index;
            if (controller.is_down(Button.Left))
                if (ms.LookDirIndex > 0) 
                    increment_index();
                else 
                    decrement_index();
            if (controller.is_down(Button.Right))
                if (ms.LookDirIndex < 0) 
                    increment_index();
                else 
                    decrement_index();
        }
        else if (is_in_menu())
        {
            prev_tool = curr_tool;
            if (controller.is_down(Button.Left))
                decrement_tool();
            if (controller.is_down(Button.Right))
                increment_tool();
        }
    }
    private bool is_transitioning()
    {
        return prev_level != curr_level;
    }
    private bool has_arrow_moved()
    {
        return curr_index != prev_index;
    }
    private bool has_tool_changed()
    {
        return curr_tool != prev_tool;
    }

    private bool is_selecting()
    {
        return curr_level == State.selection;
    }
    private bool is_in_menu()
    {
        return curr_level == State.menu;
    }
    private bool refresh_state()
    {
        // Refresh state
        switch (curr_level)
        {
            case State.close:
                return true; //Exit

            case State.selection:
                arrow.SetActive(true);
                menu.SetActive(false);
                break;
            case State.menu:
                arrow.SetActive(true);
                menu.SetActive(true);
                break;
        }
        return false;
    }

    private void refresh_arrow()
    {
        var at = animals.get_position_of(curr_index);
        arrow.transform.position = at + (Vector3)offset;
    }

    private void refresh_tool()
    {
        menu_sprites[prev_tool].sprite = unhovered;
        menu_sprites[curr_tool].sprite = hovered;
    }

    public bool are_animals_close()
    {
        return animals_around_player.Count > 0;
    }

    public override System.Type act(IController controller)
    {
        if(animals.empty())
            return typeof(MovementState);

        handle_inputs(controller);
        if (is_transitioning())
            if(refresh_state()) 
                return typeof(MovementState);

        refresh_arrow();

        if (has_tool_changed())
            refresh_tool();

        return GetType();
    }

    public override void enter()
    {
        arrow.SetActive(true);
        menu.SetActive(false);
        refresh_arrow();
        curr_level = State.selection;
        sr.flipX = !sr.flipX;
    }

    public override void exit()
    {
        arrow.SetActive(false);
        menu.SetActive(false);
        curr_level = State.close;
        sr.flipX = !sr.flipX;
        StartCoroutine(block_adoption());
        refresh_close_animals();
    }

    public void on_meeting(AnimalSystem animal)
    {
        if(animals.already_adopted(animal))
            return;

        animals_around_player.Add(animal);
        refresh_close_animals();
    }

    public bool allow_adopting = true;
    public IEnumerator block_adoption(float time = 0.4f)
    {
        allow_adopting = false;
        yield return new WaitForSeconds(time);
        allow_adopting = true;
    }
    public override void on_special(IController controller)
    {
        if(controller.is_down(Button.Up) && allow_adopting) {
            bool animals_close = are_animals_close();
            if (animals_close)
            {
                StartCoroutine(block_adoption());

                var selection = animals_around_player[animals_around_player.Count - 1];
                animals.adopt(selection);
                animals_around_player.Remove(selection);

                refresh_close_animals();
            }
        }
    }
    private void refresh_close_animals()
    {
        bool animals_close = are_animals_close();
        if (animals_close)
        {
            var selection = animals_around_player[animals_around_player.Count - 1];
            arrow.transform.position = selection.transform.position + Vector3.up * 0.5f;
        }
        arrow.SetActive(animals_close);
    }
    public void on_leaving(AnimalSystem animal)
    {
        if (animals.already_adopted(animal))
            return;
        animals_around_player.Remove(animal);
        refresh_close_animals();
    }
}
