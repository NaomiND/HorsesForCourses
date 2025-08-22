### Doc Preview

If you would like to know what a particular file would look like in the doc, just put this:
```csharp
Explain.OnlyThis<NameOfClassYouWantToSee>("temp.md");
```
somewhere in a unit test and run it.

`temp.md` will be created in the solution's root containing the generated documentation for the specified class.Chang