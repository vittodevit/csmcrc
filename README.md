# csmcrc (CSharp MineCraft Rcon Client)
**Remotely control any Minecraft server with RCON enabled!**  

**How to enable RCON:**
In server.properties of your minecraft server set those values
```
enable-rcon=true
rcon.password=<your password>
rcon.port=<1-65535>
```

**How to install (nuget)**  
Click on the package here on the right, follow the instruction to import it in your project

**How to install (from source)**  
If you want to build this project from source you should already know how to do it.

**HOW TO USE:**  
The library is really easy to use if you just use the main methods here.   
Use init to connect, send for sending commands and disconnect to terminate the connection.  
Check out the example gist here ->  [https://gist.github.com/mrBackSlash-it/80dff8cbb375d621746e33821fa513fa](https://gist.github.com/mrBackSlash-it/80dff8cbb375d621746e33821fa513fa)

**Main methods:**  
```init(string host, int port, string password)``` Connects to the server  
```send(string payload)``` Sends a command   
```disconnect()``` closes tcp socket and disconnects from the server  
```isConencted()``` returns connection state (boolean)
```getAddress() and getPort()``` they return the address and the port of that instance
