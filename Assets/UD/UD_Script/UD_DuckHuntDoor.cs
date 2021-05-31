using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_DuckHuntDoor : MonoBehaviour
{
    public ParticleSystem destructionParticles;
    public GameObject door;

    public void OpenDoor()
    {
        destructionParticles.Play();
        door.SetActive(false);
    }
}
