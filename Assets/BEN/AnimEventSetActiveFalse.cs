using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventSetActiveFalse : MonoBehaviour
{
    [SerializeField] private List<GameObject> _listToSetFalse = new List<GameObject>();
    private float _delay; 

    public void SetAllObjFalse(float delay)
    {
        _delay = delay;
        StartCoroutine(nameof(SetFalse)); 
    }

    private IEnumerator SetFalse()
    {
        yield return new WaitForSeconds(_delay);
        foreach (var obj in _listToSetFalse)
        {
            obj.SetActive(false);
        }
    }
}
