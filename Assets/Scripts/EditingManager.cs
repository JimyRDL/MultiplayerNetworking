
using System;
using UnityEngine;

public class EditingManager : MonoBehaviour
{
    public static EditingManager instance {get; private set;}

    public bool Editing;
    private void Awake()
    {
        instance = this;
    }
    
}
