
namespace Skill.SwordAndShield
{
    public class MeeleAttack : ActionBase
    {
        public override void OnStart()
        {
            base.playerAnimation.Play(ePlayerMotion.Attack1);
        }

        public override void OnUpdate()
        {
        }

        public override void OnEnd()
        {
            // Implementation for ending the melee attack skill
        }
    }
}