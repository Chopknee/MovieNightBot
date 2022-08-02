using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace MovieNightBot.Database {
	public class Controller : DbContext {

		public DbSet<Models.Server> Servers { get; set; }
		public DbSet<Models.Movie> Movies { get; set; }
		public DbSet<Models.IMDBInfo> IMDBInfo { get; set; }
		public DbSet<Models.MovieGenre> MovieGenres { get; set; }
		public DbSet<Models.Vote> Votes { get; set; }
		public DbSet<Models.MovieVote> MovieVotes { get; set; }
		public DbSet<Models.UserVote> UserVotes { get; set; }

		public string DbPath { get; }

		public Controller(string path) {
			DbPath = path;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder options)
			=> options.UseSqlite($"Data Source={DbPath}");

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<Models.Server>()
				.HasKey(server => new { server.Id });

			modelBuilder.Entity<Models.Movie>()
				.HasKey(movie => new { movie.Id });

			modelBuilder.Entity<Models.IMDBInfo>()
				.HasKey(imdb => new { imdb.Id });

			modelBuilder.Entity<Models.MovieGenre>()
				.HasKey(genre => new { genre.Id, genre.Genre });

			// ServerId is currently used to identify a vote. Will need to change in the future to save a server's vote history.
			modelBuilder.Entity<Models.Vote>() 
				.HasKey(vote => new { vote.ServerId});

			modelBuilder.Entity<Models.MovieVote>()
				.HasKey(movievote => new { movievote.Id });

			modelBuilder.Entity<Models.UserVote>()
				.HasKey(uservote => new { uservote.Id });
		}
	}
}