using SuperNewRoles.Patches;

namespace SuperNewRoles.Roles.RoleBases.Interfaces;

/// <summary>
/// 乗っ取り勝利の場合に勝利できるかなどを返すインターフェース
/// </summary>
public interface IHijackingWinner
{
    /// <summary>
    /// Rank5が最優先
    /// </summary>
    enum Rank
    {
        Rank1,
        Rank2,
        Rank3,
        Rank4,
        Rank5,
    }

    /// <summary>
    /// 勝利優先度
    /// </summary>
    Rank Priority => Rank.Rank2;

    /// <summary>
    /// 追加勝利陣営も勝利にする
    /// </summary>
    bool AllowAdditionalWins => true;

    /// <summary>
    /// 勝利した陣営
    /// </summary>
    WinCondition Condition => WinCondition.Default;

    bool CanWin(GameOverReason gameOverReason, WinCondition winCondition) => false;
}
