import {themes as prismThemes} from 'prism-react-renderer';
import type {Config} from '@docusaurus/types';
import type * as Preset from '@docusaurus/preset-classic';
import {DOCUSAURUS_VERSION} from '@docusaurus/utils';

// This runs in Node.js - Don't use client-side code here (browser APIs, JSX...)

const config: Config = {
  title: 'BusyBar .NET',
  tagline: 'A typed .NET client for the BUSY Bar HTTP API',
  // SVG favicon derived from the navbar logo mark (with its backdrop kept, unlike the navbar
  // version; here it sits on the browser tab, not on our identically-colored navbar, so there's
  // no camouflage risk). Supported by all current major browsers as a <link rel="icon"> target.
  favicon: 'img/favicon.svg',

  // Future flags, see https://docusaurus.io/docs/api/docusaurus-config#future
  future: {
    v4: true, // Improve compatibility with the upcoming Docusaurus v4
  },

  // Set the production url of your site here
  url: 'https://busybar-dotnet.homotechsual.dev',
  baseUrl: '/',

  // GitHub pages deployment config.
  organizationName: 'homotechsual',
  projectName: 'busybar-dotnet',

  onBrokenLinks: 'throw',
  // generate-api-docs.ps1 injects a real `<a id="...">` anchor into each enum's field-table row so
  // <see cref="Enum.Member"/> cross-references actually resolve; verified in a real browser:
  // document.getElementById('key') exists and #key correctly navigates there. Docusaurus's own
  // broken-anchor checker still flags these as broken because it validates against its own
  // heading registry, not the rendered DOM, so it never sees hand-injected HTML anchors it didn't
  // generate itself. This is a checker false-positive, not an unresolved link; ignore it.
  onBrokenAnchors: 'ignore',

  // Even if you don't use internationalization, you can use this field to set
  // useful metadata like html lang. For example, if your site is Chinese, you
  // may want to replace "en" with "zh-Hans".
  i18n: {
    defaultLocale: 'en',
    locales: ['en'],
  },

  presets: [
    [
      'classic',
      {
        docs: {
          sidebarPath: './sidebars.ts',
          routeBasePath: '/',
          editUrl: 'https://github.com/homotechsual/busybar-dotnet/tree/main/website/',
        },
        blog: false,
        theme: {
          customCss: './src/css/custom.css',
        },
      } satisfies Preset.Options,
    ],
  ],

  themeConfig: {
    // Replace with your project's social card
    image: 'img/docusaurus-social-card.jpg',
    colorMode: {
      defaultMode: 'dark',
      disableSwitch: false,
      respectPrefersColorScheme: true,
    },
    navbar: {
      title: 'BusyBar .NET',
      logo: {
        alt: 'BusyBar .NET Logo',
        src: 'img/logo.svg',
      },
      items: [
        {
          type: 'docSidebar',
          sidebarId: 'docsSidebar',
          position: 'left',
          label: 'Docs',
        },
        {
          type: 'docSidebar',
          sidebarId: 'apiSidebar',
          position: 'left',
          label: 'API Reference',
        },
        {
          href: 'https://github.com/homotechsual/busybar-dotnet',
          label: 'GitHub',
          position: 'right',
          target: '_blank',
          className: 'github-link',
        },
      ],
    },
    footer: {
      style: 'dark',
      links: [
        {
          title: 'Docs',
          items: [
            {
              label: 'Getting Started',
              to: '/intro',
            },
            {
              label: 'API Reference',
              to: '/api',
            },
          ],
        },
        {
          title: 'More',
          items: [
            {
              label: 'GitHub',
              href: 'https://github.com/homotechsual/busybar-dotnet',
            },
            {
              label: 'NuGet',
              href: 'https://www.nuget.org/packages/BusyBar',
            },
          ],
        },
      ],
      copyright: `Copyright © ${new Date().getFullYear()} MJCO.<br />Built with <a href="https://docusaurus.io">Docusaurus v${DOCUSAURUS_VERSION}</a>.<br /><span class="designedBy">Designed with <svg xmlns="http://www.w3.org/2000/svg" class="heart" width="24" height="24" viewBox="0 0 24 24"><path d="M14 20.408c-.492.308-.903.546-1.192.709-.153.086-.308.17-.463.252h-.002a.75.75 0 01-.686 0 16.709 16.709 0 01-.465-.252 31.147 31.147 0 01-4.803-3.34C3.8 15.572 1 12.331 1 8.513 1 5.052 3.829 2.5 6.736 2.5 9.03 2.5 10.881 3.726 12 5.605 13.12 3.726 14.97 2.5 17.264 2.5 20.17 2.5 23 5.052 23 8.514c0 3.818-2.801 7.06-5.389 9.262A31.146 31.146 0 0114 20.408z"/></svg> by <a href="https://homotechsual.dev">homotechsual</a></span>`,
    },
    prism: {
      theme: prismThemes.github,
      darkTheme: prismThemes.dracula,
    },
  } satisfies Preset.ThemeConfig,
};

export default config;
