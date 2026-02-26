using CommunityToolkit.Mvvm.ComponentModel;
using LabWork7.Models.Languages;
using LabWork7.Strings;
using System.Collections.ObjectModel;

namespace LabWork7.ViewModels
{
    public partial class MainWindowViewModel : ObservableValidator
    {
        #region Fields

        #endregion

        #region Bindable Properties
        public string Title { get; }
        public bool ShowGridLines { get; set; }
        public ObservableCollection<LanguageVM> LanguageList { get; }
        #endregion

        #region Ctor
        public MainWindowViewModel()
        {
            LanguageList = new ObservableCollection<LanguageVM>()
            { 
                new LanguageVM(){ LanguageName = "English", LanguageCode = "en-us" }
            };
            ShowGridLines = true;
            Title = AppStrings.AppTitle;
        }
        #endregion

        #region Private Methods

        #endregion

        #region Commands

        #endregion
    }
}
