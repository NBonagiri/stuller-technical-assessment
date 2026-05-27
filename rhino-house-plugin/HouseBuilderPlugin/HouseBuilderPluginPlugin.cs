using Rhino.PlugIns;

namespace HouseBuilderPlugin
{
    public class HouseBuilderPluginPlugin : PlugIn
    {
        public HouseBuilderPluginPlugin()
        {
            Instance = this;
        }

        public static HouseBuilderPluginPlugin Instance { get; private set; }
    }
}