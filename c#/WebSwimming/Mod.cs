using GDWeave;

namespace WebSwimming;

public class Mod : IMod {

    public Mod(IModInterface modInterface) {
        modInterface.RegisterScriptMod(new PlayerDataScriptMod());
        modInterface.RegisterScriptMod(new PlayerScriptMod());
    }

    public void Dispose() {
        // Cleanup anything you do here
    }
}
