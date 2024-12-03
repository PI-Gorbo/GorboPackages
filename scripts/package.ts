import { $ } from "bun";
import { readFile, writeFile } from "node:fs/promises";
import { readdir } from "node:fs/promises";
import { exit } from "node:process";

// Get the current git branch. Ensure that we are on the 'dev' branch.
const currentBranch = await getCurrentGitBranch()
if (currentBranch !== 'dev') {
    console.error("This script should only be run on the dev branch.")
    exit(1);
}


// MAIN PROGRAM
const incrementType = prompt("[Maj]or, [Min]or, or [Pat]ch?");
if (incrementType == null || !["Maj", "Min", "Pat"].includes(incrementType)) {
    console.error("Expected Maj, Min, or Pat.");
    exit(1);
}

console.log(`Incrementing by ${incrementType}\n`);

await publishPackage(incrementType as 'Maj' | 'Min' | 'Pat');

// HELPER FUNCTIONS
async function publishPackage(upgradeType : 'Maj' | 'Min' | 'Pat') {
    const packageJsonPath = `./package.json`;
    const packageJson = JSON.parse(await readFile(packageJsonPath, "utf-8"));

    // Parse version
    let [major, minor, patch] = packageJson.version.split(".").map(Number);

    // Increment version
    if (upgradeType === "Maj") {
        major++;
        minor = 0;
        patch = 0;
    } else if (upgradeType === "Min") {
        minor++;
        patch = 0;
    } else {
        patch++;
    }

    const newVersion = `${major}.${minor}.${patch}`;
    packageJson.version = newVersion;

    // Save updated package.json
    await writeFile(packageJsonPath, JSON.stringify(packageJson, null, 2), "utf-8");

    console.log(`Version updated to ${newVersion}`);

    // Commit and tag
    await $`git add ${packageJsonPath}`;
    await $`git commit -m "script: bump version to ${newVersion}"`;
    const tagName = `${newVersion}`;
    await $`git tag ${tagName}`;
    await $`git push origin ${tagName}`;
    console.log(`Published and tagged ${tagName}`);
    
    console.log("Now creating a PR into main.")
}

async function getCurrentGitBranch() {
    const branchOutput = await $`git rev-parse --abbrev-ref HEAD`.text();
    return branchOutput.trim()
}