using FluentValidation;
using FluentValidation.Results;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class RatingService : IRatingService
{
    private readonly IRatingRepository _ratingRepository;
    private readonly IMoviesRepository _moviesRepository;
    public RatingService(IRatingRepository ratingRepository, IMoviesRepository moviesRepository)
    {
        _ratingRepository = ratingRepository;
        _moviesRepository = moviesRepository;
    }

    public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken cancellationToken = default)
    {
        if (rating is <= 0 or > 5)
        {
            throw new ValidationException([
                new ValidationFailure
                {
                    PropertyName = "Rating",
                    ErrorMessage = "Rating must be between 0 and 5"
                }
            ]);
        }
        var movieExists  = await _moviesRepository.ExistsByIdAsync(movieId, cancellationToken);
        if(!movieExists) return false;
        return await _ratingRepository.RateMovieAsync(movieId, rating, userId, cancellationToken); 
        
    }

    public Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken cancellationToken = default)
    {
        return _ratingRepository.DeleteRatingAsync(movieId, userId, cancellationToken);
    }

    public Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _ratingRepository.GetRatingsForUserAsync(userId, cancellationToken);
    }
}