﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{



    [SerializeField]
    private Text _scoreText;

    [SerializeField]
    private Text _gameOverText;

    [SerializeField]
    private Text _restartLevel;

    [SerializeField]
    private Image _livesImg;

    [SerializeField]
    private Sprite[] _livesSprite;

    [SerializeField]
    private GameObject _gameManager;

    [SerializeField]
    private Camera _mainCamera;


    [Header("Thrust UI")]
    [SerializeField]
    private Text _thrustText;

    [SerializeField]
    private Slider _barThrust;

    [Header("Ammo Refill")]
    [SerializeField]
    private Text _ammoText;


    //Private Variables
    private GameManager _gm;
    private Vector3 _cameraInitial;

    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _livesImg.sprite = _livesSprite[_livesSprite.Length-1];
        _gameOverText.gameObject.SetActive(false);
        _restartLevel.gameObject.SetActive(false);

        if(_gameManager)
        {
            _gm = _gameManager.GetComponent < GameManager >();
        }

        if(_mainCamera)
        {
            _cameraInitial = _mainCamera.transform.position;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateScore(int score)
    {
        _scoreText.text = "Score: " + score;
    }

    public void UpdateLives(int currentLives)
    {

        //_livesImg.sprite = _livesSprite[currentLives];
        if(currentLives >= 0)
        {
            _livesImg.sprite = _livesSprite[currentLives];
        }
        if (currentLives <= 0)
        {
            GameOverSequence();
        }

    }

    public void ThrustUI(float x)
    {
        _barThrust.value = (x);
    }

    public void ThrustText(string t)
    {
        _thrustText.text = t;
    }

    public void AmmoText(int i)
    {
        _ammoText.text = i.ToString();
    }


    void GameOverSequence()
    {
        _restartLevel.gameObject.SetActive(true);
        _gm.GameOver();
        StartCoroutine(GameOver());
    }

    public void CameraShake()
    {
        StartCoroutine(PlayerDamage());
    }


    IEnumerator GameOver()
    {
        while (true)
        {
            _gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator PlayerDamage()
    {
        Vector3 _camPos = _mainCamera.transform.position;
        _camPos.x = Random.Range(-0.1f, 0.1f);
        _camPos.y = Random.Range(-0.1f, 0.1f);
        _mainCamera.transform.position = _camPos;
        yield return new WaitForSeconds(0.2f);
        _mainCamera.transform.position = _cameraInitial;
    }
}
