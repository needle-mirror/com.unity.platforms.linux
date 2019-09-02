using System.Diagnostics;
using System.IO;

namespace Unity.Platforms.Linux
{
    public abstract class LinuxBuildTarget : BuildTarget
    {
        public override bool HideInBuildTargetPopup => UnityEngine.Application.platform != UnityEngine.RuntimePlatform.LinuxEditor;

        public override string GetUnityPlatformName()
        {
            return nameof(UnityEditor.BuildTarget.StandaloneLinux64);
        }

        public override bool Run(FileInfo buildTarget)
        {
            var startInfo = new ProcessStartInfo();
            startInfo.Arguments = $"\"{buildTarget.FullName.Trim('\"')}\"";
            startInfo.FileName = Path.GetFullPath(Path.Combine(UnityEditor.EditorApplication.applicationContentsPath, "MonoBleedingEdge", "bin", "mono"));
            startInfo.WorkingDirectory = buildTarget.Directory.FullName;
            if (!startInfo.EnvironmentVariables.ContainsKey("LD_LIBRARY_PATH"))
				startInfo.EnvironmentVariables.Add("LD_LIBRARY_PATH",".");
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            var process = Process.Start(startInfo);
            return process != null;
        }
    }

    class DotNetLinuxBuildTarget : LinuxBuildTarget
    {
#if UNITY_EDITOR_LINUX
        protected override bool IsDefaultBuildTarget => true;
#endif

        public override string GetDisplayName()
        {
            return "Linux .NET";
        }

        public override string GetBeeTargetName()
        {
            return "linux-dotnet";
        }
        public override string GetExecutableExtension()
		{
			return ".exe";
		}
    }

    class IL2CPPLinuxBuildTarget : LinuxBuildTarget
    {
        public override string GetDisplayName()
        {
            return "Linux IL2CPP";
        }

        public override string GetBeeTargetName()
        {
            return "linux-il2cpp";
        }
        public override string GetExecutableExtension()
		{
			return string.Empty;
		}
    }
}
