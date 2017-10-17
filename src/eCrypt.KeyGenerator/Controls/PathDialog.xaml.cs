namespace eVision.eCrypt.KeyGenerator.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using Ookii.Dialogs.Wpf;

    public partial class PathDialog : UserControl
    {
        public PathDialog()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty PathProperty = DependencyProperty.Register(nameof(Path), typeof(string), typeof(PathDialog),
            new PropertyMetadata(""));

        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        public bool IsDirectory { get; set; }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            string path = IsDirectory ? OpenFolderDialog() : OpenFileDialog();
            if (path != null)
            {
                txtBox.Text = Path = path;
            }
        }

        private string OpenFolderDialog()
        {
            var dialog = new VistaFolderBrowserDialog();
            bool? result = dialog.ShowDialog();
            return result == true ? dialog.SelectedPath : null;
        }

        private string OpenFileDialog()
        {
            var dialog = new VistaOpenFileDialog();
            bool? result = dialog.ShowDialog();
            return result == true ? dialog.FileName : null;
        }
    }
}
