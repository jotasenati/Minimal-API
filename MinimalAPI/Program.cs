using MiniTodo.ViewModels;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options => options.AddPolicy(name: "TodoOrigins", policy =>{
    policy.AllowAnyHeader().AllowAnyOrigin(); 
}));

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("TodoOrigins");


app.MapGet("/v1/todos", (AppDbContext context) =>
{
    var todos = context.Todos;
    return todos is not null ? Results.Ok(todos) : Results.NotFound();
}).Produces<Todo>();

app.MapGet("v1/todosById", (AppDbContext context, Guid id) => 
{
    try
    {
        var todo = context.Todos.Find(id);

        if (todo == null) return Results.NotFound("Id não encontrado, favor verificar!");

        return Results.Ok(todo);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPost("/v1/todos", (AppDbContext context, CreateTodoViewModel model) =>
{
    try
    {
        var todo = model.MapTo();

        if (!model.IsValid)
            return Results.BadRequest(model.Notifications);

        context.Todos.Add(todo);
        context.SaveChanges();

        return Results.Created($"/v1/todos/{todo.Id}", todo);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, ex.InnerException.ToString());
    }
});

app.MapPut("v1/todos", (AppDbContext context, UpdateTodoViewModel model) =>
{
    try
    {
        var todo =  context.Todos.Find(model.Id);

        if (todo == null) return Results.NotFound();

        if (model.IsValid)
        {
            todo.Title = model.Title ?? todo.Title;
            todo.Done = model.Done;
        };

        context.SaveChanges();

        return Results.Created($"/v1/todos/{todo.Id}", todo);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, ex.InnerException.ToString());
    }
});

app.MapDelete("v1/todos", (Guid Id, AppDbContext context) =>
{
    try
    {
        var todo =  context.Todos.Find(Id);

        if (todo == null) return Results.NotFound();

        context.Todos.Remove(todo);

        context.SaveChanges();

        return Results.Ok("Item deletado com sucesso");
    }
    catch (Exception ex)
    {
        return Results.BadRequest();
    }
});

app.Run();
