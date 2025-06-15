
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Scene References")] 
    [SerializeField]
    private GameObject scoreCanvas;
    [SerializeField]
    private TextMeshProUGUI blueTeamScoreText;
    [SerializeField]
    private TextMeshProUGUI redTeamScoreText;
    [SerializeField]
    private Image blueTeamScoreSlider;
    [SerializeField]
    private Image redTeamScoreSlider;

    private PlayerTeamManager localPlayer;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    public void SetLocalPlayer(PlayerTeamManager player)
    {
        localPlayer = player;
        UpdateTeamColors();
    }

    private void UpdateTeamColors()
    {
        Debug.Log($"Updating Team Colors and localPlayer {localPlayer} and team {localPlayer.team.Value}");
        if (localPlayer.team.Value == GameManager.Teams.Blue)
        {
            blueTeamScoreSlider.color = Color.green;
            redTeamScoreSlider.color = Color.red;
        }
        else if(localPlayer.team.Value == GameManager.Teams.Red)
        {
            blueTeamScoreSlider.color = Color.red;
            redTeamScoreSlider.color = Color.green;
        }
    }

    public void UpdateScoresUI(int blueTeamScore, int redTeamScore, int maxTeamScore)
    {
        blueTeamScoreText.text = blueTeamScore.ToString();
        redTeamScoreText.text = redTeamScore.ToString();
        blueTeamScoreSlider.fillAmount = blueTeamScore / (float)maxTeamScore;
        redTeamScoreSlider.fillAmount = redTeamScore / (float)maxTeamScore;
    }

    public void ActivateScoresUI()
    {
        scoreCanvas.SetActive(true);
    }
}
