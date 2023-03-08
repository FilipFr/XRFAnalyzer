using System.Windows;
using System.Windows.Controls;


namespace XRFAnalyzer.Views.UserControls
{
    /// <summary>
    /// Interaction logic for MenuButtonContent.xaml
    /// </summary>
    public partial class MenuButtonContent : UserControl
    {
        public MenuButtonContent()
        {
            InitializeComponent();
        }

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

        public static readonly DependencyProperty LabelStyleProperty = DependencyProperty.Register(
            "LabelStyle",
            typeof(Style),
            typeof(MenuButtonContent),
            new PropertyMetadata(null));

        public string LabelStyle
        {
            get { return (string)GetValue(LabelStyleProperty); }
            set { SetValue(LabelStyleProperty, value); }
        }

        public static readonly DependencyProperty PackIconStyleProperty = DependencyProperty.Register(
            "PackIconStyle",
            typeof(Style),
            typeof(MenuButtonContent),
            new PropertyMetadata(null));

        public string PackIconStyle
        {
            get { return (string)GetValue(PackIconStyleProperty); }
            set { SetValue(PackIconStyleProperty, value); }
        }

    }
}