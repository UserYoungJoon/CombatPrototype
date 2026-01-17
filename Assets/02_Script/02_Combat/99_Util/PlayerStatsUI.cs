using TMPro;
using UnityEngine;

public class PlayerStatsUI : MonoBehaviour
{
    public PlayerUnit player;
    public TextMeshProUGUI stateText;
    public TextMeshProUGUI pendingKeyText;
    public TextMeshProUGUI actualAnimText;
    public TextMeshProUGUI currentMotionText;

    void Update()
    {
        if (player is null) return;
        stateText.text = $"State : {player.StateComponent.State}";
        var pendingKey = player.ControllerComponent.pendingKeyPress;
        pendingKeyText.text = $"Pending Key : {pendingKey}";

        var stateInfo = player.ActionComponent.CurrentStateInfo;
        string actualAnim = GetActualAnimName(stateInfo);
        actualAnimText.text = $"Actual Anim : {actualAnim} ({stateInfo.normalizedTime:F2})";
        currentMotionText.text = $"CurrentMotion : {player.ActionComponent.CurrentMotion}";
    }

    private string GetActualAnimName(AnimatorStateInfo stateInfo)
    {
        foreach (ePlayerMotionType motion in System.Enum.GetValues(typeof(ePlayerMotionType)))
        {
            if (stateInfo.IsName(motion.ToString()))
                return motion.ToString();
        }
        return "Unknown";
    }
}