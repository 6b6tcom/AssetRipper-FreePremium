# Xera Ripper

Xera Ripper is a custom build of AssetRipper used by our team for professional game reverse engineering.
AI DECOMP WIP 

## Features
- Extract and export assets from Unity games
- Web based interface for managing projects
- Customizable background color selector
- Experimental Script Content Level 4 that can use OpenAI or Claude API keys to help assemble scripts

## AI Decompiler

Script Content Level&nbsp;4 exposes the **AI Decompiler**, an experimental
pipeline for recovering IL2Cpp method bodies. Once the standard Il2Cpp
processing finishes building .NET assemblies, the decompiler invokes
`AIAssembler.AssembleScripts` to populate any empty methods with code generated
by an external AI service.


To try the AI Decompiler:

1. Open **Settings** and set **Script Content Level** to **Level&nbsp;4**.
2. Provide a valid API key through `OPENAI_API_KEY` or `CLAUDE_API_KEY`.
3. Run the export process as normal. After assembly generation, the AI service
   is contacted to fill in method bodies.

Internally, `IL2CppManager` triggers this step after calling
`BuildAssemblies` (see `ScriptContentLevel.cs` and `AIAssembler.cs`). Because
the code is heuristically generated it may not match the original source.
Treat the output as experimental and review it carefully before use.

```csharp
// Source/AssetRipper.Import/Configuration/ScriptContentLevel.cs
/// IL2Cpp methods are recovered without regard to safety. Currently uses AI assistance.
Level4,
```
