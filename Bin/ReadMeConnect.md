# Connecting Peers
There's no real idea of a 'Client' or a 'Server' - each 'Peer' is potentially either or both.

## First Node
Pyro.Console 9999
192.168.3.146:9999 pi> 

## Second Node
Pyro.Console 8888
192.168.3.146:8888 > "192.168.3.146" 9999 'Connect peer .@ &
`Client: connected to 192.168.3.146:9999 using 192.168.3.146:56143`
192.168.3.146:8888 Pi> 2 'EnterRemoteAt peer .@ &
`Remoting into 192.168.3.146:9999`
192.168.3.146:9999 Pi> "hello" ", world" +
192.168.3.146:9999 Pi>
`0: "hello, world"`
`1: 3`

Note that the shell changes from *192.168.3.146:8888* to *192.168.3.146:9999* after we remote into the other node.
This means that all processing will be executed on that machine, and it will send back its stack after each command.
