using FluentValidation;

namespace TodoApi.Validation
{
    public class NewTodoValidator : AbstractValidator<NewTodo>
    {
        public NewTodoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is mandatory!");
        }
    }
}