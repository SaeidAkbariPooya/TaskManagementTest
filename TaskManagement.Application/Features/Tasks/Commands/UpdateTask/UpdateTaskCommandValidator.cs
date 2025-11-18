using FluentValidation;

namespace TaskManagement.Application.Features.Tasks.Commands.UpdateTask
{
    public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
    {
        public UpdateTaskCommandValidator()
        {
            RuleFor(x => x.TaskId)
                .NotEmpty().WithMessage("Task ID is required")
                .Must(BeAValidGuid).WithMessage("Task ID must be a valid GUID");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required")
                .Must(BeAValidGuid).WithMessage("User ID must be a valid GUID");

            RuleFor(x => x.Title)
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters")
                .When(x => !string.IsNullOrEmpty(x.Title));

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Status must be a valid task status")
                .When(x => x.Status.HasValue);

            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("Priority must be a valid priority level")
                .When(x => x.Priority.HasValue);
        }

        private bool BeAValidGuid(Guid guid)
        {
            return guid != Guid.Empty;
        }
    }
}
