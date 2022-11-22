namespace TodoApi.Services
{
    public class PostTodoService : IPostTodoService
    {
        private readonly TodoDbContext db;

        public PostTodoService(TodoDbContext db)
        {
            this.db = db;
        }

        public async Task<Todo> PostTodoAsync(NewTodo newTodo, UserId user)
        {
            Todo todo = new()
            {
                Title = newTodo.Title!,
                OwnerId = user.Id
            };
           
            db.Todos.Add(todo);
            await db.SaveChangesAsync();
            return todo;
        }
    }
}
