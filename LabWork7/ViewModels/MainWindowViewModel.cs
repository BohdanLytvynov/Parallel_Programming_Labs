using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabWork7.BusinessLayer.PICalculators;
using LabWork7.DialogManagers.Base;
using LabWork7.Models.Languages;
using LabWork7.Settings;
using LabWork7.Strings;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace LabWork7.ViewModels
{
    public partial class MainWindowViewModel : ObservableValidator
    {
        #region Fields
        private const string EMPTY_FIELD_ERROR_MSG_KEY = "Empty_field";
        private const string INCORRECT_RANGE_KEY = "Incorrect_range";
        private const string NAN_ERROR_MSG_KEY = "NAN_Msg";
        private const string CheckNumberRegExp = @"^\d+$";

        private long m_NumOfSteps;
        private int m_NumOfThreads;

        private IAsyncPICalculator m_asyncPICalculator;
        private IProgress<double> m_CalculationProgress;
        private IDialogManager m_dialogManager;
        private AppSettings m_appSettings;
        #endregion

        #region Bindable Properties
        public string Title { get; }
        public bool ShowGridLines { get; set; }
        public ObservableCollection<LanguageVM> LanguageList { get; }

        [ObservableProperty]
        private double timeElapsed;

        [ObservableProperty]
        private int selectedLangIndex;

        [ObservableProperty]
        private bool blockScreenVisible;

        [ObservableProperty]
        private double result;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(CalculateCommand))]
        [Required(ErrorMessageResourceType = typeof(UIStrings)
            , ErrorMessageResourceName = EMPTY_FIELD_ERROR_MSG_KEY)]
        [RegularExpression(CheckNumberRegExp, ErrorMessageResourceType = typeof(UIStrings),
            ErrorMessageResourceName = NAN_ERROR_MSG_KEY)]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(UIStrings),
            ErrorMessageResourceName = INCORRECT_RANGE_KEY)]
        private string numOfSteps;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessageResourceType = typeof(UIStrings)
            , ErrorMessageResourceName = EMPTY_FIELD_ERROR_MSG_KEY)]
        [RegularExpression(CheckNumberRegExp, ErrorMessageResourceType = typeof(UIStrings),
            ErrorMessageResourceName = NAN_ERROR_MSG_KEY)]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(UIStrings),
            ErrorMessageResourceName = INCORRECT_RANGE_KEY)]
        [NotifyCanExecuteChangedFor(nameof(CalculateCommand))]
        private string numOfThreads;
        #endregion

        #region Ctor
        public MainWindowViewModel(IAsyncPICalculator asyncPICalculator,
            IDialogManager dialogManager, AppSettings appSettings) : this()
        {
            m_appSettings = appSettings ?? throw new ArgumentNullException(
                nameof(appSettings));

            LanguageList = new ObservableCollection<LanguageVM>()
            {
                new LanguageVM(){ LanguageName = UIStrings.EngLang, LanguageCode = "en-us" },
                new LanguageVM(){ LanguageName = UIStrings.UkrLang, LanguageCode = "uk-ua" }
            };

            for (int i = 0; i < LanguageList.Count; ++i)
            {
                if (m_appSettings.lang.Equals(LanguageList[i].LanguageCode))
                { 
                    selectedLangIndex = i;
                    break;
                }
            }

            m_dialogManager = dialogManager ?? throw new ArgumentNullException(
                nameof(dialogManager));
            m_asyncPICalculator = asyncPICalculator ?? throw new ArgumentNullException(
                nameof(asyncPICalculator));
        }

        public MainWindowViewModel()
        {
            ShowGridLines = false;
            blockScreenVisible = false;
            Title = AppStrings.AppTitle;
            ValidateAllProperties();
        }
        #endregion

        #region Private Methods
        partial void OnSelectedLangIndexChanged(int value)
        {
            var _ = ChangeLanguageAsync(value);
        }

        private async Task ChangeLanguageAsync(int value)
        {
            if (value < 0) return;

            var lang = LanguageList[value];
            m_appSettings.lang = lang.LanguageCode;

            string pathToConfig = Directory.GetCurrentDirectory() +
                Path.DirectorySeparatorChar + "appsettings.json";

            var root = JsonNode.Parse(File.ReadAllText(pathToConfig));

            root["global"]["lang"] = lang.LanguageCode;
            File.WriteAllText(pathToConfig, root.ToJsonString());

            MessageBox box = m_dialogManager.Create<MessageBox>();

            box.SetMsgBoxHeader(AppStrings.WarningKey, new SolidColorBrush(Colors.Orange));
            box.SetMsg(AppStrings.ReloadMsg);
            box.SetCancelButtonVisibility(false);
            box.OkPressed += () => { box.Close(); };
            box.CancelPressed += () => { box.Close(); };

            box.Closed += (object o, EventArgs e) =>
            {
                box.OkPressed -= () => { };
                box.CancelPressed -= () => { };
            };

            if (Application.Current?.ApplicationLifetime is
                IClassicDesktopStyleApplicationLifetime desktop)
            {
                box.WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterScreen;
                await box.ShowDialog(desktop.MainWindow);
            }

            Environment.Exit(0);
        }

        #endregion

        #region Commands
        private bool CanCalculate()
        {
            return !string.IsNullOrEmpty(numOfSteps)
                && !string.IsNullOrEmpty(numOfThreads)
                && long.TryParse(numOfSteps, out m_NumOfSteps)
                && int.TryParse(NumOfThreads, out m_NumOfThreads)
                && m_NumOfSteps > 0 && m_NumOfThreads > 0;
        }

        [RelayCommand(CanExecute = nameof(CanCalculate))]
        private async Task Calculate()
        {
            ProgressMessageBox dialog = m_dialogManager.Create<ProgressMessageBox>();
            dialog.CancelClicked += Dialog_CancelClicked;
            IClassicDesktopStyleApplicationLifetime Desktop = null;
            if (Application.Current?.ApplicationLifetime 
                is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Desktop = desktop;
                dialog.WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterScreen;
            }

            m_CalculationProgress = new Progress<double>(dialog.UpdateProgress);
            BlockScreenVisible = true;
            dialog.Show(Desktop.MainWindow);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var res = await m_asyncPICalculator.CalculateAsync(
                m_NumOfSteps, m_NumOfThreads,
                m_CalculationProgress);
            stopwatch.Stop();
            TimeElapsed = stopwatch.Elapsed.TotalSeconds;
            dialog.CancelClicked -= Dialog_CancelClicked;
            dialog.Close();
            BlockScreenVisible = false;
            MessageBox message = m_dialogManager.Create<MessageBox>();

            if (!res.Success)//Failed
            {
                message.SetMsgBoxHeader("Error!", new SolidColorBrush(Colors.Red));
                message.SetMsg(res.Exception.Message);
                message.ShowDialog(Desktop.MainWindow);//Block until OK or Cancel will be pressed
                message.WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterScreen;
            }
            else if (res.Cancelled)//Cancelled
            {
                message.SetMsgBoxHeader("Warning!", new SolidColorBrush(Colors.Orange));
                message.SetMsg("Operation was cancelled!");
                message.ShowDialog(Desktop.MainWindow);//Block until OK or Cancel will be pressed
                message.WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterScreen;
            }
            else//Success
            {
                Result = res.Value;
            }
        }

        private async Task Dialog_CancelClicked()
        {
            await m_asyncPICalculator.CancelCalculationAsync();
        }
        #endregion
    }
}
