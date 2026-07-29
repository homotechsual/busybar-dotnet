import type {ReactNode} from 'react';
import clsx from 'clsx';
import Link from '@docusaurus/Link';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import Layout from '@theme/Layout';
import CodeBlock from '@theme/CodeBlock';
import HomepageFeatures from '@site/src/components/HomepageFeatures';
import Heading from '@theme/Heading';

import styles from './index.module.css';

const quickStart = `using Busy.Bar;

var bar = new BusyBar(new BusyBarOptions { Addr = "10.0.4.20" });

var status = await bar.SystemStatusGetAsync();

await bar.DisplayDrawAsync(new DisplayDrawParams
{
    ApplicationName = "my_app",
    Elements = new DisplayElement[]
    {
        new TextElement
        {
            Id = "0",
            Text = "Hello!",
            Font = TextFont.Normal,
            Align = ElementAlign.Center,
        }
    }
});`;

function HomepageHeader() {
  const {siteConfig} = useDocusaurusContext();
  return (
    <header className={clsx('hero hero--primary', styles.heroBanner)}>
      <div className="container">
        <p className={styles.eyebrow}>
          <span className={styles.statusDot} /> All 14 API namespaces &middot;
          validated on real hardware
        </p>
        <Heading as="h1" className="hero__title">
          {siteConfig.title}
        </Heading>
        <p className="hero__subtitle">{siteConfig.tagline}</p>
        <p className={styles.badges}>
          <a href="https://github.com/homotechsual/busybar-dotnet/actions/workflows/ci.yml">
            <img
              src="https://img.shields.io/github/actions/workflow/status/homotechsual/busybar-dotnet/ci.yml?branch=main&style=for-the-badge&label=CI"
              alt="CI status"
            />
          </a>
          <a href="https://www.nuget.org/packages/BusyBar">
            <img
              src="https://img.shields.io/nuget/v/BusyBar?style=for-the-badge&label=NuGet"
              alt="NuGet version"
            />
          </a>
          <a href="https://codecov.io/gh/homotechsual/busybar-dotnet">
            <img
              src="https://img.shields.io/codecov/c/github/homotechsual/busybar-dotnet?style=for-the-badge&label=Coverage"
              alt="Test coverage"
            />
          </a>
        </p>
        <div className={styles.buttons}>
          <Link className="button button--secondary button--lg" to="/intro">
            Get Started
          </Link>
          <Link
            className="button button--outline button--secondary button--lg margin-left--md"
            to="/api">
            API Reference
          </Link>
        </div>
      </div>
    </header>
  );
}

function QuickLook() {
  return (
    <section className={styles.quickLook}>
      <div className="container">
        <div className="row">
          <div className="col col--6">
            <Heading as="h2">Quick look</Heading>
            <p>
              One package, one client. Connect over USB/LAN or the BUSY Cloud
              proxy, drive the display, and read device status, all through
              typed C# records instead of hand-rolled JSON.
            </p>
            <p>
              <Link to="/intro">Read the full getting-started guide →</Link>
            </p>
          </div>
          <div className="col col--6">
            <CodeBlock language="csharp" title="Program.cs">
              {quickStart}
            </CodeBlock>
          </div>
        </div>
      </div>
    </section>
  );
}

export default function Home(): ReactNode {
  const {siteConfig} = useDocusaurusContext();
  return (
    <Layout
      title={siteConfig.title}
      description="A typed .NET client for the BUSY Bar HTTP API">
      <HomepageHeader />
      <main>
        <QuickLook />
        <HomepageFeatures />
      </main>
    </Layout>
  );
}
