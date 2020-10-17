using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rigidBody = null;
    private AudioSource _audioSource = null;

    private Vector2 _posOnClick = Vector2.zero;

    [Header("Grab")]
    public float _maxGrabDistance = 0.0f; 
    public float _grabStrength = 0.0f;
    public Transform _arrowPivot = null;
    public GameObject _goOnClick = null;
    public float _arrowMinSize = 0.0f;
    public float _arrowMaxSize = 0.0f;

    [Header("Audios")]
    public AudioClip _onGrabAudio = null;
    public AudioClip _onReleaseAudio = null;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        UpdateGrab();
    }

    void UpdateGrab()
    {
        // Left click down
        if (Input.GetMouseButton(0))
        {
            // Start the grab sequence if just started clicking
            if (Input.GetMouseButtonDown(0))
                StartGrab();

            UpdateGrabbing();
        }
        // Release click
        else if (Input.GetMouseButtonUp(0))
        {
            ReleaseGrab();
        }
    }

    void StartGrab()
    {
        // Get the position of the click
        _posOnClick = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        
        // Set the gameObject to see where we clicked
        _goOnClick.transform.position = _posOnClick;
        _goOnClick.SetActive(true);

        // Activate the arrow
        _arrowPivot.gameObject.SetActive(true);

        // Play Grab sound
        _audioSource.clip = _onGrabAudio;
        _audioSource.Play();
    }

    void UpdateGrabbing()
    {
        // Get the direction
        Vector2 direction = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - _posOnClick;
        direction = -direction;

        // Set the arrow scale
        {
            float grabRatio = Mathf.Clamp(direction.magnitude / _maxGrabDistance, 0f, 1f);
            float arrowSize = Mathf.Lerp(_arrowMinSize, _arrowMaxSize, grabRatio);

            Vector3 arrowScale = _arrowPivot.transform.localScale;
            arrowScale.x = arrowSize;
            _arrowPivot.transform.localScale = arrowScale;
        }

        // Set the direction of the arrow
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _arrowPivot.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void ReleaseGrab()
    {
        _arrowPivot.gameObject.SetActive(false);
        _goOnClick.SetActive(false);

        // Get the direction to push the bubble
        Vector2 direction = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - _posOnClick;
        if (direction.magnitude > _maxGrabDistance)
        {
            direction = direction.normalized * _maxGrabDistance;
        }
        direction = -direction;

        Vector2 force = direction * _grabStrength;
        _rigidBody.AddForce(force);

        // Play audio
        _audioSource.clip = _onReleaseAudio;
        _audioSource.Play();
    }
}
