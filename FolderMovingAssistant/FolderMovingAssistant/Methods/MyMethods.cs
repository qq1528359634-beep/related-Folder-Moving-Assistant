using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using relatedFolderMovingAssistant.Models;

namespace relatedFolderMovingAssistant.Methods
{
    class MyMethods
    {
        internal static string GetPath(TextBox box)
        {
            string path = "";
            //判断是否选择路径，如果选择路径则执行 if内语句
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
            if (openFolderDialog.ShowDialog() == true)
            {
                path = openFolderDialog.FolderName;
                box.Text = path;
            }
            //返回选择路径
            return path;
        }
        internal static List<string> GetFoldersLIst(string path)
        {                         //获取目录下的所有子文件夹路径 并只保留文件夹名 转换为list集合
            List<string> foldersList = Directory.EnumerateDirectories(path).Select(e => Path.GetFileName(e)).ToList();
            return foldersList;
        }

        internal static Dictionary<IEnumerable<string>, string> GetRelevantFolders()
        {
            //返回一个合集 key为匹配到的母文件夹(key is IEnumerabkle) value为匹配到的子文件夹
            var relevantFolders = FolderPath.sourceFolders.Select(son => new
            {
                Values = son,
                key = FolderPath.destiFolders.Where(e => son.Contains(e))
            }
            ).ToDictionary(x => x.key, y => y.Values);
            return relevantFolders;
        }
        internal static List<SortedFolder> SortOneRelevantFolders(Dictionary<IEnumerable<string>, string> relevantFolders)
        {
            var sortedList = relevantFolders.Where(pairs => pairs.Key.Count() == 1).Select(onePairs => new SortedFolder
            {
                destiFolder = onePairs.Key.First(),
                SourceFolder = onePairs.Value
            }).ToList();
            return sortedList;
        }
        internal static void ExecuteMoveFolder(List<SortedFolder> sortedList)
        {
            foreach (var item in sortedList)
            {
                string soucePath = Path.Combine(FolderPath.sourcePath, item.SourceFolder);
                string destiePath = Path.Combine(FolderPath.destiPath, Path.Combine(item.destiFolder, item.SourceFolder));
                string sourceDisk = Path.GetPathRoot(soucePath);
                string destiDisk = Path.GetPathRoot(destiePath);
                if (!Path.Exists(destiePath) && Path.Exists(soucePath))
                {
                    if (sourceDisk == destiDisk)
                    {
                        try
                        {
                            Directory.Move(soucePath, destiePath);
                        }
                        catch (Exception excep)
                        {
                            MessageBox.Show(excep.Message);
                        }
                    }
                    else
                    {
                        MoveFoldersAcrossDisk(soucePath, destiePath);
                    }
                }
                else
                {
                    MessageBox.Show($" destination folder {item.destiFolder}  already existed! or\n" +
                        $"source folder{item.SourceFolder} is missed!");
                }

            }
        }

        internal static void MoveFoldersAcrossDisk(string soucePath, string destiePath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(soucePath);
                Directory.CreateDirectory(destiePath);
                foreach (FileInfo file in dir.GetFiles())
                {
                    string filePath = Path.Combine(destiePath, file.Name);
                    file.CopyTo(filePath, true);
                }
                foreach (DirectoryInfo subDir in dir.GetDirectories())
                {
                    string newDestiePath = Path.Combine(destiePath, subDir.Name);
                    MoveFoldersAcrossDisk(subDir.FullName, newDestiePath);
                }
                dir.Delete(true);
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
            }


        }
    }
}
