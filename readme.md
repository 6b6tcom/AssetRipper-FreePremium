# Xera Ripper

Xera Ripper is a custom build of AssetRipper used by our team for professional game reverse engineering.

## Features
- Extract and export assets from Unity games
- Web based interface for managing projects
- Customizable background color selector
- Experimental Script Content Level 4 that can use OpenAI or Claude API keys to help assemble scripts

## Usage
1. Build the solution with `dotnet build`.
2. Run the GUI from `AssetRipper.GUI.Free`.
3. Configure settings under **Settings**.
   - Choose a background color.
   - Provide your API key in the environment variable `OPENAI_API_KEY` or `CLAUDE_API_KEY` for script assembly.

This repository is for educational and professional research purposes.
