using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

namespace Unity.Platforms.Linux
{
    public abstract class LinuxBuildTarget : BuildTarget
    {
        public override string GetUnityPlatformName()
        {
            return nameof(UnityEditor.BuildTarget.StandaloneLinux64);
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
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            var process = new Process();
            process.StartInfo = startInfo;
            process.OutputDataReceived += (_, args) => Debug.Log(args.Data);
            process.ErrorDataReceived += (_, args) => Debug.LogError(args.Data);
            
            var success = process.Start();
            if (!success)
                return false;

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return true;
        }

        public override ShellProcessOutput RunTestMode(string exeName, string workingDirPath, int timeout)
        {
            var shellArgs = new ShellProcessArgs
            {
                Executable = Path.GetFullPath(Path.Combine(UnityEditor.EditorApplication.applicationContentsPath, "MonoBleedingEdge", "bin", "mono")),
                Arguments = new [] { $"\"{workingDirPath}/{exeName}{GetExecutableExtension()}\"" },
                WorkingDirectory = new DirectoryInfo(workingDirPath),
                ThrowOnError = false
            };

            // samples should be killed on timeout
            if (timeout > 0)
            {
                shellArgs.MaxIdleTimeInMilliseconds = timeout;
                shellArgs.MaxIdleKillIsAnError = false;
            }

            return Shell.Run(shellArgs);
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

        public override bool Run(FileInfo buildTarget)
        {
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = buildTarget.FullName.Trim('.');
            startInfo.WorkingDirectory = buildTarget.Directory.FullName;
            if (!startInfo.EnvironmentVariables.ContainsKey("LD_LIBRARY_PATH"))
                startInfo.EnvironmentVariables.Add("LD_LIBRARY_PATH",".");
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            var process = new Process();
            process.StartInfo = startInfo;
            process.OutputDataReceived += (_, args) => Debug.Log(args.Data);
            process.ErrorDataReceived += (_, args) => Debug.LogError(args.Data);
            
            var success = process.Start();
            if (!success)
                return false;

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return true;
        }

        public override ShellProcessOutput RunTestMode(string exeName, string workingDirPath, int timeout)
        {
            var shellArgs = new ShellProcessArgs
            {
                Executable = $"{workingDirPath}/{exeName}{GetExecutableExtension()}",
                Arguments = new string [] {},
                WorkingDirectory = new DirectoryInfo(workingDirPath),
                ThrowOnError = false
            };

            // samples should be killed on timeout
            if (timeout > 0)
            {
                shellArgs.MaxIdleTimeInMilliseconds = timeout;
                shellArgs.MaxIdleKillIsAnError = false;
            }

            return Shell.Run(shellArgs);
        }
    }
}
