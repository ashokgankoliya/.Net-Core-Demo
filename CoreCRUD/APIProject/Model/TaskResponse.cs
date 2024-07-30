namespace APIProject.Model
{
    public class TaskResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime Ts { get; set; }
    }
}
