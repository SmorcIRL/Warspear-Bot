namespace WarspearBot.Models;

public class TemplateMatchInfo
{
    public string TemplateName { get; set; }

    public float Threshold { get; set; }

    public float MatchingRate { get; set; }

    public (int X, int Y) Location { get; set; }

    public (int W, int H) TemplateSize { get; set; }
}