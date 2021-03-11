using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feed : MenuEntry
{
    public override void use(AnimalSystem selection, int index)
    {
        selection.feed();
    }
}
