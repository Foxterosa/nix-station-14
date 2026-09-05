using Robust.Shared.Serialization;
using Content.Shared.FixedPoint;

namespace Content.Shared.MedicalScanner;

[Serializable, NetSerializable]
public sealed class HealthAnalyzerScannedUserMessage(HealthAnalyzerUiState state) : BoundUserInterfaceMessage
{
    public readonly HealthAnalyzerUiState State = state;
}

// Starlight-start: Printable health reports.
[Serializable, NetSerializable]
public sealed class HealthAnalyzerPrintReportMessage : BoundUserInterfaceMessage
{
}
// Starlight-end

[Serializable, NetSerializable]
public struct HealthAnalyzerUiState
{
    public NetEntity? TargetEntity;
    public float Temperature;
    public float BloodLevel;
    public bool? CanPrint;
    public bool? ScanMode;
    public bool? Bleeding;
    public bool? Unrevivable;
    public List<(string ReagentId, FixedPoint2 Quantity, FixedPoint2 StomachQuantity)>? Chemicals; // Starlight - merged bloodstream and stomach reagents
    public List<(string TraitName, string TraitDesc, string ColorHex)>? DiagnosedConditions;

    public HealthAnalyzerUiState() {}

    public HealthAnalyzerUiState(
        NetEntity? targetEntity,
        float temperature,
        float bloodLevel,
        bool? canPrint,
        bool? scanMode,
        bool? bleeding,
        bool? unrevivable,
        List<(string ReagentId, FixedPoint2 Quantity, FixedPoint2 StomachQuantity)>? chemicals = null,
        List<(string TraitName, string TraitDesc, string ColorHex)>? diagnosedConditions = null)
    {
        TargetEntity = targetEntity;
        Temperature = temperature;
        BloodLevel = bloodLevel;
        CanPrint = canPrint;
        ScanMode = scanMode;
        Bleeding = bleeding;
        Unrevivable = unrevivable;
        Chemicals = chemicals; // Starlight
        DiagnosedConditions = diagnosedConditions;
    }
}
