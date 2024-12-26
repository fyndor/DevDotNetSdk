# DevDotNetSdk

The `DevDotNetSdk` library is a collection of types and tools that are useful in implementing solutions in .NET. This library was created, because I got tired of copying and pasting the same "base class library" into every new .NET project I started.

**Note**: While this library is a 0.x version, expect that the shape of the library may change a bit.  Once I get to a 1.0, I will use the standard symantic version practices, and major versions will indicate there was a breaking change of some type. I use these types in all my projects, so I do intend to maintain this library.

## `Result<TData, TError>`

This type allows you to return errors as values rather than throwing exceptions.  When calling methods that can throw, you would catch the exceptions and package them in some way to return them as value rather than have the exception bubble up the stack.  It has an `IsError` and `IsOk` property that tells you whether it contains an error or value.  To get the error or value, call `Unwrap` and `UnwrapError`.  Calling unwrap on a value when the result contains an error, and visa versa, will end in the call throwing an exception.

## `Result<TError>`

This type acts the same as the above type, but it is to be used with methods that return no value, but can result in an error.

## Release Notes

- v0.4
  Changed `Option<T> signature`
- v0.3
  Added `Option<T>`
- v0.2
  Added `Variant<T1, T2>` and `Variant<T1, T2, T3>`