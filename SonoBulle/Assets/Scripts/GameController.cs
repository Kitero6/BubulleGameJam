using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    enum EGameState
    {
        None,
        Won,
        Lost
    }
    private AudioSource _audioSource = null;
    private PlayerController _player = null;
    private List<CellController> _cells = null;
    private EGameState _state = EGameState.None;
    public Image _image = null;
    public float _fadeSpeed = 0f;
    public string _winScene = "";
    public string _gameOverScene = "";
    public AudioClipSound _winSound = null;
    public AudioClipSound _gameOverSound = null;


    // Start is called before the first frame update
    void Start()
    {
        _state = EGameState.None;

        _player = FindObjectOfType<PlayerController>();
        _audioSource = GetComponent<AudioSource>();

        _cells = new List<CellController>();
        _cells.AddRange(FindObjectsOfType<CellController>());

        _player._canMove = false;

        StartCoroutine("StartScene");
    }

    // Update is called once per frame
    void Update()
    {
        if (_player.IsDead)
        {
            _state = EGameState.Lost;
            StartCoroutine("StartEndScene");
        }

        if (_cells.Count == 0 || _state != EGameState.None)
            return;

        bool isWon = true;
        foreach (CellController cell in _cells)
        {
            if (cell.HealNeeded > 0)
            {
                isWon = false;
                break;
            }
        }

        if (isWon)
        {
            _state = EGameState.Won;
            StartCoroutine("StartEndScene");
        }
    }

    IEnumerator StartScene()
    {
        _image.color = new Color(0f, 0f, 0f, 1f);

        while (_image.color.a > 0f)
        {
            Color newCol = _image.color;
            newCol.a -= _fadeSpeed * Time.deltaTime;
            _image.color = newCol;

            yield return 0;
        }

        _player._canMove = true;
    }

    IEnumerator StartEndScene()
    {
        while (_image.color.a < 1f)
        {
            Color newCol = _image.color;
            newCol.a += _fadeSpeed * Time.deltaTime;
            _image.color = newCol;
            yield return 0;
        }

        yield return new WaitForSeconds(0.5f);

        GlobalValues.LastScene = SceneManager.GetActiveScene().name;
        if (_state == EGameState.Won)
        {
            _winSound.PlayToSource(_audioSource);
            SceneManager.LoadScene(_winScene);
        }
        else if (_state == EGameState.Lost)
        {
            _gameOverSound.PlayToSource(_audioSource);
            SceneManager.LoadScene(_gameOverScene);
        }

    }
}
