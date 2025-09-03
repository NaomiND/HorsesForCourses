### Doc Preview

If you would like to know what a particular file would look like in the doc, just put this:
```csharp
Explain.OnlyThis<NameOfClassYouWantToSee>("temp.md");
```
somewhere in a unit test and run it.

`temp.md` will be created in the solution's root containing the generated documentation for the specified class.Chang

## Interne links in je repository

Je kunt een interne link maken naar een ander bestand in je repository door de relatieve padnaam te gebruiken tussen rechte haken en haakjes: 

```markdown
[Linktekst](pad/naar/bestand.md)
```

Voorbeeld:  
[Lees meer over invarianten](../1.TheStables/readme.md)
Of door naar je bestandsmap te gaan, file te kopieëren en met SHIFT te plakken, plak je een link. 