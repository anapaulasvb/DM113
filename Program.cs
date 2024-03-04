namespace WebAppDb;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddDbContext<AppDbContext>(); //banco de dados
        builder.Services.AddAuthorization();
        builder.Services.AddEndpointsApiExplorer();

        var app = builder.Build();

        app.MapGet("/", () => { return "Cadastro de alunos e notas.\nPara cadastrar, informe o nome do aluno (name) e a nota (grade)"; });

        //CREATE
        app.MapPost("/", (Student student, AppDbContext db) => {
            db.Students.Add(student);
            Console.WriteLine($"Student: {student.Id}");
            db.SaveChanges();
            return Results.Created();
        });

        //READ
        app.MapGet("/students", (AppDbContext db) => {
            var students = db.Students.ToList();
            return students;
        });

        app.MapGet("/students/{id}", (HttpContext context) => {
            if (!int.TryParse(context.Request.RouteValues["id"]?.ToString(), out int id)) {
                context.Response.StatusCode = 400;
                return Results.BadRequest("Invalid student ID.");
            }

            var db = context.RequestServices.GetRequiredService<AppDbContext>();
            var student = db.Students.Find(id);
            if (student == null) {
                context.Response.StatusCode = 404;
                return Results.NotFound($"Student with id {id} not found.");
            }
            return Results.Ok(student);
        });

        //UPDATE
        app.MapPut("/students/{id}", async (HttpContext context, AppDbContext db) => {
            if (!int.TryParse(context.Request.RouteValues["id"]?.ToString(), out int id)) {
                context.Response.StatusCode = 400;
                return Results.BadRequest("Invalid student ID.");
            }

            var updatedStudent = await context.Request.ReadFromJsonAsync<Student>();
            var existingStudent = await db.Students.FindAsync(id);
            if (existingStudent == null) {
                context.Response.StatusCode = 404;
                return Results.NotFound($"Student with id {id} not found.");
            }

            if (updatedStudent != null) {
                existingStudent.Name = updatedStudent.Name;
                existingStudent.Grade = updatedStudent.Grade;
            }
            await db.SaveChangesAsync();
            return Results.Ok(existingStudent);
        });

        //DELETE
        app.MapDelete("/students/{id}", async (HttpContext context, AppDbContext db) => {
            if (!int.TryParse(context.Request.RouteValues["id"]?.ToString(), out int studentId)) {
                context.Response.StatusCode = 400;
                return Results.BadRequest("Invalid student ID.");
            }

            var existingStudent = await db.Students.FindAsync(studentId);
            if (existingStudent == null) {
                return Results.NotFound($"Student with id {studentId} not found.");
            }

            var deletedStudent = existingStudent;
            db.Students.Remove(existingStudent);
            await db.SaveChangesAsync();
            return Results.Ok(deletedStudent);
        });

        app.Run();
    }    
}