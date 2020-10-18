using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinMenu : MonoBehaviour
{
    private bool _canLeave = false;
    public Image _image = null;
    public string _nextScene = "";
    public float _fadeSpeed = 0f;

    // Start is called before the first frame update
    void Start()
    {
        _canLeave = false;
        StartCoroutine("StartScene");
    }

    // Update is called once per frame
    void Update()
    {
        if (_canLeave && Input.anyKeyDown)
            StartCoroutine("StartEndScene");
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

        _canLeave = true;
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
