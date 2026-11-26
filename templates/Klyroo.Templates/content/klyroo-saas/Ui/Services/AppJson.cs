using System.Text.Json;
using System.Text.Json.Serialization;
using Klyroo.Query;

namespace KlyrooSaaSApp.Services;

/// <summary>
/// The types that the prerender hands to the interactive render, listed once for the source generator so that no reflection is needed (the app can be
/// trimmed). The dashboard resources and the result of the accounts list use it.
/// </summary>
[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(Summary))]
[JsonSerializable(typeof(RevenuePoint[]))]
[JsonSerializable(typeof(PlanShare[]))]
[JsonSerializable(typeof(ActivityEntry[]))]
[JsonSerializable(typeof(DataListResult<Account>))]
public sealed partial class AppJson : JsonSerializerContext;
