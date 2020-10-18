using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public Image _image = null;
    public Button _play = null;
    public Button _quit = null;
    public string _playScene = "";
    public float _fadeSpeed = 0f;

    // Start is called before the first frame update
    void Start()
    {
        _play.onClick.AddListener(OnPlay);
        _quit.onClick.AddListener(OnQuit);

        SetButtonEnabled(false);
        StartCoroutine("StartScene");
    }

    void SetButtonEnabled(bool flag)
    {
        _play.interactable = flag;
        _quit.interactable = flag;
    }

    void OnPlay()
    {
        SceneManager.LoadScene(_playScene);
        SetButtonEnabled(false);
        StartCoroutine("StartEndScene");
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

        SetButtonEnabled(true);
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

        SceneManager.LoadScene(_playScene);
    }

    void OnQuit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif

        #if UNITY_STANDALONE
            Application.Quit();
        #endif
    }
}
