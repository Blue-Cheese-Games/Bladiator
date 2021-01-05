using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [SerializeField] private TextMeshProUGUI m_scoreText;

    private int m_Score;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public int GetScore()
    {
        return m_Score;
    }

    public void AddScore(int scoreToAdd)
    {
        m_Score += scoreToAdd;
        UpdateText();
    }

    public void UpdateText()
    {
        m_scoreText.text = m_Score.ToString("00");
    }
}