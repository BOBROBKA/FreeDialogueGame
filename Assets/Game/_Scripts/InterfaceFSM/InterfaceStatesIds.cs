public class InterfaceStatesIds
{
    private enum ids
        {
            Inspection,
            Dialogue
        }
    public static string InspectionState() => ids.Inspection.ToString();
    public static string DialogueState() => ids.Dialogue.ToString();
}