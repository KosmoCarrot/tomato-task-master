using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using TomatoTaskMaster.Model;

namespace TomatoTaskMaster.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private const int numOfPeriods = 6;
        private readonly DataRepository _repo;
        private ObservableCollection<Task> _tasks;
        private ObservableCollection<Task> _completedTasks;
        private DateTime _date;
        private Task _selectedTask;
        private bool _isOpenFlyOut;
        private bool _isCompletedTasksListOpen;
        private string _workingTime;
        private DispatcherTimer _timer;
        private TimeSpan _time;
        private string _workingStage;
        private int _seconds;
        private MediaPlayer _player;

        public MainViewModel()
        {
            _repo = new DataRepository();

            AddTaskCommand = new RelayCommand(AddTask);
            DeleteTaskCommand = new RelayCommand(DeleteTask);
            ShowTaskInfoCommand = new RelayCommand(ShowTaskInfo);
            OpenTomatoCommand = new RelayCommand(OpenTomato);
            TomatoWindowLoadedCommand = new RelayCommand(TomatoWindowLoaded);
            TaskCompletedCommand = new RelayCommand(TaskCompleted);
            MainWindowClosingCommand = new RelayCommand(MainWindowClosing);

            Date = DateTime.Now;
            _player = new MediaPlayer();
        }

        public RelayCommand AddTaskCommand { get; set; }
        public RelayCommand DeleteTaskCommand { get; set; }
        public RelayCommand ShowTaskInfoCommand { get; set; }
        public RelayCommand OpenTomatoCommand { get; set; }
        public RelayCommand TomatoWindowLoadedCommand { get; set; }
        public RelayCommand TaskCompletedCommand { get; set; }
        public RelayCommand MainWindowClosingCommand { get; set; }

        public ObservableCollection<Task> Tasks
        {
            get { return _tasks = _repo.Tasks; }
            set { _tasks = value; }
        }

        public ObservableCollection<Task> CompletedTasks
        {
            get { return _completedTasks ?? (_completedTasks = new ObservableCollection<Task>()); }
            set { _completedTasks = value; }
        }

        public DateTime Date
        {
            get { return _date; }
            set
            {
                if (_date != value)
                {
                    _date = value;
                    LoadTasks();
                    IsCompletedTasksListOpen = false;
                }
                RaisePropertyChanged("Date");
            }
        }

        public Task SelectedTask
        {
            get { return _selectedTask; }
            set
            {
                if (_selectedTask != value)
                    _selectedTask = value;
                RaisePropertyChanged("SelectedTask");
            }
        }

        public string WorkingTime
        {
            get { return _workingTime; }
            set
            {
                if (_workingTime != value)
                    _workingTime = value;
                RaisePropertyChanged("WorkingTime");
            }
        }

        public string WorkingStage
        {
            get { return _workingStage; }
            set
            {
                if (_workingStage != value)
                {
                    _workingStage = value;
                    SetTimerParams();
                }
                RaisePropertyChanged("WorkingStage");
            }
        }

        public bool IsOpenFlyOut
        {
            get { return _isOpenFlyOut; }
            set
            {
                _isOpenFlyOut = value;
                RaisePropertyChanged("IsOpenFlyOut");
            }
        }

        public bool IsCompletedTasksListOpen
        {
            get { return _isCompletedTasksListOpen; }
            set
            {
                _isCompletedTasksListOpen = value;
                RaisePropertyChanged("IsCompletedTasksListOpen");
            }
        }

        private void AddTask()
        {
            Tasks.Add(new Task());
        }

        private void DeleteTask()
        {
            Tasks.Remove(SelectedTask);
        }

        private void ShowTaskInfo()
        {
            IsOpenFlyOut = true;
        }

        private void OpenTomato()
        {
            Messenger.Default.Send(new NotificationMessage<ObservableCollection<Task>>(Tasks, "OpenTomatoWindow"));
            WorkingStage = "Working!";
            SetTimer();
            _timer.Start();
        }

        private void TomatoWindowLoaded()
        {
            Messenger.Default.Send(new NotificationMessage("SetTomatoWindowPosition"));
        }

        private void TaskCompleted()
        {
            CompletedTasks.Add(SelectedTask);
            DeleteTask();
            IsCompletedTasksListOpen = true;
            Messenger.Default.Send(new NotificationMessage("OpenMainWindow"));
            _timer.Stop();
            WorkingTime = null;
            WorkingStage = null;
        }

        private void MainWindowClosing()
        {
            _repo.SaveTasks();
        }

        private void LoadTasks()
        {
            _repo.SetTasksForDate(Date.ToShortDateString());
            Messenger.Default.Send(new NotificationMessage<ObservableCollection<Task>>(Tasks, "ShowTasksForDate"));
        }

        private void SetTimerParams()
        {
            switch (WorkingStage)
            {
               case "Working!":
                    _seconds = 1500;
                    break;
               case "Long break":
                    _seconds = 900;
                    break;
               case "Short break":
                    _seconds = 300;
                    break;
            }

            if (WorkingStage != null)
            {
                _player.Open(new Uri(string.Format(@"{0}\crank.wav", Environment.CurrentDirectory)));
                _player.Play();
            }

            _time = TimeSpan.FromSeconds(_seconds);
        }

        private void SetTimer()
        {
            int stageInd = 0;

            _timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                WorkingTime = _time.ToString("mm\\:ss");

                if (_time == TimeSpan.Zero)
                {
                    if (stageInd < numOfPeriods)
                    {
                        WorkingStage = WorkingStage == "Working!" ? "Short break" : "Working!";
                        stageInd++;
                    }
                    else
                    {
                        WorkingStage = "Long break";
                        stageInd = 0;
                    }
                }
                _time = _time.Add(TimeSpan.FromSeconds(-1));
            }, Application.Current.Dispatcher);

        }
    }
}