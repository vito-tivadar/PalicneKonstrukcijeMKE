using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace PalicneKonstrukcijeMKE.CustomControls;

public class ProgramMessageBlock : TextBlock
{
    #region DependencyProperties

    public static readonly DependencyProperty ClearMessageTimeProperty = DependencyProperty.Register("ClearMessageTime", typeof(int), typeof(ProgramMessageBlock), new PropertyMetadata(5));
    public int ClearMessageTime
    {
        get { return (int)GetValue(ClearMessageTimeProperty); }
        set { SetValue(ClearMessageTimeProperty, value); }
    }

    public static readonly DependencyProperty TimerEnabledProperty = DependencyProperty.Register("TimerEnabled", typeof(bool), typeof(ProgramMessageBlock), new PropertyMetadata(true));
    public bool TimerEnabled
    {
        get { return (bool)GetValue(TimerEnabledProperty); }
        set { SetValue(TimerEnabledProperty, value); }
    }

    #endregion // DependencyProperties

    public DispatcherTimer Timer { get; }
    private int _currentTick;

    public ProgramMessageBlock()
    {
        Timer = new DispatcherTimer();
        Timer.Interval = new TimeSpan(0, 0, 1);
        Timer.Tick += ClearMessage;
        _currentTick = 0;
    }



    private void ClearMessage(object sender, EventArgs e)
    {
        if (_currentTick >= ClearMessageTime)
        {
            Timer.Stop();
            ClearText();
        }
        else _currentTick++;
    }

    private void CheckTimer()
    {
        if (TimerEnabled)
        {
            Timer.Stop();
            _currentTick = 0;
            Timer.Start();
        }
    }



    public void SetText(string message, string tooltip = "")
    {
        Text = message;
        ToolTip = tooltip;
        Foreground = new SolidColorBrush(Colors.Black);
        CheckTimer();
    }

    public void SetText(string message, Color color, string tooltip = "")
    {
        Text = message;
        ToolTip = tooltip;
        Foreground = new SolidColorBrush(color);
        CheckTimer();
    }

    public void SetError(string message, string tooltip = "")
    {
        Text = $"Napaka: {message}";
        ToolTip = tooltip;
        Foreground = new SolidColorBrush(Colors.Red);
        CheckTimer();
    }

    public void ClearText()
    {
        Text = string.Empty;
        ToolTip = string.Empty;
    }
}
