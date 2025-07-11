
using System;
using System.Collections;
using DefaultNamespace;
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
    [SerializeField] 
    private GameObject killedFeedParent;
    [SerializeField]
    private TextMeshProUGUI killFeedPrefab;

    private PlayerControllerNB localPlayer;

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
        if (localPlayer.team.Value == GameManager.Teams.Blue)
        {
            blueTeamScoreSlider.color = Color.green;
            redTeamScoreSlider.color = Color.red;
        }
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

    public void UpdateKillFeed(string deadName, string shooterName, GameManager.Teams deadTeam, GameManager.Teams shooterTeam)
    {
        TextMeshProUGUI killFeedText = Instantiate(killFeedPrefab, killedFeedParent.transform);
        PlayerSessionNB session = localPlayer.PlayerSession;
        if(session.Team.Value == deadTeam && session.Team.Value != GameManager.Teams.None)
            killFeedText.text = $"<color=red> {shooterName}</color> killed <color=green> {deadName} </color>";
        else
            killFeedText.text = $"<color=green> {shooterName}</color> killed <color=red> {deadName} </color>";
        StartCoroutine(DisappearText(killFeedText.gameObject));

    }

    private IEnumerator DisappearText(GameObject text)
    {
        yield return new WaitForSeconds(3f);
        Destroy(text);
    }

    public void ActivateScoresUI()
    {
        scoreCanvas.SetActive(true);
    }
}
