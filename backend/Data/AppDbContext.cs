using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using todoApp.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<TodoTask> TodoTasks { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<TaskTag> TaskTags { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(u => u.Tasks)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId);
        modelBuilder.Entity<TodoTask>()
            .HasMany(tags => tags.TaskTags)
            .WithOne(tt => tt.TodoTask)
            .HasForeignKey(tt => tt.TodoTaskId);
        modelBuilder.Entity<Tag>()
            .HasIndex(t => t.Name)
            .IsUnique();
        modelBuilder.Entity<Tag>()
            .HasMany(tt => tt.TaskTags)
            .WithOne(tt => tt.Tag)
            .HasForeignKey(tt => tt.TagId);
        modelBuilder.Entity<Tag>()
            .Property(t => t.Name)    // seleciona a propriedade 'Name'
            .HasMaxLength(50)         // define o tamanho máximo da coluna no banco para 50 caracteres
            .IsRequired();            // garante que a coluna não seja nula (opcional)
        modelBuilder.Entity<TaskTag>()
            .HasKey(tt => new { tt.TodoTaskId, tt.TagId }); // Define a chave primária composta
        modelBuilder.Entity<TaskTag>()
            .HasOne(tt => tt.TodoTask)
            .WithMany(t => t.TaskTags)
            .HasForeignKey(tt => tt.TodoTaskId);
        modelBuilder.Entity<TaskTag>()
            .HasOne(tt => tt.Tag)
            .WithMany(t => t.TaskTags)
            .HasForeignKey(tt => tt.TagId);

        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
    v => v.ToUniversalTime(),
    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? v.Value.ToUniversalTime() : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                    property.SetValueConverter(dateTimeConverter);

                if (property.ClrType == typeof(DateTime?))
                    property.SetValueConverter(nullableDateTimeConverter);
            }
        }


        base.OnModelCreating(modelBuilder);
    }

}
