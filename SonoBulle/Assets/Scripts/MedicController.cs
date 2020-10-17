using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicController : MonoBehaviour
{
    public enum EMedicState
    {
        Follower,
        Heal
    }

    private EMedicState _state = EMedicState.Follower;
    private float _distToKeepFromPlayer = 0.0f;
    private float _currAngle = 0.0f;

    private Transform _player = null;

    private float _speed = 0.0f;
    private float _rotSpeed = 0.0f;
    public float _speedToHeal = 0f;
    public LayerMask _cellMask = 0;

    public float DistToKeepFromPlayer { get => _distToKeepFromPlayer; set => _distToKeepFromPlayer = value; }
    public float CurrAngle { get => _currAngle; set => _currAngle = value; }
    public Transform Player { get => _player; set => _player = value; }
    public float Speed { get => _speed; set => _speed = value; }
    public float RotSpeed { get => _rotSpeed; set => _rotSpeed = value; }
    public EMedicState State { get => _state; set => _state = value; }

    // Update is called once per frame
    void Update()
    {
        switch (_state)
        {
            case EMedicState.Follower:  UpdateFollow(); break;
            case EMedicState.Heal:      UpdateHeal(); break;
        }
    }

    void UpdateFollow()
    {
        // Update the angle
        _currAngle += _rotSpeed * Time.deltaTime;

        // Get the position to reach depending on the  rotaion of the medic
        Quaternion q = Quaternion.Euler(0f, 0f, _currAngle);
        Vector3 direction = q * Vector3.right;
        Vector3 positionToReach = _player.position + direction * _distToKeepFromPlayer;

        // Move toward the position
        transform.position = Vector3.Lerp(transform.position, positionToReach, _speed); 
    }

    void UpdateHeal()
    {
        transform.position = Vector3.Lerp(transform.position, _player.position, _speedToHeal); 
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // Colliding with a cell
        if (_state == EMedicState.Heal && ((1 << other.gameObject.layer) & _cellMask) != 0)
        {
            CellController cell = other.gameObject.GetComponent<CellController>();
            if (cell) 
            {
                cell.Heal();
                Destroy(gameObject);
            }
        }
    }
}
