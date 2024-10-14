import React, { MouseEventHandler } from 'react';
import { ButtonStyle } from './types.ts';
import styles from './Button.module.scss';
import classNames from 'classnames/bind';

interface ButtonProps {
  children: React.ReactNode;
  style?: ButtonStyle;
  onClick?: MouseEventHandler<HTMLButtonElement>;
  className?: string;
  disabled?: boolean;
}

const cn = classNames.bind(styles);

const Button = ({
  children,
  style = ButtonStyle.Basic,
  onClick,
  disabled = false,
  className,
}: ButtonProps) => {
  return (
    <button
      className={cn('button', `button--${style}`, className)}
      onClick={onClick}
      disabled={disabled}
    >
      {children}
    </button>
  );
};

export default Button;
