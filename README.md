# HEAVI 2025 Setup Guide

HEAVI is a WPF-based weight game application that communicates with hardware devices via COM ports. This guide will help you set up the development environment and configure COM port simulation fofr testing.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Project Structure](#project-structure)
- [Building the Project](#building-the-project)
- [COM Port Configuration](#com-port-configuration)
- [Using VSPEmulator for COM Port Simulation](#using-vspemulator-for-com-port-simulation)
- [Configuration](#configuration)
- [Running the Application](#running-the-application)
- [Troubleshooting](#troubleshooting)

## Prerequisites

### Required Software

1. **Visual Studio 2017 or later** (or Visual Studio Code with C# extension)
   - Ensure .NET Framework 4.8 Developer Pack is installed
   - WPF development tools

2. **.NET Framework 4.8**
   - Download from [Microsoft](https://dotnet.microsoft.com/download/dotnet-framework/net48)

3. **VSPEmulator** (for COM port simulation)
   - Download from [Eterlogic Software](https://eterlogic.com/products.vspe.html)
   - Alternative: [Virtual Serial Port Driver](https://www.virtual-serial-port.org/)

### NuGet Packages

The project uses the following NuGet packages (automatically restored on build):
- AudioSwitcher.AudioApi (3.0.0)
- AudioSwitcher.AudioApi.CoreAudio (3.0.0.1)
- DotNetSlackClient (1.0.5934.39234)
- Microsoft.Expression.Drawing (3.0.0)
- Newtonsoft.Json (11.0.2)

## Project Structure

```
Heavi2025/
├── HEAVI/              # Main application project
├── AlertLED/           # LED alert control project
├── ScaleWithEffects/   # Scale display component
└── packages/          # NuGet packages (auto-restored)
```

## Building the Project

1. **Open the Solution**
   - Open `Heavi2025/HEAVI.sln` in Visual Studio

2. **Restore NuGet Packages**
   - Right-click the solution → "Restore NuGet Packages"
   - Or build the solution (packages will auto-restore)

3. **Build the Solution**
   - Press `Ctrl+Shift+B` or go to Build → Build Solution
   - Ensure all projects build successfully

4. **Set Startup Project**
   - Right-click `HEAVI` project → Set as Startup Project

## COM Port Configuration

HEAVI requires two COM ports:

- **Weight Scale Port** (default: COM3)
  - Baud Rate: 9600
  - Data Bits: 8
  - Stop Bits: 1
  - Parity: None
  - Handshake: None
  - RTS/DTR: Enabled
  - Expected format: `ST,GS,-00001.2kg\r\n`

- **Motor/Relay Control Port** (default: COM4)
  - Baud Rate: 9600
  - Data Bits: 8
  - Stop Bits: 1
  - Parity: None
  - Handshake: None
  - Sends polling command: "R"
  - Receives 4-character responses terminated with 0x03

### Changing COM Ports

COM ports are currently set in `MainWindow.xaml.cs`:

```csharp
rs232 = new Rs232Helper("COM3", "COM4");
```

## Using VSPEmulator for COM Port Simulation

VSPEmulator allows you to create virtual COM port pairs for testing without physical hardware.

### Installation

1. Download VSPEmulator from [Eterlogic Software](https://eterlogic.com/products.vspe.html)
2. Install the application (may require administrator privileges)
3. Restart your computer if prompted

### Setting Up Virtual COM Ports

1. **Launch VSPEmulator**
   - Open the VSPEmulator application

2. **Create a Virtual Port Pair**
   - Click "Add pair" or "Create pair"
   - VSPEmulator will create two connected virtual ports (e.g., COM5 ↔ COM6)
   - Note the port names assigned

3. **Configure for Weight Scale Simulation**
   - Use one port (e.g., COM5) as the weight scale
   - Configure HEAVI to use this port for weight data
   - You can use the SerialReader solutions to create a specific app for simulating the scale and assign it to the same port that the VSP Emulator provides. So HEAVI and the simulator uses the same port. Just like a null modem cable.
   - You can use a serial terminal (like PuTTY, Tera Term, or Serial Port Monitor) to send simulated weight data:
     ```
     ST,GS,-00150.5kg
     ```
     

4. **Configure for Motor/Relay Simulation**
   - Use the other port (e.g., COM6) as the motor control
   - Configure HEAVI to use this port for motor commands
   - Monitor this port to see commands sent by HEAVI (polling "R" command)
   - Send simulated responses: `1xxx` + 0x03 terminator

### Testing Setup

1. **Update COM Ports in Code**
   ```csharp
   // In MainWindow.xaml.cs, line 123
   rs232 = new Rs232Helper("COM5", "COM6"); // Your VSPEmulator ports
   ```

2. **Simulate Weight Data**
   - Open a serial terminal on the weight port (COM5)
   - Send weight data periodically:
     ```
     ST,GS,-00150.5kg
     ST,GS,-00200.0kg
     ST,GS,-00175.3kg
     ```

3. **Simulate Motor Responses**
   - Open a serial terminal on the motor port (COM6)
   - When you see "R" commands, respond with:
     ```
     1000 + 0x03  // Start game signal
     0100 + 0x03  // Beer at bottom
     0010 + 0x03  // Wager accept
     ```

### Alternative: Using Serial Port Monitor

For easier testing, use a serial port monitor tool:
- **Serial Port Monitor** (Eterlogic Software)
- **Serial Port Terminal** (Free)
- **PuTTY** (Free, basic terminal)

These tools allow you to:
- Monitor data sent/received on COM ports
- Send custom data to simulate device responses
- Log all communication for debugging

## Configuration

### Settings File

HEAVI creates a `Settings.txt` file in the application directory on first run. This file contains:

```
RoundsToWin#50
SecondsBeforeNextGame#180
SecondsToShowCraneLowering#30
SecondsToShowRoundWon#30
DefaultRoundLength#120
RoundsLeftToWin#50
KgsWon#0
KgsLost#0
RoundsWon#0
RoundsLost#0
LargestPossibleSoundPressure#60000
GamesInADay#50
GamesWon#0
```

You can edit this file to customize game parameters. The format is `Key#Value` (one per line).

### Application Configuration

The `App.config` file contains .NET Framework settings. No COM port configuration is stored here currently.

## Running the Application

1. **Build and Run**
   - Press `F5` or click "Start" in Visual Studio
   - The application will attempt to open COM3 and COM4 by default

2. **Debug Mode**
   - A debug panel is available (click top-right corner of the screen)
   - Use debug controls to:
     - Simulate weight values
     - Manually trigger game events
     - Test motor controls

3. **Keyboard Shortcuts** (in debug mode)
   - `L` - Simulate low weight
   - `H` - Simulate high weight
   - `P` - Simulate perfect weight (in zone)
   - `Q` - Lose round

## Troubleshooting

### COM Port Errors

**Error: "Error opening COM port"**
- Ensure the COM ports exist (check Device Manager)
- Verify ports are not in use by another application
- For VSPEmulator: Ensure virtual ports are created and connected

**Error: "Access to the port 'COMx' is denied"**
- Close any other applications using the port
- Run Visual Studio as Administrator
- Check if port is locked by another process

**Port Not Found**
- Verify COM port names in Device Manager
- For VSPEmulator: Check that virtual ports are active
- Try different port numbers if conflicts exist

### Build Errors

**NuGet Package Restore Failed**
- Right-click solution → "Restore NuGet Packages"
- Check internet connection
- Verify NuGet package source is accessible

**Missing .NET Framework 4.8**
- Install .NET Framework 4.8 Developer Pack
- Update Visual Studio to latest version

### Runtime Issues

**Application Crashes on Startup**
- Check COM port availability
- Review log files in application directory
- Ensure all required DLLs are in `bin` folder

**No Weight Data Received**
- Verify COM port configuration
- Check serial port settings match device
- Test with serial port monitor tool
- Ensure data format matches expected: `ST,GS,-00001.2kg\r\n`

**Motor/Relay Not Responding**
- Verify motor COM port is correct
- Check that polling command "R" is being sent
- Monitor port with serial terminal to see communication

### VSPEmulator Issues

**Virtual Ports Not Appearing**
- Restart VSPEmulator
- Run as Administrator
- Check Windows Device Manager for virtual ports
- Reinstall VSPEmulator if ports don't appear

**Ports Not Communicating**
- Ensure ports are paired correctly in VSPEmulator
- Verify both ports are in "Connected" state
- Try recreating the port pair

## Additional Resources

- **Serial Communication**: [Microsoft SerialPort Documentation](https://docs.microsoft.com/en-us/dotnet/api/system.io.ports.serialport)
- **VSPEmulator Help**: Check VSPEmulator documentation for advanced features
- **WPF Development**: [WPF Documentation](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)

## Notes

- The application logs events to a daily log file (format: `DD.MM.YYYY.log`)
- Slack integration is configured (see `SlackClient.cs` for webhook URL)
- Sound effects are located in `Sound/Rendered/` directory
- Graphics assets are in `GFX/` directory

---

For questions or issues, check the log files or contact the development team.
