using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Application.Services;

namespace Movies.Application.Validation;

public class MovieValidator: AbstractValidator<Movie>
{
    private readonly IMoviesRepository _moviesRepository;
    public MovieValidator(IMoviesRepository movieService)
    {
        _moviesRepository = movieService;
        
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Genres).NotEmpty();
        RuleFor(x => x.YearOfRelease).LessThanOrEqualTo(DateTime.UtcNow.Year);
        RuleFor(x => x.Slug).MustAsync(ValidateSlug).WithMessage("This movie is already exists in system");
    }

    private async Task<bool> ValidateSlug(Movie movie, string slug, CancellationToken cancellationToken)
    {
        var existingMovies = await _moviesRepository.GetBySlugAsync(slug);
        if (existingMovies is not null)
        {
            return existingMovies.Id == movie.Id;
        }

        return existingMovies is null;
    }
}