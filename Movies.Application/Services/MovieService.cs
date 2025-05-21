using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class MovieService : IMovieService
{
    private readonly IMoviesRepository _moviesRepository;
    private readonly IRatingRepository _ratingRepository;
    private readonly IValidator<Movie> _validator;
    private readonly IValidator<GetAllMoviesOptions> _optionsValidator;

    public MovieService(IMoviesRepository moviesRepository, IValidator<Movie> validator, IRatingRepository ratingRepository, IValidator<GetAllMoviesOptions> optionsValidator)
    {
        _moviesRepository = moviesRepository;
        _validator = validator;
        _ratingRepository = ratingRepository;
        _optionsValidator = optionsValidator;
    }

    public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
    {
        await _validator.ValidateAndThrowAsync(movie, cancellationToken: token);
        return await _moviesRepository.CreateAsync(movie, token);
    }

    public Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default)
    {
        return _moviesRepository.GetByIdAsync(id, userId, token);
    }

    public Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default)
    {
        return _moviesRepository.GetBySlugAsync(slug, userId, token);
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token = default)
    {
        await _optionsValidator.ValidateAndThrowAsync(options, cancellationToken: token);
        return await _moviesRepository.GetAllAsync(options, token);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken token = default)
    {
        return _moviesRepository.DeleteByIdAsync(id, token);
    }

    public async Task<Movie?> UpdateAsync(Movie movie, Guid? userId = default, CancellationToken token = default)
    {
        await _validator.ValidateAndThrowAsync(movie, cancellationToken: token);
        var movieExist = await _moviesRepository.ExistsByIdAsync(movie.Id, token);
        if (!movieExist) return null;
        
        await _moviesRepository.UpdateAsync(movie, token);
        if (!userId.HasValue)
        {
            var rating = await _ratingRepository.GetRatingAsync(movie.Id, token);
            movie.Rating = rating;
            return movie;
        }
        
        var ratings = await _ratingRepository.GetRatingAsync(movie.Id, userId.Value, token);
        movie.Rating = ratings.Rating;
        movie.UserRating = ratings.UserRating;
        
        return movie;
        
    }

    public Task<int> GetCountAsync(string? title, int? yearOfRelease, CancellationToken token = default)
    {
        return _moviesRepository.GetCountAsync(title, yearOfRelease, token);
    }
}