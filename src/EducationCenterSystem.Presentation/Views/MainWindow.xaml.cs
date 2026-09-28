using System.Windows;
using EducationCenterSystem.Presentation.ViewModels;

namespace EducationCenterSystem.Presentation.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
