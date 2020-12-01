using System.Collections;
using System.Collections.Generic;
using Bladiator.Managers;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
	[SerializeField] private GameObject m_Menu, m_Pause;

	public void Start()
	{
		GameManager.Instance.OnGameStateChange += OnGameStateChange;
		GameManager.Instance.ResetEvent += ResetEvent;
		InitializeMenu();
	}

	private void ResetEvent()
	{
		m_Menu.SetActive(GameManager.Instance.GameState == GameState.MainMenu);
		m_Pause.SetActive(GameManager.Instance.GameState == GameState.Pause);
	}

	private void InitializeMenu()
	{
		m_Menu.SetActive(GameManager.Instance.GameState == GameState.MainMenu);
		m_Pause.SetActive(GameManager.Instance.GameState == GameState.Pause);
	}

	private void OnGameStateChange(GameState state)
	{
		m_Menu.SetActive(state == GameState.MainMenu);
		m_Pause.SetActive(state == GameState.Pause);
	}

	public void StartGame()
	{
		GameManager.Instance.StartGame();
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}