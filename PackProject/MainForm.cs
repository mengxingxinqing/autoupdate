using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PackProject
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            txtProject.Text = GetSetting("project");
            txtVersion.Text = GetSetting("version");
            txtZip.Text = GetSetting("zip");
        }

        private string[] filter = { "AutoUpdate.exe" , "AutoUpdate.pdb", "AutoUpdate.vshost.exe", "DotNetZip.dll", "DotNetZip.xml", "update.xml" };

        private void btnChoose1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.ShowDialog();
            txtProject.Text = path.SelectedPath;
        }

        private void btnChoose2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.ShowDialog();
            txtZip.Text = path.SelectedPath;
        }

        private void btnPack_Click(object sender, EventArgs e)
        {
            if(txtZip.Text=="" || txtProject.Text == "" || txtVersion.Text == "")
            {
                MessageBox.Show("请填写信息");
                return;
            }
            ModifySetting("project", txtProject.Text);
            ModifySetting("version", txtVersion.Text);
            ModifySetting("zip", txtZip.Text);
            ZipDirectory(txtProject.Text, Path.Combine(txtZip.Text, txtVersion.Text+".zip"));
            MessageBox.Show("生成成功，请上传文件到服务器，并且修改manifests.xml的版本号");
        }

        /// <summary>
        /// 复制文件夹（及文件夹下所有子文件夹和文件）
        /// </summary>
        /// <param name="sourcePath">待复制的文件夹路径</param>
        /// <param name="destinationPath">目标路径</param>
        public void ZipDirectory(String sourcePath, String destinationPath)
        {
            if (File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
            }
            using (ZipFile zip = new ZipFile(destinationPath, System.Text.Encoding.Default))
            {
                //zip.AddDirectory(@"E:\test");//添加文件夹
                //zip.AddFile(@"E:\房屋租赁协议.doc");//添加文件,文件不存在抛错FileNotFoundException
                //zip.Save();
                DirectoryInfo info = new DirectoryInfo(sourcePath);
                
                foreach (FileSystemInfo fsi in info.GetFileSystemInfos())
                {
                    String destName = Path.Combine(sourcePath, fsi.Name);

                    if (fsi is System.IO.FileInfo && !filter.Contains(fsi.Name))          //如果是文件，复制文件
                    {
                        //File.Copy(fsi.FullName, destName, true);
                        zip.AddFile(destName, "");
                    }
                    else if(fsi is DirectoryInfo)                                   //如果是文件夹，新建文件夹，递归
                    {
                        zip.AddDirectory(destName, fsi.Name);
                    }
                }

                zip.Save();
            }

            
        }

        public void ModifySetting(string key,string val)
        {
            string file = System.Windows.Forms.Application.ExecutablePath;
            Configuration config = ConfigurationManager.OpenExeConfiguration(file);

            bool exist = false;//记录这个com端口值是否存在
            if (config.AppSettings.Settings[key] != null)
            {
                config.AppSettings.Settings.Remove(key);
            }
            config.AppSettings.Settings.Add(key, val);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public string GetSetting(string key)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            return config.AppSettings.Settings[key].Value; 
        }
    }
}
