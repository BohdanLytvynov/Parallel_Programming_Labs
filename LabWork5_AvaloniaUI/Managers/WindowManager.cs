using Avalonia.Controls;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LabWork5_AvaloniaUI.Managers
{
    public class WindowManager : IWindowManager
    {
        IServiceProvider m_serviceProvider;

        public WindowManager(IServiceProvider serviceProvider)
        {
            m_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }
        /// <summary>
        /// Builds Window
        /// </summary>
        /// <typeparam name="TWindow"></typeparam>
        /// <returns></returns>
        public Window? Build<TWindow>()
            where TWindow : Window
        {
            Window w = null;
            try
            {
                if (!Dispatcher.UIThread.CheckAccess())//Case if the window was build not in UI Thread
                {
                    w = Dispatcher.UIThread.Invoke(() => m_serviceProvider.GetRequiredService<TWindow>());
                }
                else 
                {
                    //Window was build in UI Thread
                    w = m_serviceProvider.GetRequiredService<TWindow>();
                }
            }
            catch (Exception ex)
            {
                throw;//rethrow exception If it is called not from UI thread it will be ignored
            }
            
            return w;
        }
    }
}
