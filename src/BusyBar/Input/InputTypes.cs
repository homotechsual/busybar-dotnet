namespace Busy.Bar;

/// <summary>A physical or virtual key on the device that can be simulated via <c>InputKeySetAsync</c>.</summary>
public enum InputKey
{
    /// <summary>Navigate up.</summary>
    Up,

    /// <summary>Navigate down.</summary>
    Down,

    /// <summary>Confirm/select the current item.</summary>
    Ok,

    /// <summary>Navigate back/cancel.</summary>
    Back,

    /// <summary>Start the currently selected timer.</summary>
    Start,

    /// <summary>Shortcut to the "Busy" timer profile slot.</summary>
    Busy,

    /// <summary>Shortcut to the "Custom" timer profile slot.</summary>
    Custom,

    /// <summary>Power off.</summary>
    Off,

    /// <summary>Open the apps launcher.</summary>
    Apps,

    /// <summary>Open the settings menu.</summary>
    Settings
}

/// <summary>Identifies which key press to simulate.</summary>
/// <param name="Key">The key to press.</param>
public sealed record InputKeyParams(InputKey Key);
