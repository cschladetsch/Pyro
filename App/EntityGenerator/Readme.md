#  Network Entity Generator

Creates a network entity from a given network entity template.

The template is provided as a C# interface.

## Limitations

The input .cs file must be a valid C# interface.

The following are supported:

* Properties with get and or set accessors.
* Non-template methods.

## Usage

```
> generate -i <input file> [-op <output proxy file>] [-oa <output agent file>]
```

## Example

```
> generate -i MyInterface.cs -op MyProxy.cs -oa MyAgent.cs
```

## TODO

* Add support for multiple input files.
* Add support for template methods.
* Add a root folder option.


