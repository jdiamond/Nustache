Nustache - Logic-less templates for .NET

For a list of implementations (other than .NET) and editor plugins, see
http://mustache.github.com/.

Usage:

- Add a reference to Nustache.Core.dll.
- Import the Nustache.Core namespace.
- Use one of the static, helper methods in the Render class.

    var html = Render.FileToString("foo.template", myData);

- Data can be object, IDictionary, or DataTable.
- If you need more control, use Render.Template.
- See the source and tests for more information.

nustache.exe:

- Command-line wrapper around Render.FileToFile.
- Reads JSON from file for data. Must wrap with { }.
- Parameters are templatePath, jsonPath, and outputPath.

    nustache.exe foo.template myData.json foo.html

- External templates are assumed to be in the same folder as the template
  mentioned in templatePath.
- Extension is also assumed to be the same as the template in templatePath.

Syntax:

- The same as Mustache with some extensions.
- Support for defining internal templates:

    {{<foo}}This is the foo template.{{/foo}}
    The above doesn't get rendered until it's included
    like this:
    {{>foo}}
    You can define templates inside sections. They override
    templates defined in outer sections which override
    external templates.