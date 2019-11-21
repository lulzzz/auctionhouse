using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Common.Exceptions.Command;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Common.Command
{
    public class InvalidCommandException : CommandException
    {
        public InvalidCommandException(string message) : base(message)
        {
        }

        public InvalidCommandException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public abstract class CommandHandlerBase<T> : IRequestHandler<T, CommandResponse> where T : ICommand
    {
        private readonly ILogger _logger;

        protected CommandHandlerBase(ILogger logger)
        {
            _logger = logger;
        }

        public Task<CommandResponse> Handle(T request, CancellationToken cancellationToken)
        {
            var validationContext = new ValidationContext(request);
            var validationResults = new Collection<ValidationResult>();
            if (Validator.TryValidateObject(request, validationContext, validationResults))
            {
                return HandleCommand(request, cancellationToken);
            }
            else
            {
                _logger.LogDebug("Invalid command");
                foreach (var result in validationResults)
                {
                    _logger.LogDebug(
                        $"validation error membernames: {result.MemberNames.Aggregate((s, s1) => s + s1)} message: {result.ErrorMessage}");
                }

                throw new InvalidCommandException($"Invalid command {request.ToString()}");
            }

        }

        protected abstract Task<CommandResponse> HandleCommand(T request, CancellationToken cancellationToken);
    }
}