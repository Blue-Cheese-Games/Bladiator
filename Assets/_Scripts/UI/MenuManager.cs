using System.Collections;
using System.Collections.Generic;
using Bladiator.Managers;
using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
	[SerializeField] private GameObject m_Menu;
	[SerializeField] private GameObject[] m_HideObjects;


	public void Start()
	{
		GameManager.Instance.OnGameStateChange += OnGameStateChange;
		GameManager.Instance.ResetEvent += ResetEvent;
		InitializeMenu();
	}

	private void ResetEvent()
	{
		m_Menu.SetActive(GameManager.Instance.GameState == GameState.MainMenu ||
		                 GameManager.Instance.GameState == GameState.Pause);
		DisableObjects();
	}

	private void InitializeMenu()
	{
		m_Menu.SetActive(GameManager.Instance.GameState == GameState.MainMenu ||
		                 GameManager.Instance.GameState == GameState.Pause);
		DisableObjects();
	}

	private void OnGameStateChange(GameState state)
	{
		m_Menu.SetActive(state == GameState.MainMenu || state == GameState.Pause);
		DisableObjects();
	}

	private void DisableObjects()
	{
		foreach (GameObject hideObject in m_HideObjects)
		{
			hideObject.SetActive(GameManager.Instance.GameState != GameState.MainMenu && GameManager.Instance.GameState != GameState.Pause);
		}
	}

	public void StartGame()
	{
		if (GameManager.Instance.GameState == GameState.MainMenu)
		{
			m_Menu.SetActive(false);
			GameManager.Instance.StartGame();
		}
		else if (GameManager.Instance.GameState == GameState.Pause)
		{
			GameManager.Instance.PauseGame();
		}
	}

	public void ResumeGame()
	{
		GameManager.Instance.PauseGame();
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}