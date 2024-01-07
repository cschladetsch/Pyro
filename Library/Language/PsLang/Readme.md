# Ps Lang

Provides a _bash-like_ syntax to *PowerShell*.

* !!                 # repeat last command
* !N                 # repeat command N, as index by get-history
* !N:M               # Mth token in Nth command   
* !N:A-B             # the Ath-Bth token in Nth command
* !^                 # first token in last command
* !$                 # last token in last command
* !$:-2              # the token 3rd before the last token in last command

## Extensions

* !p                 # the path of the first token in last command
* 

## Examples

```powershell
> echo spam
spam
> !!
spam
> echo foo bar
foo bar
> echo !:0
foo
> echo !:1
bar
> cd other/path
> echo !$
other/path
```

