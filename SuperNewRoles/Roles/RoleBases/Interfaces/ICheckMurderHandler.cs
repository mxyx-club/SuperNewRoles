namespace SuperNewRoles.Roles.RoleBases.Interfaces;
public interface ICheckMurderHandler
{
    /// <summary>
    /// 自分が関係なくても叩かれる
    /// </summary>
    bool OnCheckMurderPlayer(PlayerControl source, PlayerControl target)
    {
        return true;
    }
    /// <summary>
    /// 自分がキラーの時に叩かれる
    /// </summary>
    bool OnCheckMurderPlayerAmKiller(PlayerControl target)
    {
        return true;
    }
    /// <summary>
    /// 自分がターゲットの時に叩かれる
    /// </summary>
    bool OnCheckMurderPlayerAmTarget(PlayerControl source)
    {
        return true;
    }
}