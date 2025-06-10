namespace SuperNewRoles.Roles.RoleBases.Interfaces;
public interface IJackal : IKiller, IVentAvailable
{
    bool CanSidekick { get; }
    bool isShowSidekickButton { get; }
    bool isShowKillButton => true;
    float SidekickCoolTime => 0f;
    float JackalKillCoolTime => 0f;

    void OnClickSidekickButton(PlayerControl target);
    void SetAmSidekicked();
}