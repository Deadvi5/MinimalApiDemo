namespace TodoApi.Services
{
    public interface IPostTodoService
    {
        Task<Todo> PostTodoAsync(NewTodo newTodo, UserId user);
    }
}
