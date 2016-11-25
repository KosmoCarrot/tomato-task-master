using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace TomatoTaskMaster.Model
{
    class DataRepository
    { 
        public DataRepository()
        {
            TaskCalendar = LoadTasks() ?? new Dictionary<string, ObservableCollection<Task>>();
            Tasks = new ObservableCollection<Task>();
        }

        public Dictionary<string, ObservableCollection<Task>> TaskCalendar { get; }
        public ObservableCollection<Task> Tasks { get; private set; }

        public void SetTasksForDate(string date)
        {
            if (!TaskCalendar.ContainsKey(date))
                TaskCalendar.Add(date, new ObservableCollection<Task>());

            Tasks = TaskCalendar[date];
        }

        public void SaveTasks()
        {
            File.WriteAllText("calendar.json", JsonConvert.SerializeObject(TaskCalendar, Formatting.Indented));
        }

        private Dictionary<string, ObservableCollection<Task>> LoadTasks()
        {
            string content;

            using (var fileReader = File.Open("calendar.json", FileMode.OpenOrCreate))
            {
                using (var streamReader = new StreamReader(fileReader))
                {
                    content = streamReader.ReadToEnd();
                }
            }
                
            return JsonConvert
                .DeserializeObject<Dictionary<string, ObservableCollection<Task>>>(
                 content
                );
        }   
    }
    
}
