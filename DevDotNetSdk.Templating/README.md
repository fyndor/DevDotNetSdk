# DevDotNetSdk.Templating

## What is DevDotNetSdk?

The `DevDotNetSdk` library is a collection of types and tools that are useful in implementing solutions in .NET. This library was created, because I got tired of copying and pasting the same "base class library" into every new .NET project I started.

## What is DevDotNetSdk.Templating

`DevDotNetSdk.Templating` is a simple templating system that uses mardown files for the templates and C# objects for the template models. Documentation will come later as this library stablizes.  For now, the GitHub repositor for this library contains unit tests that show how it can be used.

## Release Notes

- v0.4.1
  Added ability to use 'this' with include, which used the current model as the sub template's input model, rather than a property name on the current model.
- v0.4
  Added 'include' directives. See tests for examples
- v0.3
  Added 'if' directives. See tests for examples.

**Note**: While this library is a 0.x version, expect that the shape of the library may change a bit.  Once I get to a 1.0, I will use the standard symantic version practices, and major versions will indicate there was a breaking change of some type.
