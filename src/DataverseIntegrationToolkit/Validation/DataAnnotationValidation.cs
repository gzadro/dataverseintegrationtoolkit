using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Core.Validation
{
	/// <summary>
	/// Represents Data Annotation validator.
	/// Purpose is to validate object using System.ComponentModel
	/// </summary>
	public class DataAnnotationValidation : IValidation
	{
		private readonly ILogger _logger;

		public DataAnnotationValidation(ILogger<DataAnnotationValidation> logger)
		{
			_logger = logger;
		}

        /// <inheritdoc />
        public bool Validate<T>(T data) where T : class
		{
			var context = new ValidationContext(data);
			var validationResults = new List<ValidationResult>();

			var isValid = Validator.TryValidateObject(data, context, validationResults, true);

			if (!isValid)
			{
				_logger.LogWarning("Object is not valid.");

				foreach (var vl in validationResults)
				{
                    var members = vl.MemberNames != null
						? string.Join(", ", vl.MemberNames)
						: "None";
                    _logger.LogWarning($"{vl.ErrorMessage}, {members}");
				}
			}

			if (isValid)
			{
				_logger.LogInformation("Object is valid.");
			}

			return isValid;
		}
	}
}
