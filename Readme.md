# Moogle!

**Moogle!** is a *completely original* application designed to intelligently search for text within a set of documents. It is a web application developed using **.NET Core 6.0**, specifically utilizing **Blazor** as the web framework for the graphical interface, and written in the **C#** language. The application is divided into two fundamental components:

1. **MoogleServer**: This is a web server responsible for rendering the graphical interface and serving the search results.
2. **MoogleEngine**: A class library where, well, the logic of the search algorithm is almost implemented. 😉⁴

## Running the Project

The first thing you'll need to do to work on this project is to install **.NET Core 6.0** (which we assume isn't a problem at this point, right?). Then, navigate to the project folder and execute the following command in the Linux terminal:

```bash
make dev
```

If you're on Windows, you should be able to do the same from the Windows Subsystem for Linux (WSL) terminal. If you don't have WSL or can't install it, you should seriously consider installing Linux. But if you insist on developing the project on Windows, the *ultimate* command to run the application (from the project's root folder) is:

```bash
dotnet watch run --project MoogleServer
```
# Report
[Report_Spanish](./Informe.md)

[Report_English](./Report.md)
