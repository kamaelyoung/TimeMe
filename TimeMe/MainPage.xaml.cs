using System;
using System.Windows;
using TimeMe.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace TimeMe
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = App.ApplicationModel;

            App.ApplicationModel.TimerServiceTimeChanged -= TimerChanged;
            App.ApplicationModel.TimerServiceTimeChanged += TimerChanged;

            SetStartStopResetButtonText();
        }

        private void SetStartStopResetButtonText()
        {
            switch (App.ApplicationModel.ApplicationState)
            {
                case ApplicationStateType.Ready:
                    startStopResetButton.Content = "Start";
                    break;
                case ApplicationStateType.Running:
                    startStopResetButton.Content = "Stop";
                    break;
                case ApplicationStateType.Completed:
                    startStopResetButton.Content = "Reset";
                    break;
            }
        }

        void startStopResetButton_Click(object sender, EventArgs e)
        {
            switch (App.ApplicationModel.ApplicationState)
            {
                case ApplicationStateType.Ready:
                    App.ApplicationModel.StartRun();
                    break;
                case ApplicationStateType.Running:
                    var newLap = App.ApplicationModel.StopRun();
                    lapsListBox.UpdateLayout();
                    lapsListBox.ScrollIntoView(newLap);
                    break;
                case ApplicationStateType.Completed:
                    App.ApplicationModel.ResetRun();
                    break;
            }
            SetStartStopResetButtonText();
        }

        void recordLapButton_Click(object sender, EventArgs e)
        {
            if (App.ApplicationModel.ApplicationState == ApplicationStateType.Running)
            {
                var newLap = App.ApplicationModel.RecordLapTime();
                lapsListBox.UpdateLayout();
                lapsListBox.ScrollIntoView(newLap);
            }
        }

        private void TimerChanged(TimeSpan elapsedTime)
        {
            Dispatcher.BeginInvoke(() =>
                {
                    App.ApplicationModel.IncrementDuration(elapsedTime);
                });
        }

        private void EatMyDustLink_Click(object sender, RoutedEventArgs e)
        {
            var detailTask = new MarketplaceDetailTask();
            detailTask.ContentIdentifier = "5640834d-cff9-df11-9264-00237de2db9e";
            detailTask.ContentType = MarketplaceContentType.Applications;
            detailTask.Show();
        }
    }
}