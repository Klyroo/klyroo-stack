using System.Text.Json;
using System.Text.Json.Serialization;

namespace KlyrooAdminApp.Services;

/// <summary>
/// The types that the prerender hands to the interactive render through DashboardPersistence, listed once for the source generator so that no reflection
/// is needed (the app can be trimmed).
/// </summary>
[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(Summary))]
[JsonSerializable(typeof(ArrPoint[]))]
[JsonSerializable(typeof(RegionShare[]))]
[JsonSerializable(typeof(TopCustomer[]))]
[JsonSerializable(typeof(ActivityEntry[]))]
public sealed partial class AppJson : JsonSerializerContext;
