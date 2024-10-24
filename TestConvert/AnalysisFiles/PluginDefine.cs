using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TestConvert.AnalysisFile
{
    public class ProgramPlugins
    {
#if UNITY_EDITOR || UNITY_EDITOR_OSX
        public const string pluginName = "hello";
#elif UNITY_WEBGL 
        public const string pluginName = "__Internal";
#else
        public const string pluginName = "hello";
#endif
    }

    internal class PluginDefine
    {
#if UNITY_IPHONE
	    [DllImport("__Internal",ExactSpelling = true)]
#else
        [DllImport(ProgramPlugins.pluginName, ExactSpelling = true)]
#endif
        static extern System.IntPtr CreateDataManager();

#if UNITY_IPHONE
	    [DllImport("__Internal",ExactSpelling = true)]
#else
        [DllImport(ProgramPlugins.pluginName, ExactSpelling = true)]
#endif
        static extern void ReleaseDataManager(System.IntPtr dataManager);

#if UNITY_IPHONE
	    [DllImport("__Internal",ExactSpelling = true)]
#else
        [DllImport(ProgramPlugins.pluginName, ExactSpelling = true)]
#endif
        static extern System.Byte InitDataManager(System.IntPtr dataManager, System.UInt32 bufferSize, System.IntPtr configBuffer);

    }
}
