// Win/Helpers/PartyMemory.cs
using System.Collections.Concurrent;

public static class PartyMemory
{
    private static readonly ConcurrentDictionary<string, int> _buyerMap = new();
    private static readonly ConcurrentDictionary<string, int> _vendorMap = new();

    public static void RememberBuyer(string fileName, int id) => _buyerMap[Norm(fileName)] = id;
    public static int? RecallBuyer(string fileName) => _buyerMap.TryGetValue(Norm(fileName), out var id) ? id : null;

    public static void RememberVendor(string fileName, int id) => _vendorMap[Norm(fileName)] = id;
    public static int? RecallVendor(string fileName) => _vendorMap.TryGetValue(Norm(fileName), out var id) ? id : null;

    private static string Norm(string s) => s.ToLowerInvariant().Replace(" ", "").Replace("_", "").Replace("-", "");
}
