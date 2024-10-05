import { RequestMessages } from '../types.ts';

const createMessage = (base: string | RequestMessages, args?: string[]) => {
  if (args) return `${base}:${args.join(';')}`;

  return `${base}`;
};

export default createMessage;
