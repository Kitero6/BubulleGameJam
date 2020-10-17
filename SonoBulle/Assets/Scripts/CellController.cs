using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour
{
    private int _currHeal = 0;
    public int _healNeeded = 0;

    // Start is called before the first frame update
    void Start()
    {
        _currHeal = 0;
    }

    public void Heal()
    {
        _currHeal++;
        
        if (_currHeal >= _healNeeded)
        {
            Destroy(gameObject);
        }
    }
}
