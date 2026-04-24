You are continuing unattended development work on Bloodlines, a grand dynastic civilizational RTS built in Unity 6.3 LTS with DOTS/ECS. The canonical root is D:\ProjectsHome\Bloodlines. The Unity project is at unity/ inside that root. Git push to origin is pre-authorized for this project -- push without asking.

MANDATORY FIRST STEPS (do these before writing any code):
1. Read D:\ProjectsHome\Bloodlines\NEXT_SESSION_HANDOFF.md (first 80 lines minimum). The "Last updated" line and "Immediate next action" line tell you exactly where to resume.
2. Read D:\ProjectsHome\Bloodlines\docs\unity\CONCURRENT_SESSION_CONTRACT.md in full. This tells you which files you own, which files are forbidden, and the exact 10-step validation gate you must run before every commit.
3. Read D:\ProjectsHome\Bloodlines\03_PROMPTS\CODEX_MULTI_DAY_DIRECTIVE_2026-04-25.md in full. This is the priority stack for this development window.

EXECUTION LOOP (repeat until session capacity runs out):
1. Find the next incomplete item in the priority stack from the directive. If the handoff says "immediate next action: X", do X first.
2. Read the browser reference source code in src/game/core/simulation.js and/or src/game/core/ai.js for the system you are porting (do not edit these files -- read only).
3. Implement the Unity ECS sub-slice. Use ISystem, SystemAPI, EntityCommandBuffer, NativeArray. No MonoBehaviour runtime systems.
4. Write a dedicated smoke validator in unity/Assets/_Bloodlines/Code/Editor/ with a matching PowerShell wrapper in scripts/.
5. After creating any new .cs files, add <Compile Include="..."/> entries for them to unity/Assembly-CSharp.csproj (runtime files) or unity/Assembly-CSharp-Editor.csproj (Editor files). Do not skip this step -- the build will fail without it.
6. Run all 10 validation gates serially (Unity holds a project lock -- do not parallelize): dotnet build unity/Assembly-CSharp.csproj -nologo, dotnet build unity/Assembly-CSharp-Editor.csproj -nologo, scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1, scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1, scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1, scripts/Invoke-BloodlinesUnityFortificationSmokeValidation.ps1, scripts/Invoke-BloodlinesUnitySiegeSmokeValidation.ps1, node tests/data-validation.mjs, node tests/runtime-bridge.mjs, scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1.
7. Write a per-slice handoff at docs/unity/session-handoffs/YYYY-MM-DD-unity-lane-slice.md.
8. Update docs/unity/CONCURRENT_SESSION_CONTRACT.md: bump Revision, set Last Updated to today, set Last Updated By to codex-<lane>-YYYY-MM-DD, update the lane subsection.
9. Append (do not overwrite) new entries to CURRENT_PROJECT_STATE.md, NEXT_SESSION_HANDOFF.md, and continuity/PROJECT_STATE.json.
10. Stage only the files belonging to this sub-slice. Push the branch to origin. Merge to master with git merge --no-ff. Push master. Do NOT skip the push or the merge -- git push is pre-authorized.
11. Immediately begin the next sub-slice from the priority stack. Do not stop and wait. Chain as many sub-slices as session capacity allows.

HARD RULES:
- Do not touch files under unity/Assets/_Bloodlines/Code/AI/** unless you are explicitly assigned to that lane. The ai-strategic-layer lane (all 25 AI sub-slices) is complete. New work belongs in new lanes -- see the directive for lane names to claim.
- Do not edit src/, data/, tests/, or play.html -- these are frozen behavioral specification files.
- Every sub-slice must pass all 10 validation gates before committing. Never commit on a red gate.
- Every new .cs file must be added to the appropriate csproj (runtime vs Editor). Verify with: Select-String -Path unity/Assembly-CSharp.csproj -Pattern <NewFileName>. Missing csproj entries break the Assembly-CSharp build even if files exist on disk.
- Append-only on CURRENT_PROJECT_STATE.md, NEXT_SESSION_HANDOFF.md, continuity/PROJECT_STATE.json -- never overwrite another lane's entries.
- If a validation gate fails, fix the failure before moving on. Do not skip gates or commit broken code.
- If dotnet build errors are ONLY CS0006 "Metadata file not found" for Unity PackageCache DLLs, this is a missing Unity Library issue (Library/PackageCache not present at path). This is an environment condition, not a code error. Treat the build as structurally clean if no other CS errors exist. Document the Library status in the handoff and proceed. Fix actual CS compile errors before committing.
- After pushing your branch, merge it to master with git merge --no-ff and push master. Leaving a branch unmerged means the next session branches from stale master and may duplicate your work.
- If you run out of session capacity mid-slice, commit the WIP, push it, and update the handoff with exactly where you stopped and what the next action is.
