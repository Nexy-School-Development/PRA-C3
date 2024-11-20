using backend;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
Testappcontext context = new Testappcontext();

app.MapGet("/users", () =>
{
    return context.Users.ToArray();
});

app.MapGet("/users/{id}", (int id) =>
{
    return context.Users.Find(id);
});

app.Run();