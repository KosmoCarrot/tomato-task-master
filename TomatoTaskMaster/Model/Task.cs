
namespace TomatoTaskMaster.Model
{
    public class Task
    {
        public Task()
        {
        }
 
        public Task(string name, string description) 
        {
            Name = name;
            Description = description;
        }

        public string Name { get; set; }
        public string Description { get; set; }
    }
}
