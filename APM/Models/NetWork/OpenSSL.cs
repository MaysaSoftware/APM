using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace APM.Models.NetWork
{
    public class OpenSSL
    {
        public void ExecuteCMD(string Path,string Command,ref string CMDResult)
        { 
            var process = new Process();
            var psi = new ProcessStartInfo();
            psi.FileName = "cmd.exe"; 
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            psi.WorkingDirectory = Path;
            process.StartInfo = psi;
            process.Start();
            string ResultMessage = ""; 
            process.OutputDataReceived += (sender, e) => { ResultMessage +=  e.Data+"\n"; };
            process.ErrorDataReceived += (sender, e) => { ResultMessage += e.Data + "\n"; };
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            using (StreamWriter sw = process.StandardInput)
            {  
                sw.WriteLine(Command);
            }
            process.WaitForExit(); 
            CMDResult = ResultMessage;
            process.Close();
        }
    }
}