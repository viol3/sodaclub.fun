using UnityEngine;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Sui.Tests.CLI
{
    /// <summary>
    /// Calls sui node programmatically.
    /// <code>
    ///     RUST_LOG="off,sui_node=info" sui start --with-faucet --force-regenesis --with-indexer
    /// </code>
    /// </summary>
    public class CliCommandExecutor
    {
        public class CommandResult
        {
            public int ExitCode { get; set; }
            public string Output { get; set; }
            public string Error { get; set; }
        }

        public static CommandResult ExecuteCommand(string command, string args = "", IDictionary<string, string> environmentVariables = null)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = GetCommandFileName(command),
                Arguments = GetCommandArguments(command, args),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Application.dataPath + "/.."
            };

            // Copy current environment PATH
            var path = Environment.GetEnvironmentVariable("PATH");
            UnityEngine.Debug.Log($"Current PATH: {path}");

            // Add common Sui installation paths
            var additionalPaths = new[]
            {
                "/usr/local/bin",                    // Common Unix path
                "/opt/homebrew/bin",                 // Homebrew on Apple Silicon
                "/usr/local/Homebrew/bin",           // Homebrew on Intel Mac
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.cargo/bin"  // Rust/Cargo bin
            };

            foreach (var additionalPath in additionalPaths)
            {
                if (!path.Contains(additionalPath))
                {
                    path = $"{additionalPath}{Path.PathSeparator}{path}";
                }
            }

            startInfo.EnvironmentVariables["PATH"] = path;

            if (environmentVariables != null)
            {
                foreach (var variable in environmentVariables)
                {
                    startInfo.EnvironmentVariables[variable.Key] = variable.Value;
                }
            }

            using var process = new Process { StartInfo = startInfo };
            var result = new CommandResult();

            try
            {
                UnityEngine.Debug.Log($"Executing command: {command} {args}");
                process.Start();
                result.Output = process.StandardOutput.ReadToEnd();
                result.Error = process.StandardError.ReadToEnd();
                process.WaitForExit();
                result.ExitCode = process.ExitCode;

                UnityEngine.Debug.Log($"Command output: {result.Output}");
                if (!string.IsNullOrEmpty(result.Error))
                {
                    UnityEngine.Debug.LogError($"Command error: {result.Error}");
                }
                UnityEngine.Debug.Log($"Exit code: {result.ExitCode}");
            }
            catch (Exception ex)
            {
                result.Error = ex.ToString();
                result.ExitCode = -1;
                UnityEngine.Debug.LogError($"Exception executing command: {ex}");
            }

            return result;
        }

        public static string GetCommandFileName(string command)
        {
            // If we can find the command in PATH, use it directly
            var suiPath = GetFullPath(command);
            if (!string.IsNullOrEmpty(suiPath))
            {
                UnityEngine.Debug.Log($"Found {command} at: {suiPath}");
                return suiPath;
            }

            // Fallback to shell if direct command not found
            if (IsWindows())
            {
                return "cmd.exe";
            }
            return "/bin/bash";
        }

        public static string GetCommandArguments(string command, string args)
        {
            var fullPath = GetFullPath(command);
            if (!string.IsNullOrEmpty(fullPath))
            {
                // Direct execution - just pass the arguments as is
                return args;
            }

            // Shell wrapping only if we couldn't find the direct path
            if (IsWindows())
            {
                return $"/c {command} {args}";
            }
            return $"-c \"{command} {args}\"";
        }

        private static string GetFullPath(string command)
        {
            // Common installation paths
            var searchPaths = new List<string>
            {
                "/usr/local/bin",                    // Common Unix path
                "/opt/homebrew/bin",                 // Homebrew on Apple Silicon
                "/usr/local/Homebrew/bin",           // Homebrew on Intel Mac
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".cargo/bin")  // Rust/Cargo bin
            };

            // Add PATH environment paths
            searchPaths.AddRange(Environment.GetEnvironmentVariable("PATH").Split(Path.PathSeparator));

            foreach (var path in searchPaths)
            {
                if (string.IsNullOrEmpty(path)) continue;

                var fullPath = Path.Combine(path, command);
                if (IsWindows() && !fullPath.EndsWith(".exe"))
                {
                    fullPath += ".exe";
                }

                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }

            return string.Empty;
        }

        private static bool IsWindows()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT;
        }
    }

    public class SuiNodeTests
    {
        private Process suiNodeProcess;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            UnityEngine.Debug.Log("Starting Sui node setup...");

            // Check if Sui is installed
            var versionResult = CliCommandExecutor.ExecuteCommand("sui", "--version");
            UnityEngine.Debug.Log($"Version check exit code: {versionResult.ExitCode}");
            if (versionResult.ExitCode != 0)
            {
                throw new Exception(
                    $"Sui CLI check failed. Error: {versionResult.Error}\n" +
                    "Please install Sui first (https://docs.sui.io/build/install)"
                );
            }

            UnityEngine.Debug.Log($"Detected Sui version: {versionResult.Output.Trim()}");

            var envVars = new Dictionary<string, string>
            {
                { "RUST_LOG", "off,sui_node=info" }
            };

            // The complete command: RUST_LOG="off,sui_node=info" sui start --with-faucet --force-regenesis --with-indexer
            //var startArgs = "start --with-faucet --force-regenesis --with-indexer";
            var startArgs = "start --with-faucet --force-regenesis";
            UnityEngine.Debug.Log($"Starting Sui node with arguments: {startArgs}");

            var startInfo = new ProcessStartInfo
            {
                FileName = CliCommandExecutor.GetCommandFileName("sui"),
                Arguments = startArgs,  // Direct arguments without shell wrapping
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Application.dataPath + "/.."
            };

            foreach (var variable in envVars)
            {
                startInfo.EnvironmentVariables[variable.Key] = variable.Value;
            }

            UnityEngine.Debug.Log("Attempting to start Sui node...");
            suiNodeProcess = new Process { StartInfo = startInfo };
            suiNodeProcess.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    UnityEngine.Debug.Log($"Sui Node Output: {e.Data}");
            };
            suiNodeProcess.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    UnityEngine.Debug.LogError($"Sui Node Error: {e.Data}");
            };

            try
            {
                suiNodeProcess.Start();
                suiNodeProcess.BeginOutputReadLine();
                suiNodeProcess.BeginErrorReadLine();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Failed to start Sui node: {ex.Message}");
                throw;
            }

            // Wait for the process outside try-catch
            yield return new WaitForSeconds(5);

            if (suiNodeProcess.HasExited)
            {
                throw new Exception($"Sui node process exited prematurely with code: {suiNodeProcess.ExitCode}");
            }

            UnityEngine.Debug.Log("Sui node started successfully");
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (suiNodeProcess != null && !suiNodeProcess.HasExited)
            {
                // Try graceful shutdown first
                var killResult = CliCommandExecutor.ExecuteCommand("sui", "node kill");

                yield return new WaitForSeconds(2);

                try
                {
                    // If process is still running, force kill it
                    if (!suiNodeProcess.HasExited)
                    {
                        UnityEngine.Debug.Log("Sui node didn't shut down gracefully, forcing termination...");
                        suiNodeProcess.Kill();
                    }
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError($"Error during Sui node cleanup: {ex.Message}");
                    try
                    {
                        suiNodeProcess.Kill();
                    }
                    catch (Exception killEx)
                    {
                        UnityEngine.Debug.LogError($"Failed to force kill Sui node: {killEx.Message}");
                    }
                }
                finally
                {
                    suiNodeProcess.Dispose();
                    suiNodeProcess = null;
                }
            }
        }
    }

    public class DummyTests : SuiNodeTests
    {
        [UnityTest]
        public IEnumerator TestSuiNodeInteraction()
        {
            // Test code here
            yield return null;
            Assert.Pass();
        }
    }
}