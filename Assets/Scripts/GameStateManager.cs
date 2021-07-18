using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        WindowManager.Instance.Initialization();
    }
}
