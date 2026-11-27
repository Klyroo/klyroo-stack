namespace Klyroo.Playground.Shared;

/// <summary>One tab of the code panel: a file name, the language that colors it and the text.</summary>
public sealed record CodeTab(string Name, string Language, string Text);
