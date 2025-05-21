using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers.v1
{
    
    [ApiController]
    [ApiVersion(1.0)]
    public class MoviesController : ControllerBase 
    {
        private readonly IMovieService _movieService;
        private readonly IOutputCacheStore _outputCache;
        public MoviesController(IMovieService movieService, IOutputCacheStore outputCache)
        {
            _movieService = movieService;
            _outputCache = outputCache;
        }
        [Authorize(AuthConstants.AdminUserPolicyName)]
        [HttpPost(ApiEndpoints.Movies.Create)]
        [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken cancellationToken)
        {
            var movie = request.MapToMovie();
            await _movieService.CreateAsync(movie, cancellationToken);
            await _outputCache.EvictByTagAsync("movies", cancellationToken);
            return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie);
            
        }
        [HttpGet(ApiEndpoints.Movies.Get)]
        [OutputCache(PolicyName = "MovieCache")]
        //[ResponseCache(Duration = 4000, VaryByHeader = "Accept, Accept-Encoding", Location = ResponseCacheLocation.Any)]
        [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] string idOrSlug, [FromServices] LinkGenerator linkGenerator,CancellationToken cancellationToken )
        {
            var userId = HttpContext.GetUserId();
            var movie = Guid.TryParse(idOrSlug, out var id)
                ? await _movieService.GetByIdAsync(id, userId, cancellationToken)
                : await _movieService.GetBySlugAsync(idOrSlug, userId, cancellationToken);

            if (movie == null) return NotFound();
            var response  = movie.MapToResponse();
            var movieObject = new { Id = movie.Id };
            response.Links.Add(new Link
            {
                Href = linkGenerator.GetPathByAction(HttpContext, nameof(Get), values: new { idOrSlug = movieObject.Id })!,
                Rel = "self",
                Type = "GET"
            });
            response.Links.Add(new Link
            {
                Href = linkGenerator.GetPathByAction(HttpContext, nameof(Update), values: (movieObject))!,
                Rel = "self",
                Type = "PUT"
            });
            response.Links.Add(new Link
            {
                Href = linkGenerator.GetPathByAction(HttpContext, nameof(Delete), values: (movieObject))!,
                Rel = "self",
                Type = "DELETE"
            });
            
            return Ok(response);

            
        }   
        [HttpGet(ApiEndpoints.Movies.GetAll)]
        [OutputCache(PolicyName = "MovieCache")]
        //[ResponseCache(Duration = 400, VaryByQueryKeys = ["title", "year", "sortBy", "page", "pageSize"])]
        [ProducesResponseType(typeof(MoviesResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllMoviesRequest request, CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();
            var options = request.MapToOptions().WithUser(userId);
            var movies = await _movieService.GetAllAsync(options, cancellationToken);
            var movieCount = await _movieService.GetCountAsync(options.Title, options.Year, cancellationToken);
            var response = movies.MapToResponse(request.Page, request.PageSize, movieCount);
            return Ok(response);


        }
        [Authorize(AuthConstants.TrustedMemberPolicyName)]
        [HttpPut(ApiEndpoints.Movies.Update)]
        [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromRoute]Guid id, [FromBody]UpdateMovieRequest request, CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();
            var movie  = request.MapToMovie(id);
            var updatedMovie = await _movieService.UpdateAsync(movie, userId, cancellationToken);
            if(updatedMovie is null) return NotFound();
            var response = updatedMovie.MapToResponse();
            await _outputCache.EvictByTagAsync("movies", cancellationToken);
            return Ok(response);
        }
        [Authorize(AuthConstants.TrustedMemberPolicyName)]
        [HttpDelete(ApiEndpoints.Movies.Delete)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var deleted = await _movieService.DeleteAsync(id, cancellationToken);
            await _outputCache.EvictByTagAsync("movies", cancellationToken);
            if (!deleted) NotFound();
            return Ok();        
        }
    }
}
