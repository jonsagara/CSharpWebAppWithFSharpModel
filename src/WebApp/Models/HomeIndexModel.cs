using FSharpClassLibrary.Models;

namespace WebApp.Models;

public class HomeIndexModel
{
    public List<TermReportCards> TermReportCards { get; private set; } = new();
}
