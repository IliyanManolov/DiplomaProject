using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Domain.Models;

namespace HomeOwners.Application.Services;

public class DuesCalculationService : IDuesCalculationService
{
    private readonly IDuesCalculationRepository _repository;
    private readonly IPropertyRepository _propertyRepository;

    public DuesCalculationService(IDuesCalculationRepository repository, IPropertyRepository propertyRepository)
    {
        _repository = repository;
        _propertyRepository = propertyRepository;
    }

    public async Task<bool> Calculate()
    {
        var dbCalculation = new DuesCalculation();

        var newCalculation = await _repository.CreateAsync(dbCalculation);

        if (newCalculation == null)
            return false;

        var dbProperties = await _propertyRepository.GetAllAsync();

        foreach (var property in dbProperties)
        {
            property.Dues = property.Dues + property.MonthlyDues;
        }

        await _propertyRepository.UpdateBulkAsync(dbProperties);

        return true;
    }
}
