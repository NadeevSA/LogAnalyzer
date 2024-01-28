import React, { useState } from 'react';
import { Tabs } from '@consta/uikit/Tabs';
import { IconSettings } from '@consta/icons/IconSettings';
import { IconTest } from '@consta/icons/IconTest';

const items = [
    {
      label: 'Настройки',
      image: IconSettings,
    },
    {
        label: 'Результаты',
        image: IconTest,
    },
  ];

const getItemLabel = (item) => item.label;
const getItemIcon = (item) => item.image;

export const TabsExample = (props) => {
  const [value, setValue] = useState(items[0]);
  return (
    <Tabs
      value={value}
      onChange={(event) => {
        setValue(event.value)
        props.x(event.value)
    }}
      items={items}
      getItemLabel={getItemLabel}
      getItemLeftIcon={getItemIcon}
    />
  );
};