using System.Text;
using System.Text.Json;

namespace Klyroo.Playground.Data;

/// <summary>Writes the results of the agent tools as the JSON a model would read. The records have no source-generated context, so this writes them by hand (trim safe).</summary>
public static class AgentJson
{
    public static string Write(AgentToolResult result)
    {
        using var stream = new MemoryStream();
        using (var w = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }))
        {
            switch (result)
            {
                case AgentSnapshotResult snapshot:
                    w.WriteStartObject();
                    w.WriteStartArray("components");
                    foreach (var c in snapshot.Components)
                    {
                        w.WriteStartObject();
                        w.WriteString("ref", c.Ref);
                        w.WriteString("component", c.Component);
                        w.WriteString("label", c.Label);
                        w.WritePropertyName("value");
                        Value(w, c.Value);
                        w.WritePropertyName("actions");
                        Strings(w, c.Actions);
                        if (c.Disabled) w.WriteBoolean("disabled", true);
                        if (c.OffPage) w.WriteBoolean("offPage", true);
                        if (c.ConfirmRequired) w.WriteBoolean("confirmRequired", true);
                        if (c.ConfirmActions is { Count: > 0 }) { w.WritePropertyName("confirmActions"); Strings(w, c.ConfirmActions); }
                        w.WriteEndObject();
                    }
                    w.WriteEndArray();
                    w.WriteEndObject();
                    break;
                case AgentQueryResult query:
                    w.WriteStartObject();
                    w.WriteStartArray("matches");
                    foreach (var m in query.Matches)
                    {
                        w.WriteStartObject();
                        w.WriteString("ref", m.Ref);
                        w.WriteString("component", m.Component);
                        w.WriteString("label", m.Label);
                        w.WriteStartArray("items");
                        foreach (var i in m.Items)
                        {
                            w.WriteStartObject();
                            w.WriteString("value", i.Value);
                            w.WriteString("label", i.Label);
                            if (i.Selected) w.WriteBoolean("selected", true);
                            if (i.Disabled) w.WriteBoolean("disabled", true);
                            w.WriteEndObject();
                        }
                        w.WriteEndArray();
                        if (m.More > 0) w.WriteNumber("more", m.More);
                        if (m.Records is { Count: > 0 })
                        {
                            w.WriteNumber("total", m.Total);
                            w.WriteNumber("offset", m.Offset);
                            w.WriteStartArray("records");
                            foreach (var r in m.Records) Record(w, r);
                            w.WriteEndArray();
                        }
                        w.WriteEndObject();
                    }
                    w.WriteEndArray();
                    w.WriteEndObject();
                    break;
                case AgentActResult act:
                    w.WriteStartObject();
                    w.WriteBoolean("ok", act.Ok);
                    w.WriteStartArray("steps");
                    foreach (var s in act.Steps)
                    {
                        w.WriteStartObject();
                        w.WriteString("ref", s.Ref);
                        w.WriteString("action", s.Action);
                        w.WriteBoolean("ok", s.Ok);
                        if (s.Value is not null) { w.WritePropertyName("value"); Value(w, s.Value); }
                        w.WriteEndObject();
                    }
                    w.WriteEndArray();
                    w.WriteStartObject("values");
                    foreach (var kv in act.Values) { w.WritePropertyName(kv.Key); Value(w, kv.Value); }
                    w.WriteEndObject();
                    if (act.Error is not null) w.WriteString("error", act.Error);
                    w.WriteEndObject();
                    break;
                case AgentToolError error:
                    w.WriteStartObject();
                    w.WriteString("error", error.Error);
                    w.WriteEndObject();
                    break;
                default:
                    w.WriteStartObject();
                    w.WriteEndObject();
                    break;
            }
        }
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    private static void Strings(Utf8JsonWriter w, IEnumerable<string> items)
    {
        w.WriteStartArray();
        foreach (var s in items) w.WriteStringValue(s);
        w.WriteEndArray();
    }

    private static void Value(Utf8JsonWriter w, AgentValue? value)
    {
        switch (value)
        {
            case null: w.WriteNullValue(); break;
            case AgentTextValue t: w.WriteStringValue(t.Text); break;
            case AgentNumberValue n: w.WriteRawValue(n.Text); break;
            case AgentBooleanValue b: w.WriteBooleanValue(b.Flag); break;
            case AgentListValue l:
                w.WriteStartArray();
                foreach (var item in l.Items) Value(w, item);
                w.WriteEndArray();
                break;
            case AgentSpreadsheetSummary s:
                w.WriteStartObject();
                w.WriteString("step", s.Step);
                w.WriteBoolean("fileLoaded", s.FileLoaded);
                w.WriteString("fileName", s.FileName);
                w.WriteNumber("rows", s.Rows);
                w.WriteNumber("importableRows", s.ImportableRows);
                w.WriteNumber("invalidRows", s.InvalidRows);
                w.WriteNumber("discardedRows", s.DiscardedRows);
                w.WriteNumber("rowsWithWarnings", s.RowsWithWarnings);
                w.WriteNumber("openQuestions", s.OpenQuestions);
                w.WritePropertyName("unmappedRequiredColumns");
                Strings(w, s.UnmappedRequiredColumns);
                w.WriteBoolean("canSubmit", s.CanSubmit);
                w.WriteString("submitStatus", s.SubmitStatus);
                w.WriteNumber("importedCount", s.ImportedCount);
                w.WriteString("submitError", s.SubmitError);
                w.WriteEndObject();
                break;
            default: w.WriteStringValue(value.ToDisplayString()); break;
        }
    }

    private static void Record(Utf8JsonWriter w, AgentRecord record)
    {
        w.WriteStartObject();
        switch (record)
        {
            case AgentRowRecord r:
                w.WriteNumber("rowNumber", r.RowNumber);
                w.WriteString("status", r.Status);
                w.WriteBoolean("importable", r.Importable);
                w.WriteStartObject("cells");
                foreach (var cell in r.Cells) w.WriteString(cell.Key, cell.Value);
                w.WriteEndObject();
                w.WriteStartArray("issues");
                foreach (var i in r.Issues) { w.WriteStartObject(); w.WriteString("field", i.Field); w.WriteString("code", i.Code); w.WriteString("level", i.Level); w.WriteString("message", i.Message); w.WriteEndObject(); }
                w.WriteEndArray();
                break;
            case AgentIssueRecord i:
                w.WriteString("field", i.Field);
                w.WriteString("code", i.Code);
                w.WriteString("level", i.Level);
                w.WriteString("message", i.Message);
                if (i.RowNumber is { } n) w.WriteNumber("rowNumber", n);
                break;
            default:
                w.WriteString("record", record.GetType().Name);
                break;
        }
        w.WriteEndObject();
    }
}
