using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

public class DocumentContext : DbContext{
    public DbSet<Document> Documents { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(@"Host=localhost;Username=postgres;Password=postgres;Database=reciprocal");
}

public class Document{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
}