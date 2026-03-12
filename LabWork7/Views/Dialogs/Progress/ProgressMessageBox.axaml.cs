using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Threading.Tasks;

namespace LabWork7;

public partial class ProgressMessageBox : Window
{
    private ProgressBar m_progressBar;

    public event Func<Task> CancelClicked;

    public ProgressMessageBox()
    {
        InitializeComponent();

        m_progressBar = this.FindControl<ProgressBar>("progress") 
            ?? throw new NullReferenceException("Unable to find ProgressBar!");
    }

    public void OnCancelClick(object o, RoutedEventArgs e)
    {
        CancelClicked?.Invoke();
    }

    public void UpdateProgress(double progress)
    { 
        m_progressBar.Value = progress;
    }
}