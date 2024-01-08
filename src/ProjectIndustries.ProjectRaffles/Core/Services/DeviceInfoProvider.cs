using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
    public class DeviceInfoProvider : IDeviceInfoProvider
    {
        private static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);

        public async Task<string> GetHwidAsync(CancellationToken ct)
        {
            try
            {
                await SemaphoreSlim.WaitAsync(ct);
                return await Task.Run(GenerateHwid, ct);
                // return "E79E8DD1C14D9F9E2D190D75B7C61775";
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        private static string GenerateHwid()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string hwid = CreateMD5(WindowsHWID.Value());
                return hwid;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                string hwid = CreateMD5(Bash("system_profiler SPHardwareDataType | awk '/UUID/ { print $3; }'"));
                return hwid;
            }

            return null;
        }

        private static string CreateMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }

                return sb.ToString();
            }
        }

        private static string Bash(string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result.Trim();
        }
    }
}