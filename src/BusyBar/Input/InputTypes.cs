namespace Busy.Bar;

public enum InputKey { Up, Down, Ok, Back, Start, Busy, Custom, Off, Apps, Settings }

public sealed record InputKeyParams(InputKey Key);
