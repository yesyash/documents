using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

public class DocumentDb : DbContext
{
    public DbSet<Document> Documents { get; set; }
    public DocumentDb(DbContextOptions<DocumentDb> options) : base(options) { }
}

public class Document
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; }
}