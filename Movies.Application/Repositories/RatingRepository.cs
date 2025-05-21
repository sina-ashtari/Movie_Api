using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class RatingRepository : IRatingRepository
{
    private readonly IDbConnectionFactory _dbFactory;

    public RatingRepository(IDbConnectionFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbFactory.CreateConnectionAsync(cancellationToken);
        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         insert into ratings(userid, movieid, rating)
                                                                         values (@userid, @movieid, @rating) 
                                                                         on conflict(userid, movieid) do update set rating = @rating where userid = @userid
                                                                         """, new {userId, movieId, rating}, cancellationToken: cancellationToken));
        return result > 0;
    }

    public async Task<float?> GetRatingAsync(Guid movieId, CancellationToken token = default)
    {
        using var connection = await _dbFactory.CreateConnectionAsync(token);
        return await connection.QueryFirstOrDefaultAsync<float?>(new CommandDefinition("""
             select round(avg(r.rating), 1) from ratings r
             where movieid = @movieId
             """, new { movieId }, cancellationToken: token));
    }

    public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId, CancellationToken token = default)
    {
        using var connection = await _dbFactory.CreateConnectionAsync(token);
        return await connection.QueryFirstOrDefaultAsync<(float?, int?)>(new CommandDefinition("""
            select round(avg(rating), 1), (
                select rating 
                from ratings
                where movieid = @movieId and userid = @userId
                limit 1
            )
            from ratings r
            where movieid = @movieId
            """, new { movieId }, cancellationToken: token));
    }

    public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbFactory.CreateConnectionAsync(cancellationToken);
        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                                             delete from ratings
                                                                         where movieid = @movieId and userid = @userId
                                                                         """, new { userId, movieId }, cancellationToken: cancellationToken));
        return result > 0;
    }

    public async Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbFactory.CreateConnectionAsync(cancellationToken);
        return await connection.QueryAsync<MovieRating>(new CommandDefinition("""
                                                                              select r.rating, r.movieid, m.slug 
                                                                              from ratings r
                                                                              inner join movies m on r.movieid = m.id
                                                                              where userid = @userId
                                                                              
                                                                              
                                                                              """, new { userId }, cancellationToken: cancellationToken));
    }
}