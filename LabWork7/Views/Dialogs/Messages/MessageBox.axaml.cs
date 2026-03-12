using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;

namespace LabWork7;

public partial class MessageBox : Window
{
    public event Action OkPressed;
    public event Action CancelPressed;

    private TextBlock m_MsgTextBlock;
    private TextBlock m_HeaderTextBlock;

    public MessageBox()
    {
        InitializeComponent();

        m_MsgTextBlock = this.FindControl<TextBlock>("Msg") ?? 
            throw new NullReferenceException("Unable to find textblock for message display!");

        m_HeaderTextBlock = this.FindControl<TextBlock>("MsgHeader") ?? 
            throw new NullReferenceException("Unable to find textblock for dialog header display!");
    }

    public void SetMsg(string msg)
    { 
        m_MsgTextBlock.Text = msg;
    }

    public void SetMsgBoxHeader(string header, Brush brush)
    { 
        m_HeaderTextBlock.Text = header;
        m_HeaderTextBlock.Foreground = brush;
    }

    public void OnOkPressed(object o, RoutedEventArgs e)
    {
        OkPressed?.Invoke();
    }

    public void OnCancelPressed(object o, RoutedEventArgs e)
    { 
        CancelPressed?.Invoke();
    }
}