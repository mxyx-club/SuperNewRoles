using System.Text;
using AmongUs.GameOptions;

namespace SuperNewRoles.Roles.RoleBases.Interfaces;
public interface ISupportSHR : ITaskHolder
{
    /// <summary>
    /// 判定上の役職
    /// </summary>
    RoleTypes RealRole { get; }
    bool IsRealRoleNotModOnly => false;
    /// <summary>
    /// Desyncの場合はDesync役職を設定する
    /// </summary>
    RoleTypes DesyncRole => RealRole;
    /// <summary>
    /// Desync役職か判定
    /// </summary>
    sealed bool IsDesync => RealRole != DesyncRole;
    /// <summary>
    /// インポスター視界かを設定
    /// nullの場合はクルーか第三でDesyncインポならクルーに設定
    /// </summary>
    bool? IsImpostorLight => null;
    bool IsZeroCoolEngineer => false;
    void BuildName(StringBuilder Suffix, StringBuilder RoleNameText, PlayerData<string> ChangePlayers)
    {

    }
    void BuildSetting(IGameOptions gameOptions)
    {

    }
}