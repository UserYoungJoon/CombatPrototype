using TMPro;
using UnityEngine;

public class PlayerStatsUI : MonoBehaviour
{
    public PlayerUnit player;
    public TextMeshProUGUI stateText;
    public TextMeshProUGUI pendingKeyText;
    public TextMeshProUGUI actualAnimText;
    public TextMeshProUGUI currentMotionText;

    public TextMeshProUGUI enemyAnimText;

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

        var enemy = GameManager.Instance.monster;
        if (enemy != null)
        {
            var enemyStateInfo = enemy.ActionComponent.CurrentStateInfo;
            enemyAnimText.text = $"Enemy: {enemy.ActionComponent.CurrentMotion} ({enemyStateInfo.normalizedTime:F2})";
        }
        else
        {
            enemyAnimText.text = "Enemy: null";
        }
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