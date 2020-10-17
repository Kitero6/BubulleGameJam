using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour
{
    public Sprite _healedSprite = null;
    private float _baseRotation = 0f;

    private int _currHeal = 0;
    private float _currRotation = 0f;
    private float _vibrationStrength = 0;
    private int _vibrationDir = 0;

    public float _vibrationAngle = 0f;
    public float _vibrationMaxSpeed = 0f;
    public int _healNeeded = 0;

    public float Vibration { get => _vibrationStrength; set => _vibrationStrength = value; }
    public int HealNeeded { get => _healNeeded - _currHeal; }

    // Start is called before the first frame update
    void Start()
    {
        _vibrationDir = Random.Range(0, 2) == 0 ? -1 : 1;
        _currHeal = 0;

        Vector3 forward = transform.forward;
        _baseRotation = _currRotation = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
    }

    void Update()
    {
        _currRotation += _vibrationMaxSpeed * _vibrationStrength * _vibrationDir * Time.deltaTime;

        if (_currRotation > _baseRotation + _vibrationAngle)
            _vibrationDir = -1;
        else if (_currRotation < _baseRotation - _vibrationAngle)
            _vibrationDir = 1;

        transform.rotation = Quaternion.Euler(0f, 0f, _currRotation);
    }

    public void Heal()
    {
        _currHeal++;

        if (_currHeal >= _healNeeded)
        {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            renderer.sprite = _healedSprite;
        }
    }
}
