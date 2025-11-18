using FluentValidation;

namespace TaskManagement.Application.Features.Tasks.Commands.CreateTask
{
    public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
    {
        public CreateTaskCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters")
                .MinimumLength(3).WithMessage("Title must be at least 3 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("Priority must be a valid value");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required")
                .Must(BeAValidGuid).WithMessage("User ID must be a valid GUID");
        }

        private bool BeAValidGuid(Guid guid)
        {
            return guid != Guid.Empty;
        }
    }
}
