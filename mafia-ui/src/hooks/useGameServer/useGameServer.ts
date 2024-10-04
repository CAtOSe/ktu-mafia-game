import { useContext, useEffect, useState } from 'react';
import { WebSocketContext } from '../../contexts/WebSocketContext/WebsocketContext.tsx';
import { Message } from '../../contexts/WebSocketContext/types.ts';

interface UseGameServerProps {
  id: string;
  filter?: string[];
}

const useGameServer = ({ id, filter }: UseGameServerProps) => {
  const ws = useContext(WebSocketContext);
  const [lastMessage, setLastMessage] = useState<Message>();

  if (ws === undefined) {
    throw new Error(
      'Undefined WebSocketContext. Perhaps missing a top-level Provider?',
    );
  }

  useEffect(() => {
    const subId = ws.subscribe({ id, filter, onReceive });

    return () => {
      ws.unsubscribe(subId);
    };
  }, [ws, filter, id]);

  const onReceive = (message: Message) => {
    setLastMessage(message);
  };

  return {
    sendMessage: ws.sendMessage,
    lastMessage,
  };
};

export default useGameServer;
