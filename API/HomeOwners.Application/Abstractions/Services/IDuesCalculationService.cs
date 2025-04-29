namespace HomeOwners.Application.Abstractions.Services;

public interface IDuesCalculationService
{
    public Task<bool> Calculate();
}