using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DocumentDb>(options => options.UseNpgsql("Host=localhost;Username=postgres;Password=postgres;Database=reciprocal"));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/documents", async (DocumentDb db) => {
    var documents = await db.Documents.ToListAsync();
    return new { documents};
}).WithName("GetDocuments").WithOpenApi();

app.MapPost("/documents", async (CreateDocumentReqDto document, DocumentDb db) => {
    var newDocument = new Document {
        Name = document.Name,
        Content = document.Content,
        CreatedAt = DateTime.UtcNow
    };

    db.Documents.Add(newDocument);
    db.SaveChanges();

    var response = new {
        message = "Document created",
        document
    };

    return Results.Created($"/documents/{newDocument.Id}", response);
}).WithName("CreateDocument").WithOpenApi();

app.MapDelete("/documents/{id}", async (int id, DocumentDb db) => {
    var document = await db.Documents.FindAsync(id);

    if (document is null){
        return Results.NotFound(new { message = "Document not found" });
    }

    db.Documents.Remove(document);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Document deleted" });
});

app.Run();