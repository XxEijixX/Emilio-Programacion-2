using Fusion;
using TMPro;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [Networked] private int ScorePlayerOne { get; set; }
    [Networked] private int ScorePlayerTwo { get; set; }
    [Networked] private int ScorePlayerThree { get; set; }
    [Networked] private int ScorePlayerFour { get; set; }

    [Header("Scores Text")]
    [SerializeField] private TextMeshProUGUI scorePlayerOneText;
    [SerializeField] private TextMeshProUGUI scorePlayerTwoText;
    [SerializeField] private TextMeshProUGUI scorePlayerThreeText;
    [SerializeField] private TextMeshProUGUI scorePlayerFourText;

    [Header("Final Message")]
    [SerializeField] private TextMeshProUGUI finalMessageText;

    public override void Spawned()
    {
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        if (scorePlayerOneText != null)
            scorePlayerOneText.text = ScorePlayerOne.ToString();

        if (scorePlayerTwoText != null)
            scorePlayerTwoText.text = ScorePlayerTwo.ToString();

        if (scorePlayerThreeText != null)
            scorePlayerThreeText.text = ScorePlayerThree.ToString();

        if (scorePlayerFourText != null)
            scorePlayerFourText.text = ScorePlayerFour.ToString();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcScorePlayerOne()
    {
        if (Object.HasStateAuthority) ScorePlayerOne++;
        UpdateScoreDisplay();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcScorePlayerTwo()
    {
        if (Object.HasStateAuthority) ScorePlayerTwo++;
        UpdateScoreDisplay();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcScorePlayerThree()
    {
        if (Object.HasStateAuthority) ScorePlayerThree++;
        UpdateScoreDisplay();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcScorePlayerFour()
    {
        if (Object.HasStateAuthority) ScorePlayerFour++;
        UpdateScoreDisplay();
    }
}