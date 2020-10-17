using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medic : MonoBehaviour
{
    enum EMedicState
    {
        Follower,
        Heal
    }

    public GameObject _player = null;

    public float _distanceToKeepFromPlayer = 0.0f;
    public float _speed = 0.0f;

    // Update is called once per frame
    void Update()
    {
        
    }
}
