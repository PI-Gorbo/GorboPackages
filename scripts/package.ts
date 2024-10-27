import { $ } from "bun";
import { unlinkSync } from "node:fs";
import { readdir } from "node:fs/promises";
import { exit } from "node:process";

const packageIndexFileName = "packageIndex.json";

type VersionNumber = {
    major: number;
    minor: number;
    patch: number;
};
type Package = { name: string; latestVersion: VersionNumber };
type PackageIndex = Package[];

// MAIN PROGRAM
let index = await getPackageIndex();
const selectProjectOutput = index
    .map(
        (info, index) =>
            `[${index}]\t\t${info.name} - ${info.latestVersion.major}.${info.latestVersion.minor}.${info.latestVersion.patch}`
    )
    .join("\n");
const packageIndex = Number(
    prompt("Select project to publish: \n" + selectProjectOutput + "\n")
);
if (Number.isNaN(packageIndex)) exit(-1);

const incrementType = prompt("[Maj]or, [Min]or, or [Pat]ch?");
if (incrementType == null || !["Maj", "Min", "Pat"].includes(incrementType)) {
    console.write("Excited Maj, Min or Pat");
    exit(-1);
}
const relevantPackage = index[packageIndex];

console.write(
    `Publishing package ${relevantPackage.name}. Incrementing by ${incrementType}\n`
);
const updatedPackage = await publishPackage(relevantPackage, incrementType);

// HELPER FUNCTIONS
async function getPackageIndex(): Promise<PackageIndex> {
    let packageIndex: PackageIndex = [];

    // Initialize
    await $`git checkout main`
    const directories = await readdir(Bun.pathToFileURL(".")); // current directory
    const projectDirectories = directories
        .filter((dir) => dir.startsWith("GP.")) // Find all dirs starting with GP.
        .map((dir, index) => ({
            directory: dir.substring(3),
            index,
        })); // Grab the thing after GP.
    const packageIndexFile = Bun.file(packageIndexFileName);
    if (await packageIndexFile.exists()) {
        packageIndex = (await packageIndexFile.json()) as PackageIndex;
    }

    // Check that the package index is up-to-date
    for (let item of projectDirectories) {
        // Ensure that the item is in the package index.
        const itemMissing = !packageIndex.find((x) => item.directory == x.name);
        if (itemMissing) {
            packageIndex.push({
                name: item.directory,
                latestVersion: { major: 1, minor: 0, patch: 0 },
            });
        }
    }
    // Write package index
    const writer = packageIndexFile.writer();
    writer.write(JSON.stringify(packageIndex));
    writer.flush();
    writer.end();

    return packageIndex;
}

async function publishPackage(
    pack: Package,
    upgradeType: string
): Promise<Package> {
    // Increment version
    if (upgradeType == "Maj") {
        pack.latestVersion = {
            major: pack.latestVersion.major + 1,
            minor: 0,
            patch: 0,
        };
    } else if (upgradeType == "Min") {
        pack.latestVersion = {
            ...pack.latestVersion,
            minor: pack.latestVersion.minor + 1,
            patch: 0,
        };
    } else {
        pack.latestVersion = {
            ...pack.latestVersion,
            patch: pack.latestVersion.patch + 1,
        };
    }


    // Store the upgraded package index
    const file = await unlinkSync(packageIndexFileName); // Deletes the file.
    await Bun.write(
        packageIndexFileName,
        JSON.stringify(
            index.map((p) => (p.name == relevantPackage.name ? updatedPackage : p))
        )
    );

    // Push the updates.
    await $`git commit -am "(index) Updated package index - ${relevantPackage.name} incremented by ${incrementType} to ${pack.latestVersion.major}.${pack.latestVersion.minor}.${pack.latestVersion.patch}"`
    await $`git push`

    // Perform Upgrade
    const tagName = `${pack.name}.${pack.latestVersion.major}.${pack.latestVersion.minor}.${pack.latestVersion.patch}`;
    await $`git tag ${tagName}`;
    await $`git push origin ${tagName}`;

    return pack;
}