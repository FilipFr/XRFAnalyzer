using System.Windows;
using System.Windows.Controls;


namespace XRFAnalyzer.Views.UserControls
{
    /// <summary>
    /// Interaction logic for MenuButtonContent.xaml
    /// </summary>
    public partial class MenuButtonContent : UserControl
    {
        public static readonly DependencyProperty PackIconKindProperty = DependencyProperty.Register(
            "PackIconKind",
            typeof(string),
            typeof(MenuButtonContent),
            new PropertyMetadata(null));
        
        public string PackIconKind
        {
            get { return (string)GetValue(PackIconKindProperty); }
            set { SetValue(PackIconKindProperty, value); }
        }

        public static readonly DependencyProperty LabelContentProperty = DependencyProperty.Register(
            "LabelContent",
            typeof(string),
            typeof(MenuButtonContent),
            new PropertyMetadata(null));

        public string LabelContent
        {
            get { return (string)GetValue(LabelContentProperty); }
            set { SetValue(LabelContentProperty, value); }
        }

        public static readonly DependencyProperty LabelFontSizeProperty = DependencyProperty.Register(
            "LabelFontSize",
            typeof(string),
            typeof(MenuButtonContent),
            new PropertyMetadata(null));

        public string LabelFontSize
        {
            get { return (string)GetValue(LabelFontSizeProperty); }
            set { SetValue(LabelFontSizeProperty, value); }
        }

        public MenuButtonContent()
        {
            InitializeComponent();
        }

    }
}
