using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EducationCenterSystem.Presentation.Views.Components;

public partial class PrimaryButtonComponent : UserControl
{
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(PrimaryButtonComponent), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(PrimaryButtonComponent), new PropertyMetadata(null));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public PrimaryButtonComponent() => InitializeComponent();
}
