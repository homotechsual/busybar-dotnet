# website

The BusyBar .NET docs site, built with [Docusaurus](https://docusaurus.io/). Deployed to
[busybar-dotnet.homotechsual.dev](https://busybar-dotnet.homotechsual.dev) via Cloudflare Pages
(see `../.github/workflows/deploy-docs.yml`).

Uses [Yarn](https://yarnpkg.com/) (pinned via Corepack; see `packageManager` in `package.json`),
not npm.

## Install

```bash
corepack enable
yarn install
```

## Local development

```bash
yarn start
```

Starts a local dev server and opens a browser window. Most changes reload live.

## Build

The API Reference section (`docs/api/`) is generated from the `BusyBar` library's XML doc
comments; see `../scripts/generate-api-docs.ps1`; and is gitignored, not committed. Regenerate
it before building if the library's public API or doc comments changed:

```bash
yarn generate-api-docs   # builds the library in Release, then runs xmldoc2md
yarn build               # builds the Docusaurus site
# or, in one step:
yarn build:full
```

Static output goes to `build/`.
