public static class ClassificationActions
{
    public static string Price() => "price";
    public static string Bargain() => "bargain";
    public static string Leave() => "leave";
    public static string Insult() => "insult";
    public static string Threat() => "threat";
    public static string Story() => "story";
    public static string Buy() => "buy";
    public static string Agree() => "agree";
    public static string Nonsens() => "nonsens";

    public static string[] GetValidActions() => new string[]
    {
        Price(),
        Bargain(),
        Leave(),
        Insult(),
        Threat(),
        Story(),
        Buy(),
        Agree(),
        Nonsens()
    };
}