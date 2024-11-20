using backend;
using backend.models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


static bool IsAdmin(Testappcontext context, string token)
{
    foreach (User u in context.Users)
    {
        if (u.Token == token && u.IsAdmin == "true")
        {
            return u.IsAdmin == "true";
        }
    }
    return false;
}


app.UseHttpsRedirection();
Testappcontext context = new Testappcontext();
app.MapGet("/users/login", (string email, string password) =>
{
    foreach (User u in context.Users)
    {
        if (u.email == email && u.password == password)
        {
            return Results.Ok(u.Token);
        }
    }
    return Results.BadRequest();
});

app.MapGet("/users", (string token) =>
{
    if (!IsAdmin(context, token))
    {
        return Results.BadRequest();
    }
    return Results.Ok(context.Users.ToArray());
});

app.MapGet("/users/{id}", (int id, string token) =>
{
    if (IsAdmin(context, token))
    {
        return Results.Ok(context.Users.Find(id));
    }
    return Results.BadRequest();

});
app.MapPost("/users", (User u) =>
{
    context.Users.Add(u);
    context.SaveChanges();
});
app.MapDelete("/users/{id}", (int id) =>
{
    User? usertodelete = context.Users.Find(id);
    if (usertodelete != null)
        context.Users.Remove(usertodelete);
    context.SaveChanges();
});

app.Run();

