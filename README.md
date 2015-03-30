#Nustache - Logic-less templates for .NET

![Nustache.Core](https://img.shields.io/appveyor/ci/romanx/nustache/master.svg)
![Nustache.Core](https://img.shields.io/github/stars/jdiamond/Nustache.svg)

Nustache.Core: ![Nustache.Core](https://img.shields.io/nuget/dt/Nustache.svg)
Nustache.Mvc3: ![Nustache.Mvc3](https://img.shields.io/nuget/dt/Nustache.Mvc3.svg)
Nustache.Compliation: ![Nustache.Compliation](https://img.shields.io/nuget/dt/Nustache.Compilation.svg)

### NEW NOTICE - 2015-03-29:

Hello, as you may have noticed this repository has been silent for awhile. I worked on it awhile ago 
when I had a problem but didn't contribute any more. 

I'm planning to commit some time to looking at outstanding pull-requests and issues and try get the 
project moving again. With that in mind feel free to open issues and contribute pull requests.
- *Romanx*

### NOTICE (JDiamond):

I haven't used Nustache in a while and don't have enough bandwidth to responsibly maintain it.
If you depend on Nustache and want committ access, please contat me!

For a list of implementations (other than .NET) and editor plugins, see
http://mustache.github.com/.

## Installation:

- Pull from GitHub or download the repository and build it.
- Or, install via NuGet (search for Nustache).
- If you're using MVC, you'll want to build/install the Nustache.Mvc3 project,
  too.

## Usage:

### For non-MVC projects:

- Add a reference to Nustache.Core.dll (done for you if you used NuGet).
- Import the Nustache.Core namespace.
- Use one of the static, helper methods on the Render class.

```C#
var html = Render.FileToString("foo.template", myData);
```

- Data can be object, IDictionary, or DataTable.
- If you need more control, use Render.Template.
- See the source and tests for more information.
- For compiled templates:

```C#
var template = new Template();
template.Load(new StringReader(templateText));
var compiled = template.Compile<Foo>(null);

var html = compiled(fooInstance);
```

### For MVC projects:

- Add a reference to Nustache.Mvc3.dll (done for you if you used NuGet).
- Add NustacheViewEngine to the global list of view engines.
- See Global.asax.cs in the Nustache.Mvc3.Example project for an example.

### nustache.exe:

- Command-line wrapper around Render.FileToFile.
- Parameters are templatePath, dataPath, and outputPath.
- Reads JSON or XML from dataPath for data.
  - If extension is .js or .json, assumes JSON. Must wrap with { }.
  - If extension is .xml, assumes XML. Initial context is the document element.

    ```
	nustache.exe foo.template myData.json foo.html
	```

- External templates are assumed to be in the same folder as the template
  mentioned in templatePath.
- Extension is also assumed to be the same as the template in templatePath.

## Syntax:

- The same as Mustache with some extensions.
- Support for defining internal templates:

```
{{<foo}}This is the foo template.{{/foo}}
The above doesn't get rendered until it's included
like this:
{{>foo}}
```

You can define templates inside sections. They override
templates defined in outer sections which override
external templates.

## Development:

- Build with VS2012 or MSBuild.
- To run the tests that use Mustache specs, run this command from your
  Nustache clone:

```
git submodule update --init
```
