import { useState } from 'react';
import { Modal } from '@consta/uikit/Modal';
import { Text } from '@consta/uikit/Text';
import { Button } from '@consta/uikit/Button';

export function ModalExample() {
    const [isModalOpen, setIsModalOpen] = useState(false);
  
    return (
      <div>
        <Button
          size="m"
          view="primary"
          label="Открыть модальное окно"
          width="default"
          onClick={() => setIsModalOpen(true)}
        />
        <Modal
          isOpen={isModalOpen}
          hasOverlay
          onClickOutside={() => setIsModalOpen(false)}
          onEsc={() => setIsModalOpen(false)}
        >
          <Text as="p" size="s" view="secondary" lineHeight="m">
            Это заголовок модального окна
          </Text>
          <Text as="p" size="s" view="secondary" lineHeight="m">
            Это содержимое модального окна. Здесь может быть что угодно: текст,
            изображение, форма или таблица. Всё, что хочется вынести из контекста
            и показать поверх основной страницы.
          </Text>
          <div>
            <Button
              size="m"
              view="primary"
              label="Закрыть"
              width="default"
              onClick={() => setIsModalOpen(false)}
            />
          </div>
        </Modal>
      </div>
    );
  }