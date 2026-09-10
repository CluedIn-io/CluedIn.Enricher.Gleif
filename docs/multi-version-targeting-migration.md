# Migrating a Connector/Enricher to Multi-Version Targeting

This document tracks the migration of `CluedIn.Enricher.Gleif` from a single-version build to the
multi-version targeting pattern. Modeled on the prior migrations of `CluedIn.Connector.Dataverse.V2`,
`CluedIn.Enricher.GoogleMaps`, `CluedIn.Connector.AzureEventHubs`, and
`CluedIn.Crawling.MasterDataServices`.

---

## Overview

The goal is to produce separate NuGet packages per CluedIn version from a single branch, using the
shared `crawler.build.jobs.yml` pipeline template. Each package targets the correct .NET TFM for
that CluedIn generation, auto-detected by the template.

| CluedIn version | .NET TFM | Package suffix |
|---|---|---|
| 4.7.0 | net6.0 | `.470` |
| 4.8.0 | net6.0 | `.480` |
| 5.0.0-beta.* | net10.0 | `.500` |

**Verified independently** (not copied from an older doc): this repo's own `NuGet.config` feeds
resolve `5.0.0-*` to `5.0.0-beta.575` (beta is the current CluedIn 5.0 prerelease channel). 4.6.0
excluded, matching the MasterDataServices/GoogleMaps precedent (no known 4.6-only dependency in this
enricher's small API surface).

Branch: `feature/multi-version-targeting` (off `origin/develop`).

**Pool changed from `windows-latest` to `ubuntu-22.04`** — the original single-version pipeline ran
on Windows; every other migrated repo's multi-version pipeline runs on `ubuntu-22.04`, and the
shared template/detect-tfm script is written and tested against Linux agents. Switched to match.

---

## Step 1 — Pipeline template (`azure-pipelines.yml`)

Status: **Done**

Replaced the single-version `crawler.build.yml` steps-template with the multi-version
`crawler.build.jobs.yml` jobs-template, added `probeCluedInVersion`, switched the pool to
`ubuntu-22.04` (see Overview).

---

## Step 2 — `Directory.Build.props`

Status: **Done**

Honours `CluedInMultiVersionTargetFramework` (net10.0 local fallback); derives
`CLUEDIN_V47`/`V48`/`V50` `DefineConstants`; pinned `LangVersion` to `13.0`.

---

## Step 3 — `Packages.props`

Status: **Done**

- `_CluedIn` guarded so the pipeline's per-leg override wins.
- Test package selection (previously an unconditional xunit v3/AutoFixture.Xunit3/
  Microsoft.NET.Test.Sdk 18.3.0 block) split into `$(DefineConstants.Contains('CLUEDIN_V50'))` /
  `!...` `ItemGroup`s: xunit v3 + AutoFixture.Xunit3 + Microsoft.NET.Test.Sdk 18.3.0 for CluedIn
  5.0+, xunit v2 (2.9.3) + AutoFixture.Xunit2 (4.18.0) + Microsoft.NET.Test.Sdk 17.12.0 for
  4.7.0/4.8.0.
- `CluedIn.Testing.Base` was hardcoded at `5.0.0-*` (net10.0-only). Switched to the
  version-suffixed package ID each CluedIn leg actually publishes
  (`CluedIn.Testing.Base.$(_CluedInVersionSuffix)`, computed as the dotless
  `Major.Minor.Patch` of `_CluedIn` — `.470`/`.480`/`.500`), per the GoogleMaps doc's finding.
  Verified all three suffixed packages exist on the `develop` feed before wiring this up.

---

## Step 4 — `NuGet.config` casing

Status: **Done**

`git mv`'d `Nuget.config` → `NuGet.config` (two-step rename). No feed changes needed — confirmed via
real restore that `CluedIn.Core` and `CluedIn.ExternalSearch` both resolve at `4.7.0` against this
repo's existing feeds (no `public` feed present here, unlike AzureEventHubs/Dataverse.V2, and none
was needed).

---

## Step 5 — Test projects

Status: **Done**

- `test/Directory.Build.props` — removed the unconditional xunit v3/AutoFixture.Xunit3 package
  refs (kept only `coverlet.msbuild`/`Moq`/`Shouldly`, which don't vary by target).
- `test/integration/ExternalSearch.Gleif.Integration.Tests.csproj` — added the conditional
  xunit v2/v3 + `CluedIn.Testing.Base.$(_CluedInVersionSuffix)` `ItemGroup`s directly (this is the
  only test project in the repo).
- No `GlobalUsings.cs` needed — the one test file (`GleifTests.cs`) doesn't reference
  `AutoFixture.Xunit2`/`.Xunit3` or `ITestOutputHelper` directly, so there's no namespace that
  differs between the two xunit generations here.

---

## Step 6 — API compatibility audit across 4.7.0 / 4.8.0 / 5.0.0-beta.*

Status: **Done**

Built and ran `dotnet test` for real against all three legs (not just reasoned about).

**Finding: same RestSharp 106-vs-114 break GoogleMaps found**, in
`src/ExternalSearch.Providers.Gleif/GleifExternalSearchProvider.cs`:
- `Method.Get` (PascalCase, RestSharp 114/CluedIn 5.0) vs `Method.GET` (uppercase, RestSharp
  106/CluedIn 4.7-4.8) — 2 call sites.
- `ConstructVerifyConnectionResponse`'s parameter was explicitly typed `RestResponse response`
  (RestSharp 114 shape); under RestSharp 106, `client.ExecuteAsync(request).Result` returns
  `IRestResponse`, not assignable to `RestResponse` — 1 call site (the parameter declaration).

All three fixed with `#if CLUEDIN_V50` guards. Verified `src` builds clean (0 errors) on all three
legs, and the integration test project builds and `dotnet test` **passes** (1 passed, 2 skipped —
pre-existing `[Theory(Skip = ...)]` tests unrelated to this migration) on the 4.7.0/net6.0 leg.

---

## Step 7 — Reset the semantic version (`GitVersion.yml`)

Status: **Done**

```yaml
next-version: 1.0
ignore:
  sha: []
  commits-before: 2026-03-20T00:00:00
```

Highest pre-existing tag is `4.6.1`/`v4.6.1` at `2026-03-18T15:05:58Z`.

**Real bug found and fixed, not in any prior doc:** an initial cutoff of `2026-03-19T00:00:00`
(the naive "day after the tag's UTC date", the same pattern the other docs use) did **not** work —
`dotnet-gitversion` still resolved `MajorMinorPatch` to `4.7.0` (incrementing off the old `4.6.1`
tag), not `1.0.0`. Root cause: this machine is UTC+10, and `GitVersion.Tool 5.9.0` appears to parse
the bare `commits-before` timestamp as **local machine time**, not UTC. `2026-03-19T00:00:00`
local (UTC+10) is `2026-03-18T14:00:00Z` — which is actually *before* the tag's real UTC timestamp
(`15:05:58Z`), so the tag was never excluded. Padding to a full 2 days past the tag
(`2026-03-20T00:00:00`) fixed it — verified with a cleared `.git/gitversion_cache` and the
pipeline's actual pinned `GitVersion.Tool 5.9.0`:

```
MajorMinorPatch: 1.0.0
SemVer: 1.0.0-multi-version-targeting.88
```

**Lesson for migrating the remaining repos:** don't pad `commits-before` by just "the next
calendar day" — pad by at least a full 2 days past the highest tag's UTC timestamp, and verify the
resolved `MajorMinorPatch` is actually `1.0.0` (not just that the command didn't error) before
trusting it.

---

## Checklist

- [x] `azure-pipelines.yml` — switched to `crawler.build.jobs.yml` with `multiVersionCluedInTargets` (4.7.0, 4.8.0, 5.0.0-beta.*); pool switched from `windows-latest` to `ubuntu-22.04`
- [x] `Directory.Build.props` — honours `CluedInMultiVersionTargetFramework`; `DefineConstants` derived; `LangVersion` pinned to 13.0
- [x] `Packages.props` — `_CluedIn` guarded; test package selection split by `CLUEDIN_V50`; `CluedIn.Testing.Base` switched to the version-suffixed package ID
- [x] `NuGet.config` — renamed from `Nuget.config`; feeds confirmed sufficient as-is
- [x] Test project — conditional xunit v2/v3 selection added directly to the one integration test csproj; no `GlobalUsings.cs` needed
- [x] Source — `#if CLUEDIN_V50` guards added for the RestSharp 106↔114 break (3 call sites in `GleifExternalSearchProvider.cs`); `src` builds clean on all three legs
- [x] Integration tests — build and `dotnet test` pass on the 4.7.0/net6.0 leg
- [x] `GitVersion.yml` — `next-version: 1.0`; `ignore.commits-before: 2026-03-20T00:00:00` (padded 2 days past the tag after an initial 1-day pad failed — see Step 7); verified with the pipeline's actual pinned GitVersion.Tool 5.9.0
- [x] Pushed branch and confirmed the Azure DevOps pipeline is green end-to-end on the first push — PR #38, build 151864: all three legs + `Multi-version: publish` passed
