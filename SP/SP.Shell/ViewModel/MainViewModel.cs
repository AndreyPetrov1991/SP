using System;
using System.Collections.ObjectModel;
using System.Linq;

using Dragablz;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

using Microsoft.Practices.ServiceLocation;

using SP.Extensions;
using SP.Resources;
using SP.Shell.Behaviors;
using SP.Shell.Messages;
using SP.Shell.Services;

namespace SP.Shell.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private TabViewModel selectedTab;

        public MainViewModel()
        {
            MessengerInstance = ServiceLocator.Current.GetInstance<Messenger>();
            Tabs = new ObservableCollection<TabViewModel>
                       {
                           new TabViewModel(Strings.NewTab)
                       };
            MessengerInstance.Register<AnalyzeDataMessage>(this, AnalyzeDataExecute);
        }

        public ObservableCollection<TabViewModel> Tabs { get; private set; }

        public TabViewModel SelectedTab
        {
            get
            {
                return selectedTab;
            }

            set
            {
                selectedTab = value;
                RaisePropertyChanged(() => SelectedTab);
            }
        }

        public Func<TabViewModel> AddNewTabCommand
        {
            get
            {
                return () => new TabViewModel(TabNameService.GetImportedData(Tabs.Select(t => t.Title)));
            }
        }

        public AnalysisService Service
        {
            get { return ServiceLocator.Current.GetInstance<AnalysisService>(); }
        }

        public TabNameService TabNameService
        {
            get { return ServiceLocator.Current.GetInstance<TabNameService>(); }
        }

        public IInterTabClient Client
        {
            get { return new NoNewWindowInterTabClient(); }
        }

        private void AnalyzeDataExecute(AnalyzeDataMessage message)
        {
            try
            {
                var result = Service.Analyze(message.InputData, message.Type);
                var newTab = new TabViewModel(TabNameService.GetResults(), result.Rows.ToCompleteList());

                Tabs.Add(newTab);
                SelectedTab = newTab;
            }
            catch (Exception e)
            {
                MessengerInstance.Send(new ShowPopupMessage(Strings.ErrorOccured, e.Message));
            }
        }
    }
}