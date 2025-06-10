namespace SuperNewRoles.Roles.RoleBases.Interfaces;

// ジャッカルのサイドキック専用ではなく、全ての役職変更系に使えるインターフェースです。
public interface ISidekick
{
    /// <summary>
    /// 昇格先の役職
    /// </summary>
    RoleId TargetRole { get; }

    void SetParent(PlayerControl player);
}