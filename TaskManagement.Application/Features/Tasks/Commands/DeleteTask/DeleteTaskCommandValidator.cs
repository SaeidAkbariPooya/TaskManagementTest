using FluentValidation;

namespace TaskManagement.Application.Features.Tasks.Commands.DeleteTask
{
    public class DeleteTaskCommandValidator : AbstractValidator<DeleteTaskCommand>
    {
        public DeleteTaskCommandValidator()
        {
            RuleFor(x => x.TaskId)
                .NotEmpty().WithMessage("Task ID is required")
                .Must(BeAValidGuid).WithMessage("Task ID must be a valid GUID");

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