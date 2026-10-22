using Body4uHUB.Shared.Domain.Base;
using Body4uHUB.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Body4uHUB.Shared.Infrastructure.Tests.Persistence;

[TestFixture]
public class TimestampsInterceptorTests
{
    private static readonly DateTime CreatedAtUtc = new(2026, 1, 15, 10, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime ModifiedAtUtc = CreatedAtUtc.AddHours(2);

    private ManualTimeProvider _timeProvider;
    private DbContextOptions<TestDbContext> _options;

    [SetUp]
    public void Setup()
    {
        _timeProvider = new ManualTimeProvider { UtcNow = CreatedAtUtc };
        _options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(new TimestampsInterceptor(_timeProvider))
            .Options;
    }

    private async Task<int> SaveNewParentAsync()
    {
        using var context = new TestDbContext(_options);
        var parent = new Parent("Name");
        context.Parents.Add(parent);
        await context.SaveChangesAsync();

        return parent.Id;
    }

    [Test]
    public async Task ShouldSetCreatedAt_WhenEntityIsAdded()
    {
        var id = await SaveNewParentAsync();

        using var context = new TestDbContext(_options);
        var parent = await context.Parents.SingleAsync(x => x.Id == id);

        Assert.That(parent.CreatedAt, Is.EqualTo(CreatedAtUtc));
        Assert.That(parent.ModifiedAt, Is.Null);
    }

    [Test]
    public async Task ShouldSetModifiedAtAndKeepCreatedAt_WhenEntityIsModified()
    {
        var id = await SaveNewParentAsync();
        _timeProvider.UtcNow = ModifiedAtUtc;

        using (var context = new TestDbContext(_options))
        {
            var parent = await context.Parents.SingleAsync(x => x.Id == id);
            parent.Rename("New name");
            await context.SaveChangesAsync();
        }

        using (var context = new TestDbContext(_options))
        {
            var parent = await context.Parents.SingleAsync(x => x.Id == id);

            Assert.That(parent.CreatedAt, Is.EqualTo(CreatedAtUtc));
            Assert.That(parent.ModifiedAt, Is.EqualTo(ModifiedAtUtc));
        }
    }

    [Test]
    public async Task ShouldSetCreatedAtOnOwnedEntityAndModifiedAtOnOwner_WhenOwnedEntityIsAdded()
    {
        var id = await SaveNewParentAsync();
        _timeProvider.UtcNow = ModifiedAtUtc;

        using (var context = new TestDbContext(_options))
        {
            var parent = await context.Parents.SingleAsync(x => x.Id == id);
            parent.AddChild("Child");
            await context.SaveChangesAsync();
        }

        using (var context = new TestDbContext(_options))
        {
            var parent = await context.Parents.SingleAsync(x => x.Id == id);

            Assert.That(parent.Children.Single().CreatedAt, Is.EqualTo(ModifiedAtUtc));
            Assert.That(parent.ModifiedAt, Is.EqualTo(ModifiedAtUtc));
            Assert.That(parent.CreatedAt, Is.EqualTo(CreatedAtUtc));
        }
    }

    private sealed class ManualTimeProvider : TimeProvider
    {
        public DateTime UtcNow { get; set; }

        public override DateTimeOffset GetUtcNow() => new(UtcNow);
    }

    private sealed class Parent : AggregateRoot<int>
    {
        private readonly List<Child> _children = [];

        public string Name { get; private set; }
        public IReadOnlyCollection<Child> Children => _children.AsReadOnly();

        private Parent() { }

        public Parent(string name)
        {
            Name = name;
        }

        public void Rename(string name) => Name = name;

        public void AddChild(string name) => _children.Add(new Child(name));
    }

    private sealed class Child : Entity<int>
    {
        public string Name { get; private set; }

        private Child() { }

        public Child(string name)
            : base(default)
        {
            Name = name;
        }
    }

    private sealed class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
    {
        public DbSet<Parent> Parents => Set<Parent>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Parent>(builder =>
            {
                builder.Ignore(x => x.DomainEvents);
                builder.OwnsMany(x => x.Children, childBuilder =>
                {
                    childBuilder.WithOwner().HasForeignKey("ParentId");
                    childBuilder.HasKey(x => x.Id);
                });
            });
        }
    }
}
