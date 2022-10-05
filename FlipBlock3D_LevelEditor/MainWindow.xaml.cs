using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Threading;
using System.Globalization;
using FlipBlock3D_LevelEditor.Languages;
using FlipBlock3D_LevelEditor.Properties;

namespace FlipBlock3D_LevelEditor
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //string sProgramVersion = "0.40";
        //string sReleseData = "2018.06.05";
        string projectName = "";
        string projectPath = "";
        //double dDefoulWidthLeftPanel = 240;
        //double dDefoulWidthRightPanel = 260;
        //const string pathConfigFile = "ConfigurationFile.fb3d";
        //public ProgramConfiguration globalProgConf = new ProgramConfiguration();
        /*public class ProgramConfiguration
        {
            public string SelectedLanguage = "";
            public bool hideToolBar, hideLeftPanel, hideRightPanel, hideBottomStatusBar;
            public int iconsSize;
            public double dLeftPanelSize, dRightPanelSize;
        }
        */
        public List<Cell> lCells = new List<Cell>();
        public enum CellType { None, Finish, Normal, Ice };
        public class Cell
        {
            public int PosX { get; set; }
            public int PosY { get; set; }
            public CellType Type { get; set; }
            public int Number { get; set; }
        }
        public MainWindow()
        {
            //globalProgConf = ReadConfigFile();
            ChangeCultureInfo();
            InitializeComponent();
            SetLangIndicator();
            bAfterInitial = true;
            Cell tempCell = new Cell();
            tempCell.PosX = 0; tempCell.PosY = 0; tempCell.Number = 1; tempCell.Type = CellType.Normal;
            lCells.Add(tempCell);
            lvUsers.ItemsSource = lCells;
            UpdateMainGridView();
            SetWindowsVisableElement();
        }
        bool bAfterInitial = false;
        private void SetLangIndicator()
        {
            string langStr = Thread.CurrentThread.CurrentCulture.ToString();
            if (langStr.Contains("pl-PL"))
            {
                miLangPol.IsChecked = true; miLangEng.IsChecked = false;
            }
            else if (langStr.Contains("en-US"))
            {
                miLangPol.IsChecked = false; miLangEng.IsChecked = true;
            }
            else
            {
                miLangPol.IsChecked = false; miLangEng.IsChecked = true;
            }
        }
        private void ChangeCultureInfo()
        {
            CultureInfo ciReturnCulture = new CultureInfo("en-US");
            CultureInfo ciFromSettings = Settings.Default.selectedLanguage;
            CultureInfo ciThisPc = Thread.CurrentThread.CurrentCulture;
            if(ciFromSettings.ToString() == "")
            {
                CultureInfo ciTemp = new CultureInfo("pl-PL");
                if (ciThisPc.ToString() == ciTemp.ToString())
                {
                    ciReturnCulture = CultureInfo.GetCultureInfo("pl-PL");
                }
                else
                {
                    ciReturnCulture = CultureInfo.GetCultureInfo("en-US");
                }
            }
            else if (ciFromSettings.ToString() == CultureInfo.GetCultureInfo("pl-PL").ToString()) ciReturnCulture = CultureInfo.GetCultureInfo("pl-PL");
            else ciReturnCulture = CultureInfo.GetCultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = ciReturnCulture;
            Thread.CurrentThread.CurrentUICulture = ciReturnCulture;
            Settings.Default.selectedLanguage = ciReturnCulture;
        }
        /*
        private ProgramConfiguration ReadConfigFile()
        {
            ProgramConfiguration readData = new ProgramConfiguration();
            try
            {
                XmlSerializer xs = new XmlSerializer(globalProgConf.GetType());
                FileStream tw = new FileStream(pathConfigFile, FileMode.Open);
                readData = (ProgramConfiguration)xs.Deserialize(tw);
                tw.Close();
            }
            catch
            {
                readData.dLeftPanelSize = dDefoulWidthLeftPanel;
                readData.dRightPanelSize = dDefoulWidthRightPanel;
                readData.SelectedLanguage = Thread.CurrentThread.CurrentCulture.ToString();                
                Xceed.Wpf.Toolkit.MessageBox.Show(Lang.sErrorConfigFileNoExistMessage, Lang.sErrorConfigFileNoExistTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            return readData;
        }
        */
        private void SetWindowsVisableElement()
        {
            switch (Settings.Default.iconsSize)
            {
                case 16:
                    miIconSize16.IsChecked = true;
                    miIconSize24.IsChecked = false;
                    miIconSize32.IsChecked = false;
                    miIconSize48.IsChecked = false;
                    ChangeIconSizeTo();
                    break;
                case 24:
                    miIconSize16.IsChecked = false;
                    miIconSize24.IsChecked = true;
                    miIconSize32.IsChecked = false;
                    miIconSize48.IsChecked = false;
                    ChangeIconSizeTo();
                    break;
                case 32:
                    miIconSize16.IsChecked = false;
                    miIconSize24.IsChecked = false;
                    miIconSize32.IsChecked = true;
                    miIconSize48.IsChecked = false;
                    ChangeIconSizeTo();
                    break;
                case 48:
                    miIconSize16.IsChecked = false;
                    miIconSize24.IsChecked = false;
                    miIconSize32.IsChecked = false;
                    miIconSize48.IsChecked = true;
                    ChangeIconSizeTo();
                    break;
                default:
                    miIconSize16.IsChecked = true;
                    miIconSize24.IsChecked = false;
                    miIconSize32.IsChecked = false;
                    miIconSize48.IsChecked = false;
                    Settings.Default.iconsSize = 24;
                    Settings.Default.Save();
                    break;
            }
            miShowToolBar.IsChecked = Settings.Default.showToolBar;
            if (Settings.Default.showToolBar == true)
            {
                mainToolBar.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                mainToolBar.Visibility = System.Windows.Visibility.Collapsed;
            }
            miShowLeftPanel.IsChecked = Settings.Default.showLeftPanel;
            prevAcutalWidthLeft = Settings.Default.leftPanelSize;
            if (Settings.Default.showLeftPanel == true)
            {
                gMainGrid.ColumnDefinitions[0].Width = new GridLength(prevAcutalWidthLeft);
                gMainGrid.ColumnDefinitions[1].Width = new GridLength();
                gMainGrid.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                gMainGrid.ColumnDefinitions[0].Width = new GridLength(0);
                gMainGrid.ColumnDefinitions[1].Width = new GridLength(0);
                gMainGrid.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
            }
            miShowRightPanel.IsChecked = Settings.Default.showRightPanel;
            prevAcutalWidthRight = Settings.Default.rightPanelSize;
            if (Settings.Default.showRightPanel == true)
            {
                gMainGrid.ColumnDefinitions[4].Width = new GridLength(prevAcutalWidthRight);
                gMainGrid.ColumnDefinitions[3].Width = new GridLength();
                gMainGrid.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                gMainGrid.ColumnDefinitions[4].Width = new GridLength(0);
                gMainGrid.ColumnDefinitions[3].Width = new GridLength(0);
                gMainGrid.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
            }
            miShowStatusBar.IsChecked = Settings.Default.showBottomStatusBar;
            if (Settings.Default.showBottomStatusBar == true)
            {
                sbBottomInfo.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                sbBottomInfo.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
        private void UpdateMainGridView()
        {
            gMainPlaceGrid.Children.Clear();
            gMainPlaceGrid.RowDefinitions.Clear();
            gMainPlaceGrid.ColumnDefinitions.Clear();
            int iColNo = 1 + iudAreaViewDimMaxX.Value.Value - iudAreaViewDimMinX.Value.Value;
            int iRowNo = 1 + iudAreaViewDimMaxY.Value.Value - iudAreaViewDimMinY.Value.Value;            
            for(int i = 0; i < iColNo; i++)
            {
                ColumnDefinition gridCol = new ColumnDefinition();
                gMainPlaceGrid.ColumnDefinitions.Add(gridCol);
            }            
            for (int i = 0; i < iRowNo; i++)
            {
                RowDefinition gridRow = new RowDefinition();
                gMainPlaceGrid.RowDefinitions.Add(gridRow);
            }
            for (int i = 0; i < iColNo; i++)
            {
                for (int j = 0; j< iRowNo; j++)
                {
                    Button bTemp = new Button();
                    Point nePoint = new Point(iudAreaViewDimMinX.Value.Value + i, iudAreaViewDimMinY.Value.Value + j);
                    bTemp.Width = 60; bTemp.Height = 60; bTemp.ToolTip = nePoint;
                    Grid.SetColumn(bTemp, i);
                    Grid.SetRow(bTemp, iRowNo - j - 1);
                    bTemp.Click += bCommon_Click;
                    Cell cResult = lCells.Find(x => x.PosX == nePoint.X && x.PosY == nePoint.Y);
                    if (cResult != null)
                    {
                        if (cResult.Type == CellType.Normal)
                        {
                            Grid gr = new Grid();
                            Image img = new Image();
                            img.Source = new BitmapImage(new Uri(@"/Graphics/Cells/Normal_48.png", UriKind.Relative));
                            TextBlock lab = new TextBlock();
                            lab.HorizontalAlignment = HorizontalAlignment.Center;
                            lab.VerticalAlignment = VerticalAlignment.Center;
                            lab.TextAlignment = TextAlignment.Center;
                            if (nePoint == new Point(0, 0))
                            {
                                lab.Text = Lang.sStart + "\n" + cResult.Number.ToString();
                                lab.FontSize = 18;
                                lab.Foreground = new SolidColorBrush(Colors.Green);
                            }
                            else
                            {
                                lab.Text = cResult.Number.ToString();
                                lab.FontSize = 30;
                            }                            
                            gr.Children.Add(img);
                            gr.Children.Add(lab);
                            bTemp.Content = gr;
                        }
                        else if(cResult.Type == CellType.Ice)
                        {
                            Grid gr = new Grid();
                            Image img = new Image();
                            img.Source = new BitmapImage(new Uri(@"/Graphics/Cells/Ice_48.png", UriKind.Relative));
                            TextBlock lab = new TextBlock();
                            lab.HorizontalAlignment = HorizontalAlignment.Center;
                            lab.VerticalAlignment = VerticalAlignment.Center;
                            lab.TextAlignment = TextAlignment.Center;
                            if (nePoint == new Point(0, 0))
                            {
                                lab.Text = Lang.sStart + "\n" + cResult.Number.ToString();
                                lab.FontSize = 18;
                                lab.Foreground = new SolidColorBrush(Colors.Green);
                            }
                            else
                            {
                                lab.Text = cResult.Number.ToString();
                                lab.FontSize = 30;
                            }
                            gr.Children.Add(img);
                            gr.Children.Add(lab);
                            bTemp.Content = gr;
                        }
                        else
                        {
                            Image img = new Image();
                            img.Source = new BitmapImage(new Uri(@"/Graphics/Cells/Fin_48.png", UriKind.Relative));
                            bTemp.Content = img;
                        }
                    }
                    bTemp.MouseEnter += Button_MouseEnter;
                    gMainPlaceGrid.Children.Add(bTemp);
                }
            }
        }
        private void bCommon_Click(object sender, RoutedEventArgs e)
        {
            Button bTemo = (Button)sender;
            Point pTemp = (Point)bTemo.ToolTip;
            if (currSelectedCell == SelectedCell.normal)
            {
                Grid gr = new Grid();
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(@"/Graphics/Cells/Normal_48.png", UriKind.Relative));
                TextBlock lab = new TextBlock();
                lab.HorizontalAlignment = HorizontalAlignment.Center;
                lab.VerticalAlignment = VerticalAlignment.Center;
                lab.TextAlignment = TextAlignment.Center;
                if (pTemp == new Point(0, 0))
                {
                    lab.Text = Lang.sStart + "\n" + iNoFromTag.ToString();
                    lab.FontSize = 18;
                    lab.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    lab.Text = iNoFromTag.ToString();
                    lab.FontSize = 30;
                }
                gr.Children.Add(img);
                gr.Children.Add(lab);
                bTemo.Content = gr;
                sender = bTemo;
                Cell tempCell = new Cell();
                tempCell.PosX = (int)pTemp.X; tempCell.PosY = (int)pTemp.Y; tempCell.Number = iNoFromTag; tempCell.Type = CellType.Normal;
                int iListIndex = lCells.FindIndex(c => (c.PosX == pTemp.X && c.PosY == pTemp.Y));
                if (iListIndex != -1)
                {
                    lCells[iListIndex] = tempCell;
                }
                else
                {
                    lCells.Add(tempCell);
                }
            }
            else if (currSelectedCell == SelectedCell.normalInc)
            {
                int iListIndex = lCells.FindIndex(c => (c.PosX == pTemp.X && c.PosY == pTemp.Y));
                Cell tempCell = new Cell();
                if (iListIndex == -1)
                {
                    tempCell.PosX = (int)pTemp.X; tempCell.PosY = (int)pTemp.Y; tempCell.Number = 1; tempCell.Type = CellType.Normal;
                    lCells.Add(tempCell);
                }
                else
                {
                    if (lCells[iListIndex].Type == CellType.Finish)
                    {
                        tempCell.PosX = (int)pTemp.X; tempCell.PosY = (int)pTemp.Y; tempCell.Number = 1; tempCell.Type = CellType.Normal;
                        lCells[iListIndex] = tempCell;
                    }
                    else if (lCells[iListIndex].Type == CellType.Normal || lCells[iListIndex].Type == CellType.Ice)
                    {
                        if (lCells[iListIndex].Number >= 4)
                        {
                            return;
                        }
                        else
                        {
                            tempCell = new Cell();
                            tempCell.PosX = (int)pTemp.X; tempCell.PosY = (int)pTemp.Y; tempCell.Number = lCells[iListIndex].Number + 1; tempCell.Type = CellType.Normal;
                            lCells[iListIndex] = tempCell;
                        }
                    }
                }
                Grid gr = new Grid();
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(@"/Graphics/Cells/Normal_48.png", UriKind.Relative));
                TextBlock lab = new TextBlock();
                lab.HorizontalAlignment = HorizontalAlignment.Center;
                lab.VerticalAlignment = VerticalAlignment.Center;
                lab.TextAlignment = TextAlignment.Center;
                if (pTemp == new Point(0, 0))
                {
                    lab.Text = Lang.sStart + "\n" + tempCell.Number.ToString();
                    lab.FontSize = 18;
                    lab.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    lab.Text = tempCell.Number.ToString();
                    lab.FontSize = 30;
                }
                gr.Children.Add(img);
                gr.Children.Add(lab);
                bTemo.Content = gr;
                sender = bTemo;
            }
            else if (currSelectedCell == SelectedCell.normalDec)
            {
                int iListIndex = lCells.FindIndex(c => (c.PosX == pTemp.X && c.PosY == pTemp.Y));
                Cell tempCell = new Cell();
                if (iListIndex == -1 || lCells[iListIndex].Type == CellType.Finish)
                {
                    return;
                }
                else
                {
                    if (lCells[iListIndex].Type == CellType.Normal || lCells[iListIndex].Type == CellType.Ice)
                    {
                        if (lCells[iListIndex].Number <= 1)
                        {
                            if (pTemp == new Point(0, 0))
                            {
                                return;
                            }
                            else
                            {
                                bTemo.Content = null;
                                sender = bTemo;
                                lCells.RemoveAt(iListIndex);
                            }
                        }
                        else
                        {
                            tempCell = new Cell();
                            tempCell.PosX = (int)pTemp.X; tempCell.PosY = (int)pTemp.Y; tempCell.Number = lCells[iListIndex].Number - 1; tempCell.Type = CellType.Normal;
                            lCells[iListIndex] = tempCell;
                            Grid gr = new Grid();
                            Image img = new Image();
                            img.Source = new BitmapImage(new Uri(@"/Graphics/Cells/Normal_48.png", UriKind.Relative));
                            TextBlock lab = new TextBlock();
                            lab.HorizontalAlignment = HorizontalAlignment.Center;
                            lab.VerticalAlignment = VerticalAlignment.Center;
                            lab.TextAlignment = TextAlignment.Center;
                            if (pTemp == new Point(0, 0))
                            {
                                lab.Text = Lang.sStart + "\n" + tempCell.Number.ToString();
                                lab.FontSize = 18;
                                lab.Foreground = new SolidColorBrush(Colors.Green);
                            }
                            else
                            {
                                lab.Text = tempCell.Number.ToString();
                                lab.FontSize = 30;
                            }
                            gr.Children.Add(img);
                            gr.Children.Add(lab);
                            bTemo.Content = gr;
                            sender = bTemo;
                        }
                    }
                }
            }
            else if (currSelectedCell == SelectedCell.ice)
            {
                Grid gr = new Grid();
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(@"/Graphics/Cells/Ice_48.png", UriKind.Relative));
                TextBlock lab = new TextBlock();
                lab.HorizontalAlignment = HorizontalAlignment.Center;
                lab.VerticalAlignment = VerticalAlignment.Center;
                lab.TextAlignment = TextAlignment.Center;
                if (pTemp == new Point(0, 0))
                {
                    lab.Text = Lang.sStart + "\n" + iNoFromTag.ToString();
                    lab.FontSize = 18;
                    lab.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    lab.Text = iNoFromTag.ToString();
                    lab.FontSize = 30;
                }
                gr.Children.Add(img);
                gr.Children.Add(lab);
                bTemo.Content = gr;
                sender = bTemo;
                Cell tempCell = new Cell();
                tempCell.PosX = (int)pTemp.X; tempCell.PosY = (int)pTemp.Y; tempCell.Number = iNoFromTag; tempCell.Type = CellType.Ice;
                int iListIndex = lCells.FindIndex(c => (c.PosX == pTemp.X && c.PosY == pTemp.Y));
                if (iListIndex != -1)
                {
                    lCells[iListIndex] = tempCell;
                }
                else
                {
                    lCells.Add(tempCell);
                }
            }
            else if (currSelectedCell == SelectedCell.iceInc)
            {
                int iListIndex = lCells.FindIndex(c => (c.PosX == pTemp.X && c.PosY == pTemp.Y));
                Cell tempCell = new Cell();
                if (iListIndex == -1)
                {
                    tempCell.PosX = (int)pTemp.X; tempCell.PosY = (int)pTemp.Y; tempCell.Number = 1; tempCell.Type = CellType.Ice;
                    lCells.Add(tempCell);
                }
                else
                {
                    if (lCells[iListIndex].Type == CellType.Finish)
                    {
                        tempCell.PosX = (int)pTemp.X; tempCell.PosY = (int)pTemp.Y; tempCell.Number = 1; tempCell.Type = CellType.Ice;
                        lCells[iListIndex] = tempCell;
                    }
                    else if (lCells[iListIndex].Type == CellType.Normal || lCells[iListIndex].Type == CellType.Ice)
                    {
                        if (lCells[iListIndex].Number >= 4)
                        {
                            return;
                        }
                        else
                        {
                            tempCell = new Cell();
                            tempCell.PosX = (int)pTemp.X; tempCell.PosY = (int)pTemp.Y; tempCell.Number = lCells[iListIndex].Number + 1; tempCell.Type = CellType.Ice;
                            lCells[iListIndex] = tempCell;
                        }
                    }
                }
                Grid gr = new Grid();
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(@"/Graphics/Cells/Ice_48.png", UriKind.Relative));
                TextBlock lab = new TextBlock();
                lab.HorizontalAlignment = HorizontalAlignment.Center;
                lab.VerticalAlignment = VerticalAlignment.Center;
                lab.TextAlignment = TextAlignment.Center;
                if (pTemp == new Point(0, 0))
                {
                    lab.Text = Lang.sStart + "\n" + tempCell.Number.ToString();
                    lab.FontSize = 18;
                    lab.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    lab.Text = tempCell.Number.ToString();
                    lab.FontSize = 30;
                }
                gr.Children.Add(img);
                gr.Children.Add(lab);
                bTemo.Content = gr;
                sender = bTemo;
            }
            else if (currSelectedCell == SelectedCell.iceDec)
            {
                int iListIndex = lCells.FindIndex(c => (c.PosX == pTemp.X && c.PosY == pTemp.Y));
                Cell tempCell = new Cell();
                if (iListIndex == -1 || lCells[iListIndex].Type == CellType.Finish)
                {
                    return;
                }
                else
                {
                    if (lCells[iListIndex].Type == CellType.Normal || lCells[iListIndex].Type == CellType.Ice)
                    {
                        if (lCells[iListIndex].Number <= 1)
                        {
                            if (pTemp == new Point(0, 0))
                            {
                                return;
                            }
                            else
                            {
                                bTemo.Content = null;
                                sender = bTemo;
                                lCells.RemoveAt(iListIndex);
                            }
                        }
                        else
                        {
                            tempCell = new Cell();
                            tempCell.PosX = (int)pTemp.X; tempCell.PosY = (int)pTemp.Y; tempCell.Number = lCells[iListIndex].Number - 1; tempCell.Type = CellType.Ice;
                            lCells[iListIndex] = tempCell;
                            Grid gr = new Grid();
                            Image img = new Image();
                            img.Source = new BitmapImage(new Uri(@"/Graphics/Cells/Ice_48.png", UriKind.Relative));
                            TextBlock lab = new TextBlock();
                            lab.HorizontalAlignment = HorizontalAlignment.Center;
                            lab.VerticalAlignment = VerticalAlignment.Center;
                            lab.TextAlignment = TextAlignment.Center;
                            if (pTemp == new Point(0, 0))
                            {
                                lab.Text = Lang.sStart + "\n" + tempCell.Number.ToString();
                                lab.FontSize = 18;
                                lab.Foreground = new SolidColorBrush(Colors.Green);
                            }
                            else
                            {
                                lab.Text = tempCell.Number.ToString();
                                lab.FontSize = 30;
                            }
                            gr.Children.Add(img);
                            gr.Children.Add(lab);
                            bTemo.Content = gr;
                            sender = bTemo;
                        }
                    }
                }
            }
            else if (currSelectedCell == SelectedCell.fin)
            {
                if (pTemp == new Point(0, 0))
                {

                }
                else
                {
                    Image img = new Image();
                    img.Source = new BitmapImage(new Uri(@"/Graphics/Cells/Fin_48.png", UriKind.Relative));
                    bTemo.Content = img;
                    sender = bTemo;
                    Cell tempCell = new Cell();
                    tempCell.PosX = (int)pTemp.X; tempCell.PosY = (int)pTemp.Y; tempCell.Number = 0; tempCell.Type = CellType.Finish;
                    int iListIndex = lCells.FindIndex(c => (c.PosX == pTemp.X && c.PosY == pTemp.Y));
                    if (iListIndex != -1)
                    {
                        lCells[iListIndex] = tempCell;
                    }
                    else
                    {
                        lCells.Add(tempCell);
                    }
                }
            }
            else if (currSelectedCell == SelectedCell.delete)
            {
                if (pTemp == new Point(0, 0))
                {

                }
                else
                {
                    int iListIndex = lCells.FindIndex(c => (c.PosX == pTemp.X && c.PosY == pTemp.Y));
                    if (iListIndex != -1)
                    {
                        bTemo.Content = null;
                        sender = bTemo;
                        lCells.RemoveAt(iListIndex);
                    }
                }
            }
            tbElementCount.Text = lCells.Count.ToString();
            lvUsers.ItemsSource = null;
            lvUsers.ItemsSource = lCells;
            CalcLevelSize();
        }
        private void CalcLevelSize()
        {
            tbInfoMinX.Text = lCells.Min(c => c.PosX).ToString();
            tbInfoMaxX.Text = lCells.Max(c => c.PosX).ToString();
            tbInfoMinY.Text = lCells.Min(c => c.PosY).ToString();
            tbInfoMaxY.Text = lCells.Max(c => c.PosY).ToString();
        }
        private void miNewProject_Click(object sender, RoutedEventArgs e)
        {
            bAfterInitial = false;
            projectName = "";
            projectPath = "";
            this.Title = Lang.sFlipBlock3D_LevelEditor;
            sbiProjectPath.Text = projectPath;
            lCells.Clear();            
            Cell tempCell = new Cell();
            tempCell.PosX = 0; tempCell.PosY = 0; tempCell.Number = 1; tempCell.Type = CellType.Normal;
            lCells.Add(tempCell);
            lvUsers.ItemsSource = null;
            lvUsers.ItemsSource = lCells;
            tbElementCount.Text = lCells.Count.ToString();
            CalcLevelSize();
            iudAreaViewDimMinX.Value = -1;
            iudAreaViewDimMaxX.Value = 1;
            iudAreaViewDimMinY.Value = -1;
            iudAreaViewDimMaxY.Value = 1;
            UpdateMainGridView();
            bAfterInitial = true;
            Xceed.Wpf.Toolkit.MessageBox.Show(Lang.sInfoOpenLevelConfirmationMessage, Lang.sInfoOpenLevelConfirmationTittle, MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }
        private void miOpenProject_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog textDialogOpen = new System.Windows.Forms.OpenFileDialog();
            textDialogOpen.Filter = Lang.sGameLevel +" | *.xml";
            System.Windows.Forms.DialogResult result = textDialogOpen.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                List<Cell> objectOut = default(List<Cell>);
                try
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(textDialogOpen.FileName);
                    string xmlString = xmlDocument.OuterXml;
                    using (StringReader read = new StringReader(xmlString))
                    {
                        Type outType = typeof(List<Cell>);

                        XmlSerializer serializer = new XmlSerializer(outType);
                        using (XmlReader reader = new XmlTextReader(read))
                        {
                            lCells = (List<Cell>)serializer.Deserialize(reader);
                            reader.Close();
                        }
                        read.Close();
                    }
                    projectName = Path.GetFileNameWithoutExtension(textDialogOpen.FileName);
                    projectPath = Path.GetDirectoryName(textDialogOpen.FileName);
                    tbElementCount.Text = lCells.Count.ToString();
                    this.Title = Lang.sFlipBlock3D_LevelEditor + " [" + projectName + "]";
                    sbiProjectPath.Text = projectPath;
                    bAfterInitial = false;
                    iudAreaViewDimMinX.Value = lCells.Min(c => c.PosX);
                    iudAreaViewDimMaxX.Value = lCells.Max(c => c.PosX);
                    iudAreaViewDimMinY.Value = lCells.Min(c => c.PosY);
                    iudAreaViewDimMaxY.Value = lCells.Max(c => c.PosY);
                    tbInfoMinX.Text = iudAreaViewDimMinX.Value.Value.ToString();
                    tbInfoMaxX.Text = iudAreaViewDimMaxX.Value.Value.ToString();
                    tbInfoMinY.Text = iudAreaViewDimMinY.Value.Value.ToString();
                    tbInfoMaxY.Text = iudAreaViewDimMaxY.Value.Value.ToString();
                    bAfterInitial = true;
                    UpdateMainGridView();
                    CalcLevelSize();
                    lvUsers.ItemsSource = null;
                    lvUsers.ItemsSource = lCells;
                    Xceed.Wpf.Toolkit.MessageBox.Show(Lang.sInfoOpenLevelConfirmationMessage, Lang.sInfoOpenLevelConfirmationTittle, MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
                catch (Exception ex)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show(Lang.sErrorOpenLevelMessage, Lang.sErrorOpenLevelTittle, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
                
        }
        private void miSaveProject_Click(object sender, RoutedEventArgs e)
        {
            if (projectName == "" || projectPath == "")
            {
                SaveAs();
            }
            else
            {
                bool bRetAfterSave = SaveProject(lCells, projectName, projectPath);
                if (bRetAfterSave == true)
                {
                    this.Title = Lang.sFlipBlock3D_LevelEditor + " [" + projectName + "]";
                    sbiProjectPath.Text = projectPath;
                    Xceed.Wpf.Toolkit.MessageBox.Show(Lang.sInfoSaveLevelMessage, Lang.sInfoSaveLevelTittle, MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
                else
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show(Lang.sErrorSaveLevelMessage, Lang.sErrorSaveLevelTittle, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void miSaveAsProject_Click(object sender, RoutedEventArgs e)
        {
            SaveAs();
        }
        private void miExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void miShowToolBar_Click(object sender, RoutedEventArgs e)
        {
            if (miShowToolBar.IsChecked)
            {
                mainToolBar.Visibility = System.Windows.Visibility.Visible;
                Settings.Default.showToolBar = true;
            }
            else
            {
                mainToolBar.Visibility = System.Windows.Visibility.Collapsed;
                Settings.Default.showToolBar = false;
            }
            Settings.Default.Save();
        }
        double prevAcutalWidthLeft = 0;
        private void miShowLeftPanel_Click(object sender, RoutedEventArgs e)
        {
            if (miShowLeftPanel.IsChecked)
            {
                gMainGrid.ColumnDefinitions[0].Width = new GridLength(prevAcutalWidthLeft);
                gMainGrid.ColumnDefinitions[1].Width = new GridLength();
                Settings.Default.showLeftPanel = true;
            }
            else
            {
                prevAcutalWidthLeft = gMainGrid.ColumnDefinitions[0].ActualWidth;
                gMainGrid.ColumnDefinitions[0].Width = new GridLength(0);
                gMainGrid.ColumnDefinitions[1].Width = new GridLength(0);
                Settings.Default.showLeftPanel = false;
            }
            Settings.Default.Save();
        }
        double prevAcutalWidthRight = 0;
        private void miShowRightPanel_Click(object sender, RoutedEventArgs e)
        {
            if (miShowRightPanel.IsChecked)
            {
                gMainGrid.ColumnDefinitions[4].Width = new GridLength(prevAcutalWidthRight);
                gMainGrid.ColumnDefinitions[3].Width = new GridLength();
                Settings.Default.showRightPanel = true;
            }
            else
            {
                prevAcutalWidthRight = gMainGrid.ColumnDefinitions[4].ActualWidth;
                gMainGrid.ColumnDefinitions[4].Width = new GridLength(0);
                gMainGrid.ColumnDefinitions[3].Width = new GridLength(0);
                Settings.Default.showRightPanel = false;
            }
            Settings.Default.Save();
        }
        private void miShowStatusBar_Click(object sender, RoutedEventArgs e)
        {
            if (miShowStatusBar.IsChecked)
            {
                sbBottomInfo.Visibility = System.Windows.Visibility.Visible;
                Settings.Default.showBottomStatusBar = true;
            }
            else
            {
                sbBottomInfo.Visibility = System.Windows.Visibility.Collapsed;
                Settings.Default.showBottomStatusBar = false;
            }
            Settings.Default.Save();
        }
        private void miIconSize16_Click(object sender, RoutedEventArgs e)
        {
            miIconSize16.IsChecked = true;
            miIconSize24.IsChecked = false;
            miIconSize32.IsChecked = false;
            miIconSize48.IsChecked = false;
            Settings.Default.iconsSize = 16;
            Settings.Default.Save();
            ChangeIconSizeTo();
        }
        private void miIconSize24_Click(object sender, RoutedEventArgs e)
        {
            miIconSize16.IsChecked = false;
            miIconSize24.IsChecked = true;
            miIconSize32.IsChecked = false;
            miIconSize48.IsChecked = false;
            Settings.Default.iconsSize = 24;
            Settings.Default.Save();
            ChangeIconSizeTo();
        }
        private void miIconSize32_Click(object sender, RoutedEventArgs e)
        {
            miIconSize16.IsChecked = false;
            miIconSize24.IsChecked = false;
            miIconSize32.IsChecked = true;
            miIconSize48.IsChecked = false;
            Settings.Default.iconsSize = 32;
            Settings.Default.Save();
            ChangeIconSizeTo();
        }
        private void miIconSize48_Click(object sender, RoutedEventArgs e)
        {
            miIconSize16.IsChecked = false;
            miIconSize24.IsChecked = false;
            miIconSize32.IsChecked = false;
            miIconSize48.IsChecked = true;
            Settings.Default.iconsSize = 48;
            Settings.Default.Save();
            ChangeIconSizeTo();
        }
        private void ChangeIconSizeTo()
        {
            int width = Settings.Default.iconsSize;
            int height = Settings.Default.iconsSize;
            vNewProject.Width = width; vNewProject.Height = height;
            vOpenProject.Width = width; vOpenProject.Height = height;
            vSaveProject.Width = width; vSaveProject.Height = height;
            vSaveAsProject.Width = width; vSaveAsProject.Height = height;
            vSetFefoultView.Width = width; vSetFefoultView.Height = height;
            vResizeViewToAll.Width = width; vResizeViewToAll.Height = height;
            vDeleteAllCells.Width = width; vDeleteAllCells.Height = height;
            vOpenManual.Width = width; vOpenManual.Height = height;
            vAboutProgram.Width = width; vAboutProgram.Height = height;
            
            vMoveViewLeftUp.Width = width; vMoveViewLeftUp.Height = height;
            vMoveViewUp.Width = width; vMoveViewUp.Height = height;
            vMoveViewRightUp.Width = width; vMoveViewRightUp.Height = height;            
            vMoveViewLeft.Width = width; vMoveViewLeft.Height = height;
            vMoveViewRight.Width = width; vMoveViewRight.Height = height;
            vMoveViewLeftDown.Width = width; vMoveViewLeftDown.Height = height;
            vMoveViewDown.Width = width; vMoveViewDown.Height = height;
            vMoveViewRightDown.Width = width; vMoveViewRightDown.Height = height;

            vZoomPlus.Width = width; vZoomPlus.Height = height;
            vZoomMinus.Width = width; vZoomMinus.Height = height;
        }
        private void miOpenManual_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string fileName = "";
                if (miLangPol.IsChecked)
                {
                    fileName = "Flip Block 3D - Edytor Poziomów (Poradnik Użytkownika).pdf";
                }
                else
                {
                    fileName = "Flip Block 3D - Level Editor (User Manual).pdf";
                }
                string path = Path.Combine(Environment.CurrentDirectory, @"Manuals\", fileName);
                System.Diagnostics.Process.Start(path);
            }
            catch
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(Lang.sErrorManualOpenMessage, Lang.sErrorManualOpenTittle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void miAboutProgram_Click(object sender, RoutedEventArgs e)
        {
            ShowVersionInformation();
        }
        public void ShowVersionInformation()
        {
            string sProgramVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string sReleseData = "2018.06.07";
            string insaidInfo = Lang.sFlipBlock3D_LevelEditor + "\n" + Lang.sVersionNumber + ": " + sProgramVersion + "\n" + Lang.sReleaseDate + ": " + sReleseData;
            Xceed.Wpf.Toolkit.MessageBox.Show(insaidInfo, Lang.sAboutProgram, MessageBoxButton.OK, MessageBoxImage.Information);
            /*
            Xceed.Wpf.Toolkit.MessageBox verionInfoMessageBox = new Xceed.Wpf.Toolkit.MessageBox();
            verionInfoMessageBox.ImageSource = new BitmapImage(new Uri(String.Format("/FlipBlock3D_LevelEditor;component/Graphics/ProgramIcon_128.png"), UriKind.RelativeOrAbsolute));
            verionInfoMessageBox.Caption = "About Program";
            verionInfoMessageBox.Text = insaidInfo;
            verionInfoMessageBox.ShowDialog();
            */
        }
        enum SelectedCell { none, normal, normalInc, normalDec, ice, iceInc, iceDec, fin, delete};
        SelectedCell currSelectedCell = SelectedCell.none;
        int iNoFromTag = 0;
        private void bCellNormal_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border bTemp = (Border)sender;
            int iTempTag = Convert.ToInt16(bTemp.Tag.ToString());
            if (currSelectedCell != SelectedCell.normal || iTempTag != iNoFromTag)
            {
                currSelectedCell = SelectedCell.normal;
                iNoFromTag = iTempTag;
            }
            else
            {
                currSelectedCell = SelectedCell.none;
            }
            UpdateCellBorder();
        }
        private void bCellNormalInc_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currSelectedCell != SelectedCell.normalInc)
            {
                currSelectedCell = SelectedCell.normalInc;
            }
            else
            {
                currSelectedCell = SelectedCell.none;
            }
            UpdateCellBorder();
        }
        private void bCellNormalDec_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currSelectedCell != SelectedCell.normalDec)
            {
                currSelectedCell = SelectedCell.normalDec;
            }
            else
            {
                currSelectedCell = SelectedCell.none;
            }
            UpdateCellBorder();
        }
        private void bCellIce_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border bTemp = (Border)sender;
            int iTempTag = Convert.ToInt16(bTemp.Tag.ToString());
            if (currSelectedCell != SelectedCell.ice || iTempTag != iNoFromTag)
            {
                currSelectedCell = SelectedCell.ice;
                iNoFromTag = iTempTag;
            }
            else
            {
                currSelectedCell = SelectedCell.none;
            }
            UpdateCellBorder();
        }
        private void bCellIceInc_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currSelectedCell != SelectedCell.iceInc)
            {
                currSelectedCell = SelectedCell.iceInc;
            }
            else
            {
                currSelectedCell = SelectedCell.none;
            }
            UpdateCellBorder();
        }
        private void bCellIceDec_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currSelectedCell != SelectedCell.iceDec)
            {
                currSelectedCell = SelectedCell.iceDec;
            }
            else
            {
                currSelectedCell = SelectedCell.none;
            }
            UpdateCellBorder();
        }
        private void bCellFin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currSelectedCell != SelectedCell.fin)
            {
                currSelectedCell = SelectedCell.fin;
            }
            else
            {
                currSelectedCell = SelectedCell.none;
            }
            UpdateCellBorder();
        }
        private void bDelete_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currSelectedCell != SelectedCell.delete)
            {
                currSelectedCell = SelectedCell.delete;
            }
            else
            {
                currSelectedCell = SelectedCell.none;
            }
            UpdateCellBorder();
        }
        private void UpdateCellBorder()
        {
            SolidColorBrush scbWhite = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            SolidColorBrush scbGreen = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            bCellNormal1.BorderBrush = scbWhite;
            bCellNormal2.BorderBrush = scbWhite;
            bCellNormal3.BorderBrush = scbWhite;
            bCellNormal4.BorderBrush = scbWhite;
            bCellNormalInc.BorderBrush = scbWhite;
            bCellNormalDec.BorderBrush = scbWhite;
            bCellIce1.BorderBrush = scbWhite;
            bCellIce2.BorderBrush = scbWhite;
            bCellIce3.BorderBrush = scbWhite;
            bCellIce4.BorderBrush = scbWhite;
            bCellIceDec.BorderBrush = scbWhite;
            bCellIceInc.BorderBrush = scbWhite;
            bCellFin.BorderBrush = scbWhite;
            bDelete.BorderBrush = scbWhite;
            if (currSelectedCell == SelectedCell.normal)
            {
                switch (iNoFromTag)
                {
                    case 1:
                        bCellNormal1.BorderBrush = scbGreen;
                        break;
                    case 2:
                        bCellNormal2.BorderBrush = scbGreen;
                        break;
                    case 3:
                        bCellNormal3.BorderBrush = scbGreen;
                        break;
                    case 4:
                        bCellNormal4.BorderBrush = scbGreen;
                        break;
                }
            }
            else if(currSelectedCell == SelectedCell.normalInc)
            {
                bCellNormalInc.BorderBrush = scbGreen;
            }
            else if(currSelectedCell == SelectedCell.normalDec)
            {
                bCellNormalDec.BorderBrush = scbGreen;
            }
            else if(currSelectedCell == SelectedCell.ice)
            {
                switch (iNoFromTag)
                {
                    case 1:
                        bCellIce1.BorderBrush = scbGreen;
                        break;
                    case 2:
                        bCellIce2.BorderBrush = scbGreen;
                        break;
                    case 3:
                        bCellIce3.BorderBrush = scbGreen;
                        break;
                    case 4:
                        bCellIce4.BorderBrush = scbGreen;
                        break;
                }
            }
            else if (currSelectedCell == SelectedCell.iceDec)
            {
                bCellIceDec.BorderBrush = scbGreen;
            }
            else if (currSelectedCell == SelectedCell.iceInc)
            {
                bCellIceInc.BorderBrush = scbGreen;
            }
            else if (currSelectedCell == SelectedCell.fin)
            {
                bCellFin.BorderBrush = scbGreen;
            }
            else if (currSelectedCell == SelectedCell.delete)
            {
                bDelete.BorderBrush = scbGreen;
            }
        }
        private void iudAreaViewDim_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if(bAfterInitial)
            {
                UpdateMainGridView();
            }            
        }
        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            Button bTemo = (Button)sender;
            tbCurrentGrid.Text = bTemo.ToolTip.ToString();
        }
        private void SaveAs()
        {
            System.Windows.Forms.SaveFileDialog textDialogSave = new System.Windows.Forms.SaveFileDialog();
            textDialogSave.Filter = Lang.sGameLevel + " | *.xml";
            System.Windows.Forms.DialogResult result = textDialogSave.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                bool bRetAfterSave = SaveProject(lCells, Path.GetFileNameWithoutExtension(textDialogSave.FileName), Path.GetDirectoryName(textDialogSave.FileName));
                if (bRetAfterSave == true)
                {
                    this.Title = Lang.sFlipBlock3D_LevelEditor + " [" + projectName + "]";
                    sbiProjectPath.Text = projectPath;
                    Xceed.Wpf.Toolkit.MessageBox.Show(Lang.sInfoSaveLevelMessage, Lang.sInfoSaveLevelTittle, MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
                else
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show(Lang.sErrorSaveLevelMessage, Lang.sErrorSaveLevelTittle, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private bool SaveProject(List<Cell> saveObject, string pName, string pPath)
        {
            bool correctSave = false;
            if (pName == "")
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(Lang.sErrorIncorrectNameMessage, Lang.sErrorIncorrectNameTittle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (pPath == "")
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(Lang.sErrorIncorrectPathMessage, Lang.sErrorIncorrectPathTittle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (!Directory.Exists(pPath))
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(Lang.sErrorPathNoExistMessage, Lang.sErrorPathNoExistTittle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                try
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    XmlSerializer serializer = new XmlSerializer(lCells.GetType());
                    string fileLoc = Path.Combine(pPath, pName + ".xml");
                    using (MemoryStream stream = new MemoryStream())
                    {
                        serializer.Serialize(stream, lCells);
                        stream.Position = 0;
                        xmlDocument.Load(stream);
                        xmlDocument.Save(fileLoc);
                        stream.Close();
                    }
                    projectName = pName;
                    projectPath = pPath;
                    correctSave = true;
                }
                catch
                {
                    return correctSave;
                }   
            }
            return correctSave;
        }
        private void bShowAllBlock_Click(object sender, RoutedEventArgs e)
        {
            ShowAllCels();
        }
        private void bDeleteAllCels_Click(object sender, RoutedEventArgs e)
        {
            DeleteAllCelss();
        }
        private void miResizeViewToAll_Click(object sender, RoutedEventArgs e)
        {
            ShowAllCels();
        }
        private void miDeleteAllCells_Click(object sender, RoutedEventArgs e)
        {
            DeleteAllCelss();
        }
        private void DeleteAllCelss()
        {
            lCells.Clear();
            Cell tempCell = new Cell();
            tempCell.PosX = 0; tempCell.PosY = 0; tempCell.Number = 1; tempCell.Type = CellType.Normal;
            lCells.Add(tempCell);
            lvUsers.ItemsSource = null;
            lvUsers.ItemsSource = lCells;
            tbElementCount.Text = lCells.Count.ToString();
            CalcLevelSize();
            UpdateMainGridView();
        }
        private void ShowAllCels()
        {
            bAfterInitial = false;
            iudAreaViewDimMinX.Value = lCells.Min(c => c.PosX);
            iudAreaViewDimMaxX.Value = lCells.Max(c => c.PosX);
            iudAreaViewDimMinY.Value = lCells.Min(c => c.PosY);
            iudAreaViewDimMaxY.Value = lCells.Max(c => c.PosY);
            bAfterInitial = true;
            UpdateMainGridView();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(Settings.Default.showLeftPanel)
            {
                Settings.Default.leftPanelSize = gMainGrid.ColumnDefinitions[0].ActualWidth;
            }
            else
            {
                Settings.Default.leftPanelSize = prevAcutalWidthLeft;
            }
            if (Settings.Default.showRightPanel)
            {
                Settings.Default.rightPanelSize = gMainGrid.ColumnDefinitions[4].ActualWidth;
            }
            else
            {
                Settings.Default.rightPanelSize = prevAcutalWidthRight;
            }
            /*
            XmlSerializer xs = new XmlSerializer(globalProgConf.GetType());
            TextWriter tw = new StreamWriter(pathConfigFile);
            xs.Serialize(tw, globalProgConf);
            tw.Close();
            */
            Settings.Default.Save();
        }
        private void miSetDefoultView_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.leftPanelSize = 240;
            Settings.Default.rightPanelSize = 280;
            Settings.Default.showBottomStatusBar = true;
            Settings.Default.showLeftPanel = true;
            Settings.Default.showRightPanel = true;
            Settings.Default.showToolBar = true;
            Settings.Default.iconsSize = 24;
            SetWindowsVisableElement();
        }
        private void miLangPol_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.selectedLanguage = new CultureInfo("pl-PL");
            Settings.Default.Save();
            Xceed.Wpf.Toolkit.MessageBox.Show("Wybrałeś język Polski.\nProszę zamknać i ponownie uruchomić program aby dokończyć zmiane języka.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            SetLangIndicator();
        }
        private void miLangEng_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.selectedLanguage = new CultureInfo("en-US");
            Settings.Default.Save();
            Xceed.Wpf.Toolkit.MessageBox.Show("You select English language.\nPlease close and once again open program to valid language switch.", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            SetLangIndicator();
        }
        private void vPlaceAllCells_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            int iChangeValue = 0;
            if (e.Delta > 0)
            {
                iChangeValue = -1;
                
            }
            else if (e.Delta < 0)
            {
                iChangeValue = 1;
            }
            bAfterInitial = false;
            if(iudAreaViewDimMinX.Value - iChangeValue < iudAreaViewDimMaxX.Value)
            {
                iudAreaViewDimMinX.Value -= iChangeValue;
                iudAreaViewDimMaxX.Value += iChangeValue;
            }
            if (iudAreaViewDimMinY.Value - iChangeValue < iudAreaViewDimMaxY.Value)
            {
                iudAreaViewDimMinY.Value -= iChangeValue;
                iudAreaViewDimMaxY.Value += iChangeValue;
            }                
            bAfterInitial = true;
            UpdateMainGridView();
        }
        private void bMoveViewLeftUp_Click(object sender, RoutedEventArgs e)
        {
            int iChangeValue = -1;
            bAfterInitial = false;
            iudAreaViewDimMinX.Value -= iChangeValue;
            iudAreaViewDimMaxX.Value -= iChangeValue;
            iudAreaViewDimMinY.Value += iChangeValue;
            iudAreaViewDimMaxY.Value += iChangeValue;
            bAfterInitial = true;
            UpdateMainGridView();
        }
        private void bMoveViewUp_Click(object sender, RoutedEventArgs e)
        {
            int iChangeValue = -1;
            bAfterInitial = false;
            iudAreaViewDimMinY.Value += iChangeValue;
            iudAreaViewDimMaxY.Value += iChangeValue;
            bAfterInitial = true;
            UpdateMainGridView();
        }
        private void bMoveViewRightUp_Click(object sender, RoutedEventArgs e)
        {
            int iChangeValue = -1;
            bAfterInitial = false;
            iudAreaViewDimMinX.Value += iChangeValue;
            iudAreaViewDimMaxX.Value += iChangeValue;
            iudAreaViewDimMinY.Value += iChangeValue;
            iudAreaViewDimMaxY.Value += iChangeValue;
            bAfterInitial = true;
            UpdateMainGridView();
        }
        private void bMoveViewLeft_Click(object sender, RoutedEventArgs e)
        {
            int iChangeValue = -1;
            bAfterInitial = false;
            iudAreaViewDimMinX.Value -= iChangeValue;
            iudAreaViewDimMaxX.Value -= iChangeValue;
            bAfterInitial = true;
            UpdateMainGridView();
        }
        private void bMoveViewRight_Click(object sender, RoutedEventArgs e)
        {
            int iChangeValue = -1;
            bAfterInitial = false;
            iudAreaViewDimMinX.Value += iChangeValue;
            iudAreaViewDimMaxX.Value += iChangeValue;
            bAfterInitial = true;
            UpdateMainGridView();
        }
        private void bMoveViewLeftDown_Click(object sender, RoutedEventArgs e)
        {
            int iChangeValue = -1;
            bAfterInitial = false;
            iudAreaViewDimMinX.Value -= iChangeValue;
            iudAreaViewDimMaxX.Value -= iChangeValue;
            iudAreaViewDimMinY.Value -= iChangeValue;
            iudAreaViewDimMaxY.Value -= iChangeValue;
            bAfterInitial = true;
            UpdateMainGridView();
        }
        private void bMoveViewDown_Click(object sender, RoutedEventArgs e)
        {
            int iChangeValue = -1;
            bAfterInitial = false;
            iudAreaViewDimMinY.Value -= iChangeValue;
            iudAreaViewDimMaxY.Value -= iChangeValue;
            bAfterInitial = true;
            UpdateMainGridView();
        }
        private void bMoveViewRightDown_Click(object sender, RoutedEventArgs e)
        {
            int iChangeValue = -1;
            bAfterInitial = false;
            iudAreaViewDimMinX.Value += iChangeValue;
            iudAreaViewDimMaxX.Value += iChangeValue;
            iudAreaViewDimMinY.Value -= iChangeValue;
            iudAreaViewDimMaxY.Value -= iChangeValue;
            bAfterInitial = true;
            UpdateMainGridView();
        }
        private void bZoomPlus_Click(object sender, RoutedEventArgs e)
        {
            int iChangeValue = 1;
            bAfterInitial = false;
            if (iudAreaViewDimMinX.Value + iChangeValue < iudAreaViewDimMaxX.Value)
            {
                iudAreaViewDimMinX.Value += iChangeValue;
                iudAreaViewDimMaxX.Value -= iChangeValue;
            }
            if (iudAreaViewDimMinY.Value + iChangeValue < iudAreaViewDimMaxY.Value)
            {
                iudAreaViewDimMinY.Value += iChangeValue;
                iudAreaViewDimMaxY.Value -= iChangeValue;
            }
            bAfterInitial = true;
            UpdateMainGridView();
        }
        private void bZoomMinus_Click(object sender, RoutedEventArgs e)
        {
            int iChangeValue = 1;
            bAfterInitial = false;
            iudAreaViewDimMinX.Value -= iChangeValue;
            iudAreaViewDimMaxX.Value += iChangeValue;
            iudAreaViewDimMinY.Value -= iChangeValue;
            iudAreaViewDimMaxY.Value += iChangeValue;
            bAfterInitial = true;
            UpdateMainGridView();
        }
    }
}