using SuperNewRoles.CustomObject;

namespace SuperNewRoles.WaveCannonObj;
public interface IWaveCannonAnimationHandler
{
    public WaveCannonObject CannonObject { get; }
    public CustomAnimationOptions Init();
    public void OnShot();
    public void RendererUpdate();
    public void OnUpdate() { }
}