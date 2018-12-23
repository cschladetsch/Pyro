# Rho Documentation

Rho is an in-fix language that reads a lot like Python.

Rho translates to Pi which is then executed on the common Executor.

One side-effect of this is that you can inject *Pi* code into *Rho* source code.

## Rationale

*Pi* is a practically useful language system, but it's really just a stream of tokens.

*Rho* provides a structured language system that uses tabs for block indentation. Rho is easier to read, especially for longer programs.

```pi
1 2 + a #
```

```rho
a = 1 + 2
```

## Tests

See the [test suite](/Test/TestRho).

## Examples

See tests.