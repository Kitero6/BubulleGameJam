using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Properties
    private Rigidbody2D _rigidBody = null;
    private AudioSource _generalAudioSource = null;
    private AudioSource _vibrateAudioSource = null;
    private Animator _animator = null;

    #region Health
    private Vector3 _lastPos = Vector3.zero;
    private float _currHealth = 0f;
    public float _maxHealth = 0f;
    public float _lostHealthPerMeter = 0f;
    public float _lostHealthOnCollision = 0f;
    #endregion

    #region Grab
    [Header("Grab", order=0)]
    private bool _isGrabbing = false;
    private Vector2 _posOnClick = Vector2.zero;
    public float _maxGrabDistance = 0.0f; 
    public float _grabStrength = 0.0f;
    public Transform _arrowPivot = null;
    public GameObject _goOnClick = null;
    public float _arrowMinSize = 0.0f;
    public float _arrowMaxSize = 0.0f;
    private bool _hasGrabSoundTriggered = false;
    [Range(0f, 1f)] public float _percentTriggerGrabSound = 0.0f;
    #endregion

    #region Vibration
    [Header("Vibration", order=1)]
    private bool _isVibrating = false;
    private float _currVibration = 0f;
    public float _vibrationSpeed = 0f;
    public float _vibrationReleaseSpeed = 0f;
    [Range(0f, 1f)] public float _vibrationToOpenCell = 0f;
    public float _vibrationAnimationMinSpeed = 0f;
    public float _vibrationAnimationMaxSpeed = 0f;
    #endregion

    #region Medic
    [Header("Medic", order=2)]
    private List<MedicController> _medics = null;
    public GameObject _medicGO = null;
    public int _numMedic = 0;
    public float _distToKeepFromPlayer = 0.0f;
    public float _randomDistFromPlayer = 0.0f;
    public float _medicSpeed = 0.0f;
    public float _medicRandomSpeed = 0.0f;
    public float _medicRotSpeed = 0.0f;
    public float _medicRandomRotSpeed = 0.0f;
    #endregion

    #region Cells
    private List<CellController> _cellsInRange = null;
    private Dictionary<CellController, List<MedicController>> _cellsHealing = null;
    public LayerMask _cellMask = 0;
    #endregion

    #region Audios
    [Header("Audios", order=3)]
    public AudioClipSound _onGrabAudio = null;
    public AudioClipSound _onShmolReleaseAudio = null;
    public AudioClipSound _onBigReleaseAudio = null;
    public AudioClipSound _onCollisionAudio = null;
    public AudioClipSound _vibrationSound = null;
    #endregion
    #endregion

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        AudioSource[] audioSources = GetComponents<AudioSource>();
        _generalAudioSource = audioSources[0];
        _vibrateAudioSource = audioSources[1];

        _medics       = new List<MedicController>();
        _cellsInRange = new List<CellController>();
        _cellsHealing = new Dictionary<CellController, List<MedicController>>();
        
        _currHealth = _maxHealth;

        SpawnAllMedics();
    }

    void SpawnAllMedics()
    {
        for (int i = 0; i <_numMedic; ++i)
        {
            float dist = _distToKeepFromPlayer + Random.Range(-_randomDistFromPlayer, _randomDistFromPlayer);
            float angle = Random.Range(0f, 360f);

            Quaternion q = Quaternion.Euler(0f, 0f, angle);
            Vector3 direction = q * Vector3.right;
            Vector3 posSpawn = transform.position + direction * dist;

            GameObject go = Instantiate(_medicGO, posSpawn, Quaternion.identity);
            MedicController medic = go.GetComponent<MedicController>();
            if (medic)
            {
                medic.Player = transform;
                medic.DistToKeepFromPlayer = dist;
                medic.Speed = _medicSpeed + Random.Range(-_medicRandomSpeed, _medicRandomSpeed);
                medic.CurrAngle = angle;
                medic.RotSpeed = (_medicRotSpeed + Random.Range(-_medicRandomRotSpeed, _medicRandomRotSpeed)) * (Mathf.RoundToInt(Random.Range(0f, 1f)) == 0 ? -1f : 1f);

                _medics.Add(medic);
            }
        }
    }

    void Update()
    {
        UpdateGrab();
        UpdateVibration();

        UpdateHealthLoss();
    }

    void UpdateHealthLoss()
    {
        float distFromLastFrame = (transform.position - _lastPos).magnitude;

        _currHealth -= distFromLastFrame * _lostHealthPerMeter;

        _lastPos = transform.position;

        if (_currHealth <= 0f)
        {

        }
    }

    #region Grab
    void UpdateGrab()
    {
        if (_isVibrating && !_isGrabbing)
            return;

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

        _hasGrabSoundTriggered = false;
        _isGrabbing = true;
    }

    void UpdateGrabbing()
    {
        // Get the direction
        Vector2 direction = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - _posOnClick;
        direction = -direction;

        float grabRatio = Mathf.Clamp(direction.magnitude / _maxGrabDistance, 0f, 1f);
        
        // Set the arrow scale
        {
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

        // If we passed the threshold to trigger the sound
        if (!_hasGrabSoundTriggered && grabRatio > _percentTriggerGrabSound)
        {
            _onGrabAudio.PlayToSource(_generalAudioSource);
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

        // Play audio depending on the strength
        float grabRatio = Mathf.Clamp(direction.magnitude / _maxGrabDistance, 0f, 1f);
        if (grabRatio > _percentTriggerGrabSound)
            _onBigReleaseAudio.PlayToSource(_generalAudioSource);
        else
            _onShmolReleaseAudio.PlayToSource(_generalAudioSource);

        _isGrabbing = false;
    }
    #endregion

    #region Vibration
    void UpdateVibration()
    {
        UpdateVibrating();

        if (!_isGrabbing || _isVibrating)
        {
            // Right click down
            if (Input.GetMouseButtonDown(1))
            {
                StartVibration();
            }
            else if (Input.GetMouseButtonUp(1))
            {
                ReleaseVibration();
            }
        }
    }

    void StartVibration()
    {
        _isVibrating = true;

    }

    void UpdateVibrating()
    {
        // Update the vibration
        _currVibration += Time.deltaTime * (_isVibrating ? _vibrationSpeed : -_vibrationReleaseSpeed);
        _currVibration = Mathf.Clamp01(_currVibration);

        _animator.SetFloat("vibration", _currVibration);
        if (_currVibration > 0.1f)
            _animator.speed = Mathf.Lerp(_vibrationAnimationMinSpeed, _vibrationAnimationMaxSpeed, _currVibration);
        else
            _animator.speed = 1f;


        SetVibrationToCells();
        PlayVibrationSounds();

        // If we have to send the medics to the cell
        if (_currVibration >= _vibrationToOpenCell)
        {
            StartCellHeal();
        }
        else
        {
            StopCellHeal();
        }
    }

    void SetVibrationToCells()
    {
        foreach (CellController cell in _cellsInRange)
        {
            cell.Vibration = _currVibration;
        }
    }

    void PlayVibrationSounds()
    {
        if (!_vibrateAudioSource.isPlaying && _currVibration > 0.1f)
        {
            _vibrationSound.PlayToSource(_vibrateAudioSource);
        }

        else if (_vibrateAudioSource.isPlaying)
        {
            if (_currVibration < 0.1f)
                _vibrateAudioSource.Stop();
            else
                _vibrateAudioSource.volume = _currVibration;
        }
    }

    void StartCellHeal()
    {
        // For all the cells in range
        foreach (CellController cell in _cellsInRange)
        {
            if (!_cellsHealing.ContainsKey(cell))
                SendHealers(cell);
        }
    }

    void SendHealers(CellController cell)
    {
        // Create the array of senders
        List<MedicController> healers = new List<MedicController>();

        // Send the right amount of healers needed
        for (int i = 0; i < cell.HealNeeded; ++i)
        {
            if (_medics.Count == 0)
                return;

            int j = Mathf.RoundToInt(Random.Range(0, _medics.Count - 1));
            MedicController currMedic = _medics[j];

            currMedic.Player = cell.transform;
            currMedic.State = MedicController.EMedicState.Heal;

            _medics.Remove(currMedic);
            healers.Add(currMedic);
        }

        _cellsHealing.Add(cell, healers);
    }

    void StopCellHeal()
    {
        foreach (List<MedicController> medics in _cellsHealing.Values)
        {
            foreach (MedicController medic in medics)
            {
                medic.Player = transform;
                medic.State = MedicController.EMedicState.Follower;
                
                _medics.Add(medic);
            }
        }

        _cellsHealing.Clear();
    }

    CellController GetClosestCell()
    {
        float lowestDist = Mathf.Infinity;
        CellController closest = null;

        foreach (CellController cell in _cellsInRange)
        {
            float dist = (transform.position - cell.transform.position).sqrMagnitude;

            if (lowestDist > dist)
            {
                lowestDist = dist;
                closest = cell;
            }
        }

        return closest;
    }

    void ReleaseVibration()
    {
        _isVibrating = false;
    }
    #endregion

    void OnCollisionEnter2D(Collision2D other)
    {
        _onCollisionAudio.PlayToSource(_generalAudioSource);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Colliding with a cell
        if (((1 << other.gameObject.layer) & _cellMask) != 0)
        {
            CellController cell = other.gameObject.GetComponent<CellController>();
            if (cell) 
                _cellsInRange.Add(cell);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Colliding with a cell
        if (((1 << other.gameObject.layer) & _cellMask) != 0)
        {
            CellController cell = other.gameObject.GetComponent<CellController>();
            if (cell) 
            {
                cell.Vibration = 0f;
                _cellsInRange.Remove(cell);
            }
        }
    }
}
