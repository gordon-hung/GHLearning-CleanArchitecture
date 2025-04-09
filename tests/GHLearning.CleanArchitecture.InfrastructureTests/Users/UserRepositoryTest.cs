using GHLearning.CleanArchitecture.Core.Users;
using GHLearning.CleanArchitecture.Infrastructure.Entities;
using GHLearning.CleanArchitecture.Infrastructure.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GHLearning.CleanArchitecture.InfrastructureTests.Users;

public class UserRepositoryTest
{
	[Fact]
	public async Task Created()
	{
		var options = new DbContextOptionsBuilder<SampleContext>()
			.UseInMemoryDatabase(databaseName: $"dbo.{nameof(Created)}")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new SampleContext(options);
		_ = context.Database.EnsureDeleted();
		_ = context.Database.EnsureCreated();

		var source = new UserCreated(
			Id: Guid.NewGuid(),
			Email: "Email@gmail.com",
			FirstName: "FirstName",
			LastName: "LastName",
			PasswordHash: "PasswordHash");

		var sut = new UserRepository(context);

		await sut.CreatedAsync(source);

		var actual = await context.Users
		.Where(x => x.Id == source.Id)
		.SingleAsync();

		Assert.NotNull(actual);
		Assert.Equal(source.Id, actual.Id);
		Assert.Equal(source.Email.ToLower(), actual.Email);
		Assert.Equal(source.FirstName, actual.FirstName);
		Assert.Equal(source.LastName, actual.LastName);
		Assert.Equal(source.PasswordHash, actual.PasswordHash);
	}

	[Fact]
	public async Task Created_InvalidOperationException()
	{
		var options = new DbContextOptionsBuilder<SampleContext>()
			.UseInMemoryDatabase(databaseName: $"dbo.{nameof(Created_InvalidOperationException)}")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new SampleContext(options);
		_ = context.Database.EnsureDeleted();
		_ = context.Database.EnsureCreated();

		var user = new User
		{
			Id = Guid.NewGuid(),
			Email = "email@gmail.com",
			FirstName = "firstName",
			LastName = "lastName",
			PasswordHash = "passwordHash"
		};
		_ = await context.Users.AddAsync(user);
		_ = await context.SaveChangesAsync();

		var source = new UserCreated(
			Id: user.Id,
			Email: "Email@gmail.com",
			FirstName: "FirstName",
			LastName: "LastName",
			PasswordHash: "PasswordHash");

		var sut = new UserRepository(context);

		await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CreatedAsync(source));
	}

	[Fact]
	public async Task Created_MySqlException_Duplicate()
	{
		var options = new DbContextOptionsBuilder<SampleContext>()
			.UseInMemoryDatabase(databaseName: $"dbo.{nameof(Created_MySqlException_Duplicate)}")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new SampleContext(options);
		_ = context.Database.EnsureDeleted();
		_ = context.Database.EnsureCreated();

		var user = new User
		{
			Id = Guid.NewGuid(),
			Email = "email@gmail.com",
			FirstName = "firstName",
			LastName = "lastName",
			PasswordHash = "passwordHash"
		};
		_ = context.Users.Add(user);
		_ = context.SaveChanges();

		var source = new UserCreated(
			Id: Guid.NewGuid(),
			Email: "Email@gmail.com",
			FirstName: "FirstName",
			LastName: "LastName",
			PasswordHash: "PasswordHash");

		var sut = new UserRepository(context);

		await sut.CreatedAsync(source);

		var actual = await context.Users
			.Where(x => x.Email == "email@gmail.com")
			.CountAsync();

		Assert.Equal(2, actual);
	}

	[Fact]
	public async Task Get_UserId()
	{
		var options = new DbContextOptionsBuilder<SampleContext>()
			.UseInMemoryDatabase(databaseName: $"dbo.{nameof(Get_UserId)}")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new SampleContext(options);
		_ = context.Database.EnsureDeleted();
		_ = context.Database.EnsureCreated();

		var user = new User
		{
			Id = Guid.NewGuid(),
			Email = "email@gmail.com",
			FirstName = "FirstName",
			LastName = "LastName",
			PasswordHash = "PasswordHash"
		};
		_ = context.Users.Add(user);
		_ = context.SaveChanges();

		var sut = new UserRepository(context);

		var actual = await sut.GetAsync(user.Id);

		Assert.NotNull(actual);
		Assert.Equal(user.Id, actual.Id);
		Assert.Equal(user.Email, actual.Email);
		Assert.Equal(user.FirstName, actual.FirstName);
		Assert.Equal(user.LastName, actual.LastName);
		Assert.Equal(user.PasswordHash, actual.PasswordHash);
	}

	[Fact]
	public async Task Get_Email()
	{
		var options = new DbContextOptionsBuilder<SampleContext>()
			.UseInMemoryDatabase(databaseName: $"dbo.{nameof(Get_Email)}")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new SampleContext(options);
		_ = context.Database.EnsureDeleted();
		_ = context.Database.EnsureCreated();

		var user = new User
		{
			Id = Guid.NewGuid(),
			Email = "email@gmail.com",
			FirstName = "FirstName",
			LastName = "LastName",
			PasswordHash = "PasswordHash"
		};
		_ = context.Users.Add(user);
		_ = context.SaveChanges();

		var sut = new UserRepository(context);

		var actual = await sut.GetAsync("Email@gmail.com");

		Assert.NotNull(actual);
		Assert.Equal(user.Id, actual.Id);
		Assert.Equal(user.Email, actual.Email);
		Assert.Equal(user.FirstName, actual.FirstName);
		Assert.Equal(user.LastName, actual.LastName);
		Assert.Equal(user.PasswordHash, actual.PasswordHash);
	}

	[Fact]
	public async Task IsEmailUnique_False()
	{
		var options = new DbContextOptionsBuilder<SampleContext>()
			.UseInMemoryDatabase(databaseName: $"dbo.{nameof(IsEmailUnique_False)}")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new SampleContext(options);
		_ = context.Database.EnsureDeleted();
		_ = context.Database.EnsureCreated();

		var user = new User
		{
			Id = Guid.NewGuid(),
			Email = "email@gmail.com",
			FirstName = "FirstName",
			LastName = "LastName",
			PasswordHash = "PasswordHash"
		};
		_ = context.Users.Add(user);
		_ = context.SaveChanges();

		var sut = new UserRepository(context);

		var actual = await sut.IsEmailUniqueAsync("Email@gmail.com");

		Assert.False(actual);
	}

	[Fact]
	public async Task IsEmailUnique_True()
	{
		var options = new DbContextOptionsBuilder<SampleContext>()
			.UseInMemoryDatabase(databaseName: $"dbo.{nameof(IsEmailUnique_True)}")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new SampleContext(options);
		_ = context.Database.EnsureDeleted();
		_ = context.Database.EnsureCreated();

		var user = new User
		{
			Id = Guid.NewGuid(),
			Email = "email@gmail.com",
			FirstName = "FirstName",
			LastName = "LastName",
			PasswordHash = "PasswordHash"
		};
		_ = context.Users.Add(user);
		_ = context.SaveChanges();

		var sut = new UserRepository(context);

		var actual = await sut.IsEmailUniqueAsync("Email2@gmail.com");

		Assert.True(actual);
	}

	[Fact]
	public async Task UpdatePasswordHash()
	{
		var options = new DbContextOptionsBuilder<SampleContext>()
			.UseInMemoryDatabase(databaseName: $"dbo.{nameof(UpdatePasswordHash)}")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new SampleContext(options);
		_ = context.Database.EnsureDeleted();
		_ = context.Database.EnsureCreated();

		var user = new User
		{
			Id = Guid.NewGuid(),
			Email = "email@gmail.com",
			FirstName = "FirstName",
			LastName = "LastName",
			PasswordHash = "PasswordHash"
		};
		_ = context.Users.Add(user);
		_ = context.SaveChanges();

		var source = new UserUpdatePassword(
			Id: user.Id,
			Email: user.Email,
			PasswordHash: "NewPasswordHash");

		var sut = new UserRepository(context);

		await sut.UpdatePasswordHashAsync(source);

		var actual = await context.Users
		.Where(x => x.Id == source.Id)
		.SingleAsync();

		Assert.Equal(source.PasswordHash, actual.PasswordHash);
	}
}
