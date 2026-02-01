using Microsoft.Win32;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace FolderMovingAssistant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Set source Path and destination Path
        private void SelectMonPath_Click(object sender, RoutedEventArgs e)
        {
            FolderPath.destiPath = MyMethods.GetPath(monPathBox);
            if (!string.IsNullOrWhiteSpace(FolderPath.destiPath))
            {
                FolderPath.destiFolders = MyMethods.GetFoldersLIst(FolderPath.destiPath);

            }


        }
        private void SelectSonPath_Click(object sender, RoutedEventArgs e)
        {
            FolderPath.sourcePath = MyMethods.GetPath(sonPathBox);

            if (!string.IsNullOrWhiteSpace(FolderPath.sourcePath))
            {
                FolderPath.sourceFolders = MyMethods.GetFoldersLIst(FolderPath.sourcePath);
                var rawList = FolderPath.sourceFolders;
                // 2. 加工数据：将 string 转换为包含 Id 和 Text 的匿名对象
                // index + 1 是为了让序号从 1 开始，而不是 0
                //show source folders list in ListView
                var itemsWithIndex = rawList.Select((e, Index) => new
                {
                    Id = Index + 1,
                    Text = e
                }).ToList();

                // 3. 赋值给控件
                sonFolderList.ItemsSource = itemsWithIndex;
                return;

            }
        }
        #endregion

        #region floders move method
        private void Move_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FolderPath.sourcePath) || string.IsNullOrWhiteSpace(FolderPath.destiPath))
            {
                MessageBox.Show("Please configure the complete folder path first!");
                return;
            }
            else
            {
                var relevantFolders = MyMethods.GetRelevantFolders();
                var sortedList = MyMethods.SortOneRelevantFolders(relevantFolders);
                MyMethods.ExecuteMoveFolder(sortedList);
                MessageBox.Show("Move Finished！");

            }

        }

    }
    #endregion
}
class FolderPath
{
    public static string? destiPath;
    public static string? sourcePath;

    public static List<string>? destiFolders;
    public static List<string>? sourceFolders;
}
class SortedFolder
{
    public string? destiFolder { get; set; }
    public string? SourceFolder { get; set; }
}
class MyMethods
{    ///<summary>
     ///select disk folder path
     ///</summary>
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
    /// <summary>
    /// get source folders list adn destination folders list
    /// </summary>
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
            if (!Path.Exists(destiePath)&&Path.Exists(soucePath))
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
