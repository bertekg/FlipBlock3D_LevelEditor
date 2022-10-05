using FlipBlock3D_LevelEditor.Languages;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace FlipBlock3D_LevelEditor
{
    /// <summary>
    /// Interaction logic for AboutProgram.xaml
    /// </summary>
    public partial class AboutProgram : Window
    {
        public AboutProgram()
        {
            InitializeComponent();
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(assembly.Location);
            DateTime lastModified = fileInfo.LastWriteTime;
            tbVersionNo.Text = Lang.sVersionNumber + ": " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            tbReleaseDate.Text = Lang.sReleaseDate + ": " + lastModified.Date.ToString("yyyy/MM/dd");
            tbCopyright.Text = fvi.LegalCopyright;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Escape || e.Key == Key.Enter || e.Key == Key.Back)
            {
                this.Close();
            }
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
