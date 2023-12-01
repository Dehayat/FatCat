using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public PlayableDirector dir;
    public Cat cat;
    public GameObject menu;
    public GameObject hud;
    public GameObject loseMenu;
    public GameObject winMenu;
    public Fish fish;
    public AudioSource bgm;

    enum gameState
    {
        cutscene,
        menu,
        game,
        lose,
        win,
        credits
    }

    private gameState state;

    private void Start()
    {
        cat.onLose.AddListener(LoseGame);
        fish.onWin.AddListener(WinGame);
    }

    private void WinGame()
    {
        if (state == gameState.game)
        {
            StartCoroutine(WinSequence());
        }
    }
    IEnumerator WinSequence()
    {
        cat.SetInvincible();
        cat.EnableControls(false);
        cat.Win();
        yield return new WaitForSeconds(2);
        winMenu.SetActive(true);
        state = gameState.lose;
        yield return new WaitForSeconds(2);
    }

    private void LoseGame()
    {
        if (state == gameState.game)
        {
            StartCoroutine(LoseGameSequence());
        }
    }
    IEnumerator LoseGameSequence()
    {
        cat.EnableControls(false);
        loseMenu.SetActive(true);
        state = gameState.lose;
        yield return null;
    }

    private void Update()
    {
        switch (state)
        {
            case gameState.cutscene:
                CutSceneState();
                break;
            case gameState.menu:
                MenuState();
                break;
            case gameState.game:
                break;
            case gameState.lose:
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    SceneManager.LoadScene(0);
                }
                break;
            case gameState.win:
                break;
            case gameState.credits:
                break;
            default:
                break;
        }
    }

    private void MenuState()
    {
        Vector2 move = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (move.sqrMagnitude > 0.1f)
        {
            menu.SetActive(false);
            hud.SetActive(true);
            state = gameState.game;
        }
    }

    private void CutSceneState()
    {
        if (dir.state != PlayState.Playing)
        {
            state = gameState.menu;
            bgm.Play();
            menu.SetActive(true);
            cat.EnableControls(true);
        }
    }
}
