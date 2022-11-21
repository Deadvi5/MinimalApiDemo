using Microsoft.EntityFrameworkCore;
using TodoApi.Validation;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace TodoApi;

internal static class TodoApi
{
    public static RouteGroupBuilder MapTodos(this RouteGroupBuilder group)
    {
        group.WithTags("Todos");

        // Add security requirements, all incoming requests to this API *must*
        // be authenticated.
        group.RequireAuthorization()
             .AddOpenApiSecurityRequirement();

        group.MapGet("/", async (TodoDbContext db, UserId owner) =>
        {
            return await db.Todos.Where(todo => todo.OwnerId == owner.Id).ToListAsync();
        });

        group.MapGet("/{id}", async (TodoDbContext db, int id, UserId owner) =>
        {
            return await db.Todos.FindAsync(id) switch
            {
                Todo todo when todo.OwnerId == owner.Id || owner.IsAdmin => Results.Ok(todo),
                _ => Results.NotFound()
            };
        })
        .Produces<Todo>()
        .Produces(Status404NotFound);

        group.MapPost("/", async (TodoDbContext db, [Validate]NewTodo newTodo, UserId owner) =>
        {
            Todo todo = new()
            {
                Title = newTodo.Title!,
                OwnerId = owner.Id
            };

            db.Todos.Add(todo);
            await db.SaveChangesAsync();

            return Results.Created($"/todos/{todo.Id}", todo);
        })
       .Produces(Status201Created)
       .ProducesValidationProblem();

        group.MapPut("/{id}", async (TodoDbContext db, int id, Todo todo, UserId owner) =>
        {
            if (id != todo.Id)
            {
                return Results.BadRequest();
            }

            int rowsAffected = await db.Todos.Where(t => t.Id == id && (t.OwnerId == owner.Id || owner.IsAdmin))
                                             .ExecuteUpdateAsync(updates =>
                                                updates.SetProperty(t => t.IsComplete, todo.IsComplete)
                                                       .SetProperty(t => t.Title, todo.Title));

            return rowsAffected == 0 ? Results.NotFound() : Results.Ok();
        })
        .Produces(Status400BadRequest)
        .Produces(Status404NotFound)
        .Produces(Status200OK);

        group.MapDelete("/{id}", async (TodoDbContext db, int id, UserId owner) =>
        {
            int rowsAffected = await db.Todos.Where(t => t.Id == id && (t.OwnerId == owner.Id || owner.IsAdmin))
                                             .ExecuteDeleteAsync();

            return rowsAffected == 0 ? Results.NotFound() : Results.Ok();
        })
        .Produces(Status400BadRequest)
        .Produces(Status404NotFound)
        .Produces(Status200OK);

        return group;
    }
}
