using Avalonia.Controls;
using LabWork7.DialogManagers.Base;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LabWork7.DialogManagers
{
    public class DialogManager : IDialogManager
    {
        private IServiceProvider m_serviceProvider;

        public DialogManager(IServiceProvider serviceProvider)
        {
            m_serviceProvider = serviceProvider ?? 
                throw new ArgumentNullException(nameof(serviceProvider));
        }

        public TDialog Create<TDialog>() where TDialog : Window
        {
            return (TDialog)m_serviceProvider.GetRequiredService<TDialog>();
        }
    }
}
