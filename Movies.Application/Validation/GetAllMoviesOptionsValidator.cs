using FluentValidation;
using Movies.Application.Models;

namespace Movies.Application.Validation;

public class GetAllMoviesOptionsValidator : AbstractValidator<GetAllMoviesOptions>
{
    private static readonly string[] AcceptableSortFields =
    {
        "title","yearofrelease"
    };
    public GetAllMoviesOptionsValidator()
    {
        RuleFor(x => x.Year).LessThanOrEqualTo(DateTime.UtcNow.Year);
        RuleFor(x => x.SortField).Must(x => x is null || AcceptableSortFields.Contains(x, StringComparer.OrdinalIgnoreCase)).WithMessage("you can only sort by 'title' or 'yearofrelease'");
    }
}