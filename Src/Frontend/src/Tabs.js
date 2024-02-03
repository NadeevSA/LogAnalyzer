import React, { useState } from 'react';
import { Tabs } from '@consta/uikit/Tabs';
import { IconSettings } from '@consta/icons/IconSettings';
import { IconTest } from '@consta/icons/IconTest';
import { IconRoute } from '@consta/icons/IconRoute';

const items = [
    {
      label: 'Настройки',
      image: IconSettings,
    },
    {
        label: 'Результаты',
        image: IconTest,
    },
    {
      label: 'Git',
      image: IconRoute,
  },
  ];

const getItemLabel = (item) => item.label;
const getItemIcon = (item) => item.image;

export const TabsExample = (props) => {
  return (
    <Tabs
      value={items.find(i => i.label == props.value)}
      onChange={(event) => {
        props.x(event.value)
    }}
      items={items}
      getItemLabel={getItemLabel}
      getItemLeftIcon={getItemIcon}
    />
  );
};