using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SistemaConsultasUVV.Data;
using SistemaConsultasUVV.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/Account/Login");

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// USUÁRIOS
app.MapGet("/api/usuarios", async (AppDbContext db) =>
    await db.Usuarios.Select(u => new { u.Id, u.Nome, u.Email, u.DataCadastro }).ToListAsync())
   .WithTags("Usuários");

app.MapPost("/api/usuarios", async (Usuario u, AppDbContext db) =>
{
    if (await db.Usuarios.AnyAsync(x => x.Email == u.Email))
        return Results.BadRequest(new { mensagem = "E-mail já cadastrado." });

    u.Senha = BCrypt.Net.BCrypt.HashPassword(u.Senha);
    u.DataCadastro = DateTime.UtcNow;
    db.Usuarios.Add(u);
    await db.SaveChangesAsync();
    return Results.Created($"/api/usuarios/{u.Id}", new { u.Id, u.Nome, u.Email });
}).WithTags("Usuários");

// CONSULTAS
app.MapGet("/api/consultas", async (AppDbContext db) =>
    await db.Consultas.ToListAsync())
   .WithTags("Consultas");

app.MapGet("/api/consultas/{id:int}", async (int id, AppDbContext db) =>
    await db.Consultas.FindAsync(id) is Consulta c ? Results.Ok(c) : Results.NotFound())
   .WithTags("Consultas");

app.MapPost("/api/consultas", async (Consulta c, AppDbContext db) =>
{
    db.Consultas.Add(c);
    await db.SaveChangesAsync();
    return Results.Created($"/api/consultas/{c.Id}", c);
}).WithTags("Consultas");

app.MapPut("/api/consultas/{id:int}", async (int id, Consulta input, AppDbContext db) =>
{
    var consulta = await db.Consultas.FindAsync(id);
    if (consulta == null) return Results.NotFound();

    consulta.Especialidade = input.Especialidade;
    consulta.DataHora = input.DataHora;
    consulta.Descricao = input.Descricao;

    await db.SaveChangesAsync();
    return Results.NoContent();
}).WithTags("Consultas");

app.MapDelete("/api/consultas/{id:int}", async (int id, AppDbContext db) =>
{
    if (await db.Consultas.FindAsync(id) is Consulta c)
    {
        db.Consultas.Remove(c);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    return Results.NotFound();
}).WithTags("Consultas");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();