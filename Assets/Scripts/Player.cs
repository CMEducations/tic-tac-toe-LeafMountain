using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public virtual Vector2Int? PickPosition()
    {
        return null;
    }

    private void Update()
    {
        // Get picked tile
        // Send to statemachine
    }
}
