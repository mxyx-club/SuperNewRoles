using SuperNewRoles.CustomObject;

namespace SuperNewRoles.WaveCannonObj;
public interface IWaveCannonAnimationHandler
{
    WaveCannonObject CannonObject { get; }
    CustomAnimationOptions Init();
    void OnShot();
    void RendererUpdate();
    void OnUpdate() { }
}