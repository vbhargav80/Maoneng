using System;

namespace MyApplication.Domain.Entities
{
    public class TodoItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Completed { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime DueOn { get; set; }
    }
}
