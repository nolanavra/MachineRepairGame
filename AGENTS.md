# AGENTS.md

This file contains guidelines for any ChatGPT agent contributing to this repository.

## Scope
These instructions apply to the entire repository.

## Coding and style
- Follow existing C# patterns in the repository; prefer clear, self-documenting names and avoid unnecessary abstractions.
- Keep changes minimal and focused on the requested task. Avoid refactors unrelated to the ask.
- Never wrap `using` directives in try/catch blocks.

## Documentation and comments
- Add concise comments only when behavior is non-obvious.
- Update or add summary comments (`///`) for public methods when behavior changes.

## Testing
- Run available tests relevant to the change. If no automated tests exist, mention manual verification steps or note that tests were not run.

## Pull request / Summary message
- Summaries should list key changes with file references.
- Testing section must enumerate commands run, marking successes and any limitations.
- If no tests were run, explicitly state that in the Testing section.

## Files and formatting
- Preserve existing line endings and indentation (tabs vs. spaces) within edited files.
- Ensure new files end with a trailing newline.
