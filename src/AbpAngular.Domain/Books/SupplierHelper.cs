using System;
using System.Collections.Generic;
using System.Linq;

namespace AbpAngular.Books;

public static class SupplierHelper
{
    /// <summary>
    /// Convert comma-separated supplier IDs string to List of Guid
    /// </summary>
    /// <param name="supplierIds">Comma-separated supplier IDs</param>
    /// <returns>List of Guid</returns>
    public static List<Guid> ConvertStringToSupplierIds(string supplierIds)
    {
        if (string.IsNullOrWhiteSpace(supplierIds))
            return new List<Guid>();

        return supplierIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(id => Guid.TryParse(id.Trim(), out var guid) ? guid : Guid.Empty)
            .Where(id => id != Guid.Empty)
            .ToList();
    }

    /// <summary>
    /// Convert List of Guid to comma-separated string
    /// </summary>
    /// <param name="supplierIds">List of supplier IDs</param>
    /// <returns>Comma-separated string</returns>
    public static string ConvertSupplierIdsToString(List<Guid> supplierIds)
    {
        return supplierIds?.Any() == true ? string.Join(",", supplierIds) : string.Empty;
    }

    /// <summary>
    /// Add supplier ID to comma-separated string
    /// </summary>
    /// <param name="currentSuppliers">Current comma-separated supplier IDs</param>
    /// <param name="newSupplierId">New supplier ID to add</param>
    /// <returns>Updated comma-separated string</returns>
    public static string AddSupplierToString(string currentSuppliers, Guid newSupplierId)
    {
        var supplierIds = ConvertStringToSupplierIds(currentSuppliers);
        
        if (!supplierIds.Contains(newSupplierId))
        {
            supplierIds.Add(newSupplierId);
        }
        
        return ConvertSupplierIdsToString(supplierIds);
    }

    /// <summary>
    /// Remove supplier ID from comma-separated string
    /// </summary>
    /// <param name="currentSuppliers">Current comma-separated supplier IDs</param>
    /// <param name="supplierIdToRemove">Supplier ID to remove</param>
    /// <returns>Updated comma-separated string</returns>
    public static string RemoveSupplierFromString(string currentSuppliers, Guid supplierIdToRemove)
    {
        var supplierIds = ConvertStringToSupplierIds(currentSuppliers);
        supplierIds.Remove(supplierIdToRemove);
        
        return ConvertSupplierIdsToString(supplierIds);
    }
}
