import type {ReactNode} from 'react';
import clsx from 'clsx';
import Heading from '@theme/Heading';
import styles from './styles.module.css';

type FeatureItem = {
  title: string;
  Svg: React.ComponentType<React.ComponentProps<'svg'>>;
  description: ReactNode;
};

const FeatureList: FeatureItem[] = [
  {
    title: 'Full API Coverage',
    Svg: require('@site/static/img/icon-coverage.svg').default,
    description: (
      <>
        Every namespace of the BUSY Bar HTTP API: Account, Assets, Audio, Ble,
        Busy timer, Display, Input, Settings, SmartHome, Storage, System, Time,
        Update, and Wifi.
      </>
    ),
  },
  {
    title: 'Typed & Idiomatic',
    Svg: require('@site/static/img/icon-typed.svg').default,
    description: (
      <>
        C# records, polymorphic JSON via <code>System.Text.Json</code>, and a
        documented exception contract, modelled on the official TypeScript
        client&apos;s naming.
      </>
    ),
  },
  {
    title: 'Local & Cloud',
    Svg: require('@site/static/img/icon-cloud.svg').default,
    description: (
      <>
        Works against a device over USB/LAN or the BUSY Cloud proxy; the
        library builds the right request URLs for each transport
        automatically.
      </>
    ),
  },
  {
    title: 'Validated on Real Hardware',
    Svg: require('@site/static/img/icon-hardware.svg').default,
    description: (
      <>
        Every namespace has been exercised against a physical BUSY Bar,
        display draw, brightness, LED notifications, and more, not just
        synthetic fixtures.
      </>
    ),
  },
];

function Feature({title, Svg, description}: FeatureItem) {
  return (
    <div className={clsx('col col--3')}>
      <div className="text--center">
        <Svg className={styles.featureSvg} role="img" />
      </div>
      <div className="text--center padding-horiz--md">
        <Heading as="h3">{title}</Heading>
        <p>{description}</p>
      </div>
    </div>
  );
}

export default function HomepageFeatures(): ReactNode {
  return (
    <section className={styles.features}>
      <div className="container">
        <div className="row">
          {FeatureList.map((props, idx) => (
            <Feature key={idx} {...props} />
          ))}
        </div>
      </div>
    </section>
  );
}
