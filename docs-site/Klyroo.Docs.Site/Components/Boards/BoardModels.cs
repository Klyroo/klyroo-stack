namespace Klyroo.Docs.Site.Components.Boards;

/// <summary>A line of the code panel of a board. <paramref name="Features"/> lists the chips (separated by spaces) whose choice lights the line.</summary>
/// <param name="Text">The code of the line, as written (indentation included).</param>
/// <param name="Language">The highlighter of the line: <c>csharp</c>, <c>html</c> (markup) or <c>text</c>.</param>
public sealed record BoardLine(string Text, string Features = "", string Language = "csharp");

/// <summary>A feature the reader can pick on a board: its id (what <c>data-lines</c> and <c>data-target</c> name), its label and the sentence under the code while it is picked.</summary>
public sealed record BoardChip(string Id, string Label, string Caption);
