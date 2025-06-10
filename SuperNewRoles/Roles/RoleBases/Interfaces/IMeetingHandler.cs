using AmongUs.GameOptions;

namespace SuperNewRoles.Roles.RoleBases.Interfaces;
/// <summary>
/// 会議に関する処理を行う際に使うインターフェース
/// </summary>
public interface IMeetingHandler
{
    void StartMeeting();
    void CloseMeeting();

    /// <summary> 匿名投票か </summary>
    /// <returns> true : 匿名投票 / false : 公開投票</returns>
    bool EnableAnonymousVotes => GameOptionsManager.Instance.CurrentGameOptions.GetBool(BoolOptionNames.AnonymousVotes);
}