
using UnityEngine;

namespace Skill.SwordAndShield
{
    /// <summary>
    /// 검, 방패 착용 상태에서의 기본 공격<br/>
    /// - 공격 모션 순차적으로 재생 (기본 베기 - 뒤돌아 횡 베기)<br/>
    /// - 일정 시간 내에 다음 공격이 이어지지 않으면 공격 인덱스 초기화 (원신 다이루크, 던파 단공참)<br/>
    /// - 쿨타임 = 1 / 공격 속도
    /// </summary>
    public class SwordAndShieldBaseAttack : SkillBase
    {
        /// <summary> 해당 시간 안에 다음 공격으로 넘어가지 않으면 공격 인덱스를 초기화 </summary>
        private readonly float RESET_TIME = 1.0f;
        private readonly ePlayerMotion[] MOTIONS = new ePlayerMotion[]
        {
            ePlayerMotion.Attack1,
            ePlayerMotion.Skill7_Around,
        };
        private int attackIndex = 0;

        protected override void OnInit()
        {
            attackIndex = 0;
        }

        protected override void OnStartSkill()
        {
            if (lastUsedTime + RESET_TIME < Time.time)
            {
                attackIndex = 0;
            }

            base.player.ActionComponent.Play(MOTIONS[attackIndex], player.AttackSpeed);
            cooldownTime = 1f / player.AttackSpeed;

            attackIndex = (attackIndex + 1) % MOTIONS.Length;
        }
    }
}