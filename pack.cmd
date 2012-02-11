set opts=-Prop Configuration=Release -Build -OutputDirectory .

nuget pack Nustache.Core\Nustache.Core.csproj %opts%
nuget pack Nustache.Mvc3\Nustache.Mvc3.csproj %opts%

