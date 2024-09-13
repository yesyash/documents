var builder = WebApplication.CreateBuilder(args);

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

app.MapGet("/documents", () => {
    using var db = new DocumentContext();
    var documents = db.Documents.ToList().OrderBy(d => d.CreatedAt);
    return new { documents};
}).WithName("GetDocuments").WithOpenApi();

app.MapPost("/documents", (CreateDocumentReqDto document) => {
    using var db = new DocumentContext();
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

app.MapGet("/health", () => {
    return new { message = "Healthy" };
}).WithName("HealthCheck").WithOpenApi();

app.Run();