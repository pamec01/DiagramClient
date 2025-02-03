
using DiagramClient;


ReflectionTools reflectionTools = new();
var diagramSource = reflectionTools.AnalyzeAssembly(reflectionTools.GetType());

Console.WriteLine(diagramSource);

KrokiClient kroki = new ();

var diagram = await kroki.GenerateDiagramAsync(diagramSource);
Console.WriteLine("test");



DiagramPersistence persistence = new();
persistence.SaveSvgToFile(diagram, "../../../../diagram.svg");