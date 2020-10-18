using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    private PlayerController _player = null;
    public Slider _healthBar = null;

    void Start()
    {
        _player = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        _healthBar.value = _player.CurrHealth / _player._maxHealth;
    }
}
