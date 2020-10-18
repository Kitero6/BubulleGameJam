using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    private string _nextScene = "";
    public Image _image = null;
    public Button _retry = null;
    public Button _mainMenu = null;
    public string _mainMenuScene = "";
    public float _fadeSpeed = 0f;

    // Start is called before the first frame update
    void Start()
    {
        _retry.onClick.AddListener(OnRetry);
        _mainMenu.onClick.AddListener(OnMainMenu);

        SetButtonEnabled(false);
        StartCoroutine("StartScene");
    }

    void OnRetry()
    {
        _nextScene = GlobalValues.LastScene;
        SetButtonEnabled(false);
        StartCoroutine("StartEndScene");
    }

    void OnMainMenu()
    {
        _nextScene = _mainMenuScene;
        SetButtonEnabled(false);
        StartCoroutine("StartEndScene");
    }

    void SetButtonEnabled(bool flag)
    {
        _retry.interactable = flag;
        _mainMenu.interactable = flag;
    }

    IEnumerator StartScene()
    {
        _image.color = new Color(0f, 0f, 0f, 1f);

        while (_image.color.a > 0f)
        {
            Color newCol = _image.color;
            newCol.a -= _fadeSpeed;
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
            newCol.a += _fadeSpeed;
            _image.color = newCol;
            yield return 0;
        }

        SceneManager.LoadScene(_nextScene);
    }
}
