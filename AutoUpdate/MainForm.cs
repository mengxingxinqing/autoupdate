using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace AutoUpdate
{
    public partial class MainForm : Form
    {
        private LocalConf localConf = new LocalConf();
        private string tmpPath = Path.Combine(Environment.CurrentDirectory, "tmp");
        private string tmpFilePath;
        private Manifest manifest;
        public MainForm()
        {
            InitializeComponent();

            BuildDir(tmpPath);
            tmpFilePath = Path.Combine(tmpPath, "files");
            BuildDir(tmpFilePath);
            check();
        }



        public void check()
        {
            Uri uri = new Uri(localConf.Manifest);
            string doc = GetManifest(uri);
            XmlSerializer xser = new XmlSerializer(typeof(Manifest));
            manifest = xser.Deserialize(new XmlTextReader(doc, XmlNodeType.Document, null)) as Manifest;
            lbRemark.Text = manifest.Description;
            lbVersion.Text = manifest.Version;
            SetProcessBar(20);
            if (manifest.Version != localConf.Version)
            {
                string cmd = "taskkill /im " + Path.Combine(Environment.CurrentDirectory, manifest.ExePath) + " /f ";
                ExeCommand(cmd);

                Thread entry = new Thread(ProcessUpdate);//求和方法被定义为工作线程入口  
                entry.Start();
                localConf.Version = manifest.Version;

            }
        }

        public void ProcessUpdate()
        {
            Thread.Sleep(1000);
            string serPath = Path.Combine(manifest.WebPath, manifest.Version + ".zip");
            string cliPath = Path.Combine(tmpPath, manifest.Version + ".zip");
            DownZip(serPath, cliPath);
            SetProcessBar(40);
            UnZip(cliPath, tmpFilePath);
            SetProcessBar(60);
            CopyDirectory(tmpFilePath, Environment.CurrentDirectory);
            SetProcessBar(90);
            DeleteFolder(tmpPath);
            SetProcessBar(100);
            Thread.Sleep(1000);
            Process.Start(Path.Combine(Environment.CurrentDirectory, manifest.ExePath));

            //Application.Exit();
            Environment.Exit(0);
        }


        /// <summary>
        /// 删除文件夹（及文件夹下所有子文件夹和文件）
        /// </summary>
        /// <param name="directoryPath"></param>
        public static void DeleteFolder(string directoryPath)
        {
            foreach (string d in Directory.GetFileSystemEntries(directoryPath))
            {
                if (File.Exists(d))
                {
                    FileInfo fi = new FileInfo(d);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                        fi.Attributes = FileAttributes.Normal;
                    File.Delete(d);     //删除文件   
                }
                else
                    DeleteFolder(d);    //删除文件夹
            }
            Directory.Delete(directoryPath);    //删除空文件夹
        }

        /// <summary>
        /// 复制文件夹（及文件夹下所有子文件夹和文件）
        /// </summary>
        /// <param name="sourcePath">待复制的文件夹路径</param>
        /// <param name="destinationPath">目标路径</param>
        public static void CopyDirectory(String sourcePath, String destinationPath)
        {
            DirectoryInfo info = new DirectoryInfo(sourcePath);
            foreach (FileSystemInfo fsi in info.GetFileSystemInfos())
            {
                String destName = Path.Combine(destinationPath, fsi.Name);

                if (fsi is System.IO.FileInfo)          //如果是文件，复制文件
                    File.Copy(fsi.FullName, destName,true);
                else                                    //如果是文件夹，新建文件夹，递归
                {
                    Directory.CreateDirectory(destName);
                    CopyDirectory(fsi.FullName, destName);
                }
            }
        }

        delegate void SetProcessBarCallBack(int current);
        private void SetProcessBar(int current)
        {
            if (this.progressBar1.InvokeRequired)
            {
                SetProcessBarCallBack cb = new SetProcessBarCallBack(SetProcessBar);
                this.Invoke(cb, new object[] { current });
            }
            else
            {
                if (current > 100)
                {
                    current = 100;
                }
                this.progressBar1.Value = current;
            }
        }


        public void BuildDir(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public void UnZip(string file,string dir)
        {
            using (ZipFile zip = new ZipFile(file, System.Text.Encoding.Default))
            {
                zip.ExtractAll(dir, ExtractExistingFileAction.OverwriteSilently);
            }
        }

        public static void ExeCommand(string commandText)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            try
            {
                p.Start();
                p.StandardInput.WriteLine(commandText);
                p.StandardInput.WriteLine("exit");
                //p.StandardOutput.ReadToEnd();
            }
            catch
            {

            }

        }

        public void DownZip(string ser,string cli)
        {
            WebClient webClient = new WebClient();
            Uri uri = new Uri(ser);
            webClient.DownloadFile(uri, cli);
        }

        private string GetManifest(Uri uri)
        {
            WebRequest request = WebRequest.Create(uri);
            request.Credentials = CredentialCache.DefaultCredentials;
            string response = String.Empty;
            using (WebResponse res = request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(res.GetResponseStream(), true))
                {
                    response = reader.ReadToEnd();
                }
            }
            return response;
        }
        
    }
}
