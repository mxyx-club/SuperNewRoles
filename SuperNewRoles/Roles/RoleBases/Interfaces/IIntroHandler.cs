namespace SuperNewRoles.Roles.RoleBases.Interfaces;
/// <summary>
/// Introのタイミングで処理を行うインターフェイス
/// </summary>
public interface IIntroHandler
{
    /// <summary>
    /// Introが始まった時に呼ばれる(全視点で)
    /// </summary>
    void OnIntroStart()
    {

    }
    /// <summary>
    /// Introが始まった時に呼ばれる(自分視点で)
    /// </summary>
    void OnIntroStartMe()
    {

    }
    /// <summary>
    /// IntroCutsceneが破棄された時に呼ばれる(全視点で)
    /// </summary>
    void OnIntroDestory()
    {

    }
    /// <summary>
    /// IntroCutsceneが破棄された時に呼ばれる(自分視点で)
    /// </summary>
    void OnIntroDestoryMe()
    {

    }
}