using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace Readme
{
    public partial class Tree : Form
    {
        public Tree()
        {
            InitializeComponent();
        }

        private void Tree_Load(object sender, EventArgs e)
        {
            TreeNode root = new TreeNode();
            //根目录名称
            root.Text = "";
            //根目录路径
            //GetFolderFiles(@"E:\", root);
            GetFolderFiles(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), root);
            treeView1.Nodes.Add(root);

            //root.Remove();
            //GetFolderFiles(Environment.GetFolderPath(Environment.SpecialFolder.Personal), root);
            //treeView1.Nodes.Add(root);
            //DriveInfo.GetDrives();
        }

        private void GetFolderFiles(string filePath, TreeNode node)
        {
            DirectoryInfo folder = new DirectoryInfo(filePath);
            node.Text = folder.Name;
            node.Tag = folder.FullName;

            /*FileInfo[] chldFiles = folder.GetFiles("*.*");
            foreach (FileInfo chlFile in chldFiles)
            {
                TreeNode chldNode = new TreeNode();
                chldNode.Text = chlFile.Name;
                chldNode.Tag = chlFile.FullName;
                node.Nodes.Add(chldNode);
            }*/
            DirectoryInfo[] chldFolders = folder.GetDirectories();
            foreach (DirectoryInfo chldFolder in chldFolders)
            {
                // 不显示隐藏文件夹
                if ((chldFolder.Attributes & FileAttributes.Hidden) != 0)
                {
                    continue;
                }
                TreeNode chldNode = new TreeNode(filePath, 0, 0);
                chldNode.Text = folder.Name;
                chldNode.Tag = folder.FullName;
                node.Nodes.Add(chldNode);
                GetFolderFiles(chldFolder.FullName, chldNode);
            }

        }
    }
}