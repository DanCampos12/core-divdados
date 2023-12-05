using Core.Divdados.Domain.UserContext.Entities;

namespace Core.Divdados.Domain.UserContext.Results;

public class PreferenceResult
{
    public PreferenceResult() { }

    private PreferenceResult(Preference preference)
    {
        Dark = preference.Dark;
        DisplayValues = preference.DisplayValues;
    }

    public bool Dark { get; set; }
    public bool DisplayValues { get; set; }

    public static PreferenceResult Create(Preference preference) => new(preference);
}
