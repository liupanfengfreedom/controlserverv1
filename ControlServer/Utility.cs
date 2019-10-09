using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

namespace ControlServer
{
    class Utility
    {
#region launchdemo
        //string path = @"F:\uev\pro422.uproject";
        //string Arguments = "";
        //Utility.CommandRun(path, Arguments);
        static string path = @"C:\Program Files\Epic Games\UE_4.22\Engine\Build\BatchFiles\RunUAT.bat";
        static string Arguments = "BuildCookRun -project=D:\\ueprojecttest/MyProject/MyProject.uproject  -noP4 -platform=Android -clientconfig=Development -serverconfig=Development -cook -allmaps -stage -pak -archive";
#endregion

        public static Process CommandRun(string exe, string arguments)
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    FileName = exe,
                    Arguments = arguments,
                    UseShellExecute = true,//true mean can launch .bat file
                };
                Process p = Process.Start(info);
                //p.WaitForExit();
                return p;
            }
            catch (Exception e)
            {
                // Log.Error(e);
            }
            return null;
        }
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                bool bfileexist = File.Exists(temppath);
                if (bfileexist)
                {
                    File.Delete(temppath);
                }
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
        public static bool  DirectoryDelete(string sourceDirName)
        {
            bool bexist = Directory.Exists(sourceDirName);
            if (bexist)
            {
                try
                {
                    Directory.Delete(sourceDirName, true);
                }
                catch
                {

                }
            }
            bexist = Directory.Exists(sourceDirName);
            if (bexist)
            {
                return false;
            }
            return true;
        }
        public static bool SubDirectoryDelete(string sourceDirName)
        {

            bool bexist = Directory.Exists(sourceDirName);
            if (bexist)
            {
                DirectoryInfo dir = new DirectoryInfo(sourceDirName);
                DirectoryInfo[] dirs = dir.GetDirectories();
                try
                {
                    foreach (DirectoryInfo subdir in dirs)
                    {
                        string temppath = Path.Combine(sourceDirName, subdir.Name);
                        Directory.Delete(temppath, true);
                    }
                }
                catch
                {

                }
            }
            return true;
        }
        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
        public static string MD5Hash(byte[]bytesinput)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(bytesinput);

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
    }
}
