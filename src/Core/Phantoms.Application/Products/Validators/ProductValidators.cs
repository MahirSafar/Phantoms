using FluentValidation;
using Phantoms.Application.Products.Commands;

namespace Phantoms.Application.Products.Validators;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Dto.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters.");

        RuleFor(x => x.Dto.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(x => x.Dto.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");

        RuleFor(x => x.Dto.Category)
            .MaximumLength(100).WithMessage("Category must not exceed 100 characters.")
            .When(x => x.Dto.Category is not null);

        RuleFor(x => x.Dto.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.")
            .When(x => x.Dto.Description is not null);

        RuleFor(x => x.Dto.ThumbnailUrl)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Thumbnail URL must be a valid URL.")
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.ThumbnailUrl));

        RuleForEach(x => x.Dto.ImageUrls)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Each image URL must be a valid URL.");
    }
}

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(x => x.Dto.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters.");

        RuleFor(x => x.Dto.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(x => x.Dto.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");

        RuleFor(x => x.Dto.Category)
            .MaximumLength(100).WithMessage("Category must not exceed 100 characters.")
            .When(x => x.Dto.Category is not null);

        RuleFor(x => x.Dto.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.")
            .When(x => x.Dto.Description is not null);

        RuleFor(x => x.Dto.ThumbnailUrl)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Thumbnail URL must be a valid URL.")
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.ThumbnailUrl));
    }
}

public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID is required.");
    }
}
