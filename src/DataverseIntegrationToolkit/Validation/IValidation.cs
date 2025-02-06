namespace Core.Validation;

public interface IValidation
{
    /// <summary>
    /// Validates data based on the data annotations of the model
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    bool Validate<T>(T data) where T : class;
}