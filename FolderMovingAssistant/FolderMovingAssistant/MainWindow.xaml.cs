using Microsoft.Win32;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using relatedFolderMovingAssistant.Methods;
using relatedFolderMovingAssistant.Models;


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
            FolderPath.destiPath = MyMethods.GetPath(targerPathBox);
            if (!string.IsNullOrWhiteSpace(FolderPath.destiPath))
            {   //若路径不为空 则读取路径下所有文件夹
                FolderPath.destiFolders = MyMethods.GetFoldersLIst(FolderPath.destiPath);
            }


        }
        private void SelectSonPath_Click(object sender, RoutedEventArgs e)
        {   
            FolderPath.sourcePath = MyMethods.GetPath(sourcePathBox);
            if (!string.IsNullOrWhiteSpace(FolderPath.sourcePath) && FolderPath.sourcePath!= FolderPath.destiPath)
            { //若路径不为空 则读取路径下所有文件夹
                FolderPath.sourceFolders = MyMethods.GetFoldersLIst(FolderPath.sourcePath);
                var sourceList = FolderPath.sourceFolders;
                // 2. 加工数据：将 string 转换为包含 Id 和 Text 的匿名对象
                // index + 1 是为了让序号从 1 开始，而不是 0
                //show source folders list in ListView
                var itemsWithIndex = sourceList.Select((e, Index) => new
                {
                    Id = Index + 1,
                    Text = e
                }).ToList();

                // 3. 赋值给控件
                sourceFolderList.ItemsSource = itemsWithIndex;
                return;
            }
            else
            {   
                MessageBox.Show("the sourcePath is null or the same to targerPath!,Please chose againt!");
                sourcePathBox.Text = "";
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


