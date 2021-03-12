using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;
public interface Commandable
{
    public abstract void on_move(Vector3 position);
}

public interface Emotional
{
    public abstract void on_start(InnerState state);
    public abstract void on_shelter_enter(AnimalShelter shelter, InnerState state);
    public abstract void on_shelter_exit(AnimalShelter shelter, InnerState state);
    public abstract void on_update(AnimalShelter shelter, InnerState state);
    public abstract void on_animation(Vector3 velocity);
    public abstract void on_pat(InnerState state);
    public abstract void on_feed(InnerState state);
    public abstract void on_waiting(InnerState state);
}

[System.Serializable]
public class BinaryFeeling
{
    const float normalised = 1.0f;
    private float negative_weight;
    private float positive_weight;
    private float bound;

    public BinaryFeeling(float positive = 0.0f, float negative = 0.0f, float bound = normalised)
    {
        // If passed bound is smaller or equal to 0.0f
        // assign the default 
        if (bound <= 0.0f)
        {
            bound = normalised;
        }

        this.bound = bound;
        this.negative_weight = Mathf.Clamp(negative, 0.0f, bound);
        this.positive_weight = Mathf.Clamp(positive, 0.0f, bound);
    }
    public bool IsNegeative { get { return Value < 0.0f; } }
    public bool IsPositive { get { return Value > 0.0f; } }
    public bool IsNeutral { get { return Value == 0.0f; } }

    public float Value 
    { 
        get
        {
            var that = positive_weight - negative_weight;
            return that;
        }
    }
    public float Positive
    {
        get
        {
            return positive_weight;
        }
        set
        {
            this.positive_weight = Mathf.Clamp(value, 0.0f, bound); ;
        }
    }
    public float Negative
    {
        get
        {
            return negative_weight;
        }
        set
        {
            this.negative_weight = Mathf.Clamp(value, 0.0f, bound); ;
        }
    }

}

[System.Serializable]
public class InnerState
{
    public enum Emotion
    {
        None = 0,
        Loved = 1,
        Lonely = -Loved,
        NotHungry = 2,
        Hungry = -NotHungry
    }
    public enum BinaryEmotion
    {
        LovedLonely,
        NotHungryHungry,
        Count
    }

    private BinaryFeeling[] emotions = new BinaryFeeling[(int)BinaryEmotion.Count];
    private Vector2[] timers = new Vector2[(int)BinaryEmotion.Count];

    private BinaryFeeling emotion(BinaryEmotion index)
    {
        return emotions[(int)index];
    }

    public InnerState()
    {
        for (int i = 0; i < (int)BinaryEmotion.Count; i++)
        {
            emotions[i] = new BinaryFeeling();
            timers[i] = new Vector2(0.0f, 0.0f);
        }
    }

    public void update()
    {
        for (int i = 0; i < (int)BinaryEmotion.Count; i++)
        {
            if (emotions[i].Value > 0.0f)
            {
                timers[i].x += Time.deltaTime;
                timers[i].y = Mathf.Clamp(timers[i].y - Time.deltaTime, 0.0f, float.MaxValue);
            }
            else if (emotions[i].Value < 0.0f)
            {
                timers[i].x = Mathf.Clamp(timers[i].x - Time.deltaTime, 0.0f, float.MaxValue);
                timers[i].y += Time.deltaTime;
            }
        }
    }

    SortedDictionary<float, Emotion> sorted_by_time = new SortedDictionary<float, Emotion>();
    public SortedDictionary<float, Emotion> get_emotions_sorted_by_duration()
    {
        sorted_by_time.Clear();
        for (int i = 0; i < (int)BinaryEmotion.Count; i++)
        {
            float time = float.NaN;
            Emotion em = Emotion.None;
            if (emotions[i].Value > 0.0f)
            {
                time = timers[i].x;
                em = (Emotion)(i + 1);
            }
            else if (emotions[i].Value < 0.0f)
            {
                time = timers[i].y;
                em = (Emotion)(-(i + 1));
            }
            else
            {
                continue;
            }
            sorted_by_time.Add(time, em);
        }
        return sorted_by_time;
    }
    SortedDictionary<float, Emotion> sorted_by_intensity = new SortedDictionary<float, Emotion>();
    public SortedDictionary<float, Emotion> get_emotions_sorted_by_intensity()
    {
        sorted_by_intensity.Clear();
        for (int i = 0; i < (int)BinaryEmotion.Count; i++)
        {
            float intensity = float.NaN;
            Emotion em = Emotion.None;
            if (emotions[i].Value > 0.0f)
            {
                intensity = emotions[i].Positive;
                em = (Emotion)(i + 1);
            }
            else if (emotions[i].Value < 0.0f)
            {
                intensity = emotions[i].Negative;
                em = (Emotion)(-(i + 1));
            }
            else
            {
                continue;
            }
            sorted_by_intensity.Add(intensity, em);
        }
        return sorted_by_intensity;
    }
    public bool feels(Emotion em)
    {
        // if the emotion is on the negative side
        bool is_negative = (int)em < 0.0f;

        // Receive the binary emotion index
        int index = Mathf.Abs(((int)em)) - 1;

        if (is_negative)
        {
            return emotion(BinaryEmotion.LovedLonely).Value < 0.0f;
        }
        else
        {
            return emotion(BinaryEmotion.LovedLonely).Value > 0.0f;
        }
    }
    public void change(Emotion em, float by)
    {
        // if the emotion is on the negative side
        bool is_negative = (int)em < 0.0f;

        // Receive the binary emotion index
        int index = Mathf.Abs(((int)em)) - 1;

        if (is_negative) {
            emotion(BinaryEmotion.LovedLonely).Negative += by;
        }
        else {
            emotion(BinaryEmotion.LovedLonely).Positive += by;
        }
    }
    public void reset_timer(Emotion em)
    {
        // if the emotion is on the negative side
        bool is_negative = (int)em < 0.0f;

        // Receive the binary emotion index
        int index = Mathf.Abs(((int)em)) - 1;

        if (is_negative)
        {
            timers[(int)index].y = 0.0f;
        }
        else
        {
            timers[(int)index].x = 0.0f;
        }
    }
    public void set(Emotion em, float by)
    {
        // if the emotion is on the negative side
        bool is_negative = (int)em < 0.0f;

        // Receive the binary emotion index
        int index = Mathf.Abs(((int)em)) - 1;

        if (is_negative)
        {
            emotion(BinaryEmotion.LovedLonely).Negative = by;
        }
        else
        {
            emotion(BinaryEmotion.LovedLonely).Positive = by;
        }
    }

    public float get(Emotion em)
    {
        // if the emotion is on the negative side
        bool is_negative = (int)em < 0.0f;

        // Receive the binary emotion index
        int index = Mathf.Abs(((int)em)) - 1;

        if (is_negative)
        {
            return emotion(BinaryEmotion.LovedLonely).Negative;
        }
        else
        {
            return emotion(BinaryEmotion.LovedLonely).Positive;
        }
    }
}

public abstract class AnimalSystem : MonoBehaviour, Commandable, Emotional
{
    public InnerState inner_state = new InnerState();

    private Collider2D[] colliders;
    private SpriteRenderer sr;
    [SerializeField] private SpriteResolver emotion_resolver;
    private Vector3 previous_position;
    private Vector3 look_dir = Vector3.left;
    private Disolve disolve;
    private struct TimestampMove
    {
        public TimestampMove(Vector3 pos, float t)
        {
            time = t;
            position = pos;
        }
        public Vector3 position;
        public float time;
    }

    private List<TimestampMove> positions = new List<TimestampMove>();

    [SerializeField] private float receive_frequency = 0.1f;
    [SerializeField] private float reaction_delay = 0.5f;
    float threshhold = 1.0f;
    float accumulator = 0.0f;
    float last_time = 0.0f;
    private bool is_adopted = false;

    public Vector3 get_last_position()
    {
        if (positions.Count - 1 < 0)
            return transform.position;
        return positions[positions.Count - 1].position;
    }
    private TimestampMove[] find_pair_at(float time)
    {
        // 0 = from; 1 = to
        TimestampMove[] to_return = new TimestampMove[2];
        float alpha = float.MaxValue;
        for (int i = 1; i < positions.Count; i++) {
            if(alpha < positions[i].time)
            {
                alpha = positions[i].time;
            }

            if (positions[i].time > time) {
                to_return[0] = positions[i - 1];
                to_return[1] = positions[i];
                return to_return;
            }
        }
        return to_return;
    }

    private void Start()
    {
        colliders = GetComponents<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        disolve = GetComponent<Disolve>();
        on_start(inner_state);
        previous_position = transform.position;
    }

    bool is_emotion_playing = false;
    int emotion_sprite_index = 0;
    string emotion_name = "Empty";
    private void OnEnable()
    {
        is_emotion_playing = true;
        StartCoroutine(play_animation());
        previous_position = transform.position;
    }
    private void OnDisable()
    {
        is_emotion_playing = false;
        set_animation(InnerState.Emotion.None);
        StopAllCoroutines();
    }
    public void disappear(GameObject context, Disolve.Callback callback = null)
    {
        disolve.disappear(context, callback);
    }
    public void appear(GameObject context, Disolve.Callback callback = null)
    {
        disolve.appear(context, callback);
    }
    public void on_adopt(AnimalShelter shelter)
    {
        on_shelter_enter(shelter, inner_state);
        is_adopted = true;
    }
    public void on_abandon(AnimalShelter shelter)
    {
        on_shelter_exit(shelter, inner_state);
        is_adopted = false;
    }
    public void update(AnimalShelter shelter)
    {
        inner_state.update();
        on_update(shelter, inner_state);
    }
    public void pat()
    {
        on_pat(inner_state);
    }
    public void feed()
    {
        on_feed(inner_state);
    }

    void Update()
    {
        if (is_adopted) {
            previous_position = transform.position;
            float my_time = Time.time - reaction_delay;
            if (positions.Count > 1)
            {
                if (positions[positions.Count - 2].time > my_time)
                {
                    var positions = find_pair_at(my_time);
                    var from = positions[0];
                    var to = positions[1];

                    float full = to.time - from.time;
                    float current = my_time - from.time;
                    float t = current / full;

                    transform.position = Vector3.Lerp(from.position, to.position, t);
                }
            }

            positions.RemoveAll((TimestampMove move) =>
            {
                return move.time < Time.time - threshhold;
            });

            if (transform.position != previous_position)
            {
                look_dir = transform.position - previous_position;
                sr.flipX = look_dir.x > 0.0f;
            }
        }
        else {
            var dir = PlayerSystem.Main.transform.position.x - transform.position.x;
            sr.flipX = dir > 0.0f;
            on_waiting(inner_state);
        }

        on_animation(transform.position - previous_position);

    }
    public void set_collidable(bool to)
    {
        foreach(var collider in colliders)
        {
            collider.enabled = to;
        }
    }
    public void on_move(Vector3 position)
    {
        accumulator += (Time.time - last_time);

        last_time = Time.time;
        if (accumulator < receive_frequency)
            return;

        accumulator -= receive_frequency;
        var pos = transform.position;
        pos.x = position.x;
        positions.Add(new TimestampMove(pos, Time.time));
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            collision.GetComponent<CaringState>().on_meeting(this);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            collision.GetComponent<CaringState>().on_leaving(this);
    }
    
    public void set_animation(InnerState.Emotion emotion)
    {
        timer = 1.5f;
        switch (emotion)
        {
            case InnerState.Emotion.None:
                emotion_name = "Empty";
                break;
            case InnerState.Emotion.Loved:
                emotion_name = "Love";
                break;
            case InnerState.Emotion.Lonely:
                emotion_name = "Disappointed";
                break;
            case InnerState.Emotion.NotHungry:
                emotion_name = "Angry";
                break;
            case InnerState.Emotion.Hungry:
                emotion_name = "Fear";
                break;
        }
    }

    float timer = 0.0f;
    public IEnumerator play_animation()
    {
        while(is_emotion_playing)
        {
            emotion_resolver.SetCategoryAndLabel(emotion_name, emotion_sprite_index.ToString());
            yield return new WaitForSeconds(0.2f);
            emotion_sprite_index = (emotion_sprite_index + 1) % 2;

            timer -= 0.2f;
            if(timer < 0.0f)
            {
                emotion_name = "Empty";
            }
        }
    }

    



    public abstract void on_start(InnerState state);
    public abstract void on_shelter_enter(AnimalShelter shelter, InnerState state);
    public abstract void on_shelter_exit(AnimalShelter shelter, InnerState state);
    public abstract void on_update(AnimalShelter shelter, InnerState state);
    public abstract void on_pat(InnerState state);
    public abstract void on_feed(InnerState state);
    public abstract void on_waiting(InnerState state);
    public abstract void on_animation(Vector3 velocity);
}
