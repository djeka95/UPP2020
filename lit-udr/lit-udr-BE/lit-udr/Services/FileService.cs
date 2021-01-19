using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace lit_udr.Services
{
    public class FileService
    {
        public static void CreateFile(string fileName,string message)
        {
            using (StreamWriter writer = System.IO.File.AppendText($"Files\\{fileName}.txt"))
            {
                writer.WriteLine(message);
            }
        }
    }

    public static class ControlFlow
    {
        public static Exception ResumeOnError(Action action)
        {
            try
            {
                action();
                return null;
            }
            catch (Exception caught)
            {
                return caught;
            }
        }
    }
}
