---
name: unity-process-reset
description: Invoke when the Unity batch-mode wrapper exits unexpectedly, fails to start, or produces a truncated log. Kills all Unity-related processes, removes the lockfile, and optionally clears build caches. Trigger phrases: "Unity wrapper failed", "exits 1 truncated", "exits -1 timeout", "lockfile", "could not find BloodlinesMapBootstrapAuthoring", "Unity won't start", "wrapper hangs", "batch mode stuck".
---

# Unity Process Reset Skill (Bloodlines)

Unity batch-mode is fragile when another Unity instance is running or has a stale lockfile. This skill recovers the environment before the next wrapper invocation.

## Symptom map

| Symptom | Most likely cause |
|---|---|
| Wrapper exits code 1 immediately; log stops at `COMMAND LINE ARGUMENTS` | Another Unity instance has the project lock |
| Wrapper exits code -1 after full timeout; no validation output | Unity launched but hung; possibly a compilation error loop |
| Log reports `could not find BloodlinesMapBootstrapAuthoring` | Stale `Library/ScriptAssemblies` from a partial compile |
| Wrapper exits 0 but smoke output is missing fields | HMR scene was dirty; a prior run wrote a partial output file |

## Step 1: Kill all Unity-related processes

```powershell
# Run in PowerShell (Bypass ExecutionPolicy if needed)
$processNames = @(
    "Unity",
    "bee_backend",
    "Unity.ILPP.Runner",
    "Unity.ILPP.Trigger",
    "UnityShaderCompiler",
    "UnityPackageManager",
    "UnityAutoQuitter"
)
foreach ($name in $processNames) {
    $procs = Get-Process -Name $name -ErrorAction SilentlyContinue
    if ($procs) {
        $procs | Stop-Process -Force
        Write-Host "Killed $name ($($procs.Count) instance(s))"
    }
}
Write-Host "Process sweep complete."
```

Or from bash:
```bash
taskkill /F /IM Unity.exe /T 2>/dev/null || true
taskkill /F /IM bee_backend.exe /T 2>/dev/null || true
taskkill /F /IM Unity.ILPP.Runner.exe /T 2>/dev/null || true
taskkill /F /IM Unity.ILPP.Trigger.exe /T 2>/dev/null || true
taskkill /F /IM UnityShaderCompiler.exe /T 2>/dev/null || true
taskkill /F /IM UnityPackageManager.exe /T 2>/dev/null || true
taskkill /F /IM UnityAutoQuitter.exe /T 2>/dev/null || true
echo "Process sweep complete."
```

Wait 3 seconds after killing before proceeding.

## Step 2: Remove the lockfile

```bash
rm -f unity/Temp/UnityLockfile
echo "Lockfile removed."
```

If the file is locked by a process that did not die in step 1, repeat step 1. On Windows, a locked file means the process is still running.

## Step 3 (conditional): Clear stale compile output

Run this step only if:
- The symptom is `could not find BloodlinesMapBootstrapAuthoring`, or
- Wrapper exited -1 (full timeout) twice in a row, or
- The `Library/ScriptAssemblies` directory timestamp is newer than the most recent `.cs` file.

```bash
rm -rf unity/Library/ScriptAssemblies
echo "Cleared ScriptAssemblies."
```

Do NOT clear `unity/Library/Bee` as a first resort. Only clear it if step 3 above did not resolve the issue after a retry cycle:

```bash
# Last resort only:
rm -rf unity/Library/Bee
echo "Cleared Bee build cache."
```

Clearing `Library/Bee` causes a full incremental recompile which adds 5-15 minutes to the next wrapper run. Reserve it for persistent failure.

## Step 4: Verify clean state

```bash
ls unity/Temp/UnityLockfile 2>/dev/null && echo "LOCKFILE STILL PRESENT - step 2 failed" || echo "Lockfile absent. OK."
pgrep -a Unity 2>/dev/null || tasklist 2>/dev/null | grep -i unity || echo "No Unity processes. OK."
```

## Step 5: Retry the wrapper

Once the environment is clean, retry the validation wrapper that failed. Do not change any code between the reset and the retry unless the failure was a compilation error (not a lockfile issue).

If the wrapper fails again after a clean reset:
1. Read the full log output carefully. Look for `error CS` lines, missing GUID references, or `NullReferenceException` in the log.
2. If it is a compilation error: the problem is in the C# code, not the process state. Do not loop on resets.
3. If it is a lockfile error again: something is re-acquiring the lock. Check for background Unity processes started by IDE integrations (Rider, VS, VS Code with the Unity extension). Kill those too.

## What this skill does NOT do

- Does not fix C# compilation errors (those require reading the build output and editing code).
- Does not reset git state.
- Does not repair corrupted `Library/` beyond the targeted clears above.
- Does not resolve Unity license or activation failures.
