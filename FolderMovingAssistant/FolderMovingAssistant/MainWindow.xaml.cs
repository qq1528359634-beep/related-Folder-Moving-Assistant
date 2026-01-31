using Microsoft.Win32;
using System.IO;
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


        private void SelectMonPath_Click(object sender, RoutedEventArgs e)
        {
            MyObejects.MonPath = MyMethods.GetPath(monPathBox);
            if (!string.IsNullOrWhiteSpace(MyObejects.MonPath))
            {
                MyObejects.MonFolders = MyMethods.GetFolders(MyObejects.MonPath);

            }


        }
        private void SelectSonPath_Click(object sender, RoutedEventArgs e)
        {
            MyObejects.SonPath = MyMethods.GetPath(sonPathBox);

            if (!string.IsNullOrWhiteSpace(MyObejects.SonPath))
            {
                MyObejects.SonFolders = MyMethods.GetFolders(MyObejects.SonPath);
                var rawList = MyObejects.SonFolders;
                // 2. 加工数据：将 string 转换为包含 Id 和 Text 的匿名对象
                // index + 1 是为了让序号从 1 开始，而不是 0
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

        private void Move_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MyObejects.SonPath) || string.IsNullOrWhiteSpace(MyObejects.MonPath))
            {
                MessageBox.Show("Please configure the complete folder path first!");
                return;
            }
            else
            {
                var list = MyMethods.MoveFolder();
                var moveList = list.Where(e => e.Key.Count() == 1).Select(e => new
                {
                    Key = e.Key.First(),
                    Value = e.Value
                }).ToList();
                foreach (var item in moveList)
                {   //别忘记追加斜杆 
                    //string destiPath = MyObejects.MonPath +"\\"+item.Key;
                    //string sourcePath = MyObejects.SonPath +"\\"+item.Value;
                    string soucePath = Path.Combine(MyObejects.SonPath, item.Value);
                    string destiePath = Path.Combine(MyObejects.MonPath, Path.Combine(item.Key, item.Value));
                    string sourceDisk = Path.GetPathRoot(soucePath);
                    string destiDisk = Path.GetPathRoot(destiePath);
                    if (sourceDisk==destiDisk)
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

                    }
                    

                }
                MessageBox.Show("Move Finished！");
            }

        }
    }
    class MyObejects
    {
        public static string? MonPath;
        public static string? SonPath;

        public static List<string>? MonFolders;
        public static List<string>? SonFolders;
    }
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
        internal static List<string> GetFolders(string path)
        {                         //获取目录下的所有子文件夹路径 并只保留文件夹名 转换为list集合
            List<string> Folders = Directory.EnumerateDirectories(path).Select(e => Path.GetFileName(e)).ToList();
            return Folders;
        }

        internal static Dictionary<IEnumerable<string>, string> MoveFolder()
        {
            //返回一个合集 key为匹配到的母文件夹 value为匹配到的子文件夹
            var associatedFolder = MyObejects.SonFolders.Select(son => new
            {
                Values = son,
                key = MyObejects.MonFolders.Where(e => son.Contains(e))
            }
            ).ToDictionary(x => x.key, y => y.Values);
            return associatedFolder;
        }

    }
}