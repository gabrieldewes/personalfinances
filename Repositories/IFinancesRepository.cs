using System;
using System.Collections.Generic;
using MyApp.Models;

namespace MyApp.Repositories {
    public interface IFinancesRepository {
        Decimal GetFinancesSumByUserId(long userId, Dictionary<string, object> searchable);
        Finance GetFinanceById(long id);
        List<Finance> GetAllFinancesByUserId(long userId, Dictionary<string, object> sortable, Dictionary<string, object> searchable);
        long CreateFinance(Finance finance);
        List<FinanceType> GetAllFinanceTypesByUserId(long userId);
        List<Place> GetAllPlacesByUserId(long userId);
        long CreateFinanceType(FinanceType financeType);
        long DeleteFinance(long id);
        long DeleteFinanceType(long id);
        long CreatePlace(Place place);
        long DeletePlace(long id);
        bool CheckForeign(long id, string table, string column);
        long UpdateStatus(long id, int status);
    }
}