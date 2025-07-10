
using System.Collections;
using FishNet.Object;
using TMPro;
using UnityEngine;

public class StartEndScene : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    public override void OnStartServer()
    {
        base.OnStartServer();
        string teamwontext = "";
        if (KeepTrackScores.teamWon == 0)
        {
            teamwontext = "Vex";
        } else if (KeepTrackScores.teamWon == 1)
        {
            teamwontext = "Bolt";
        }
        StartCoroutine(DelayedShowMessage(teamwontext));
    }

    private IEnumerator DelayedShowMessage(string teamwon)
    {
        yield return new WaitForSeconds(0.5f);
        ShowMessage(teamwon);
    }

    [ObserversRpc]
    private void ShowMessage(string teamwon)
    {
        if (text != null)
            text.text = $"AND THE WINNER IS TEAM {teamwon}";
        else
            Debug.LogWarning("text is missing");
    }
}
