using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New_Load_Actions", menuName = "Utility/Loaded Actions")]
public class LoadedActions_SO : ScriptableObject
{
    public bool
        cac,
        distance,
        defense,
        grapling; 

    public bool CacIsLoaded { get; set; }
    public bool DistanceIsLoaded { get; set; }
    public bool DefenseIsLoaded { get; set; }
    public bool GraplingIsLoaded { get; set; }
}
