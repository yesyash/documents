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

var documents = app.MapGroup("/documents");

documents.MapGet("/", GetAllDocuments).WithName("GetAllDocuments").WithOpenApi();
documents.MapGet("/{id}", GetDocumentById);
documents.MapPost("/", CreateDocument);
documents.MapDelete("/{id}", DeleteDocument);

app.Run();

static async Task<IResult> GetAllDocuments(DocumentDb db)
{
    var documents = await db.Documents.ToArrayAsync();
    return TypedResults.Ok(new { documents });
}

static async Task<IResult> GetDocumentById(int id, DocumentDb db)
{
    var document = await db.Documents.FindAsync(id);

    if (document is null)
    {
        return TypedResults.NotFound(new { message = "Document not found" });
    }

    return TypedResults.Ok(new { document });
}

static async Task<IResult> CreateDocument(CreateDocumentReqDto document, DocumentDb db)
{
    var newDocument = new Document
    {
        Name = document.Name,
        Content = document.Content,
        CreatedAt = DateTime.UtcNow
    };

    db.Documents.Add(newDocument);
    await db.SaveChangesAsync();

    var response = new
    {
        message = "Document created",
        document
    };

    return TypedResults.Created($"/documents/{newDocument.Id}", response);
}

static async Task<IResult> DeleteDocument(int id, DocumentDb db)
{
    var document = await db.Documents.FindAsync(id);

    if (document is null)
    {
        return TypedResults.NotFound(new { message = "Document not found" });
    }

    db.Documents.Remove(document);
    await db.SaveChangesAsync();
    return TypedResults.Ok(new { message = "Document deleted" });
}