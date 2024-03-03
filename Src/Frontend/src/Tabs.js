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
      label: 'Результаты изменений',
      image: IconTest,
  },
];

const getItemLabel = (item) => item.label;
const getItemIcon = (item) => item.image;

export const TabsExample = (props) => {
  return (
    <Tabs
      value={items.find(r => r.label == props.value)}
      onChange={(event) => {
        props.x(event.value)
      }}
      items={items}
      getItemLabel={getItemLabel}
      getItemLeftIcon={getItemIcon}
    />
  );
};