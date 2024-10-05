import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Login from '../components/Login/Login';
import WaitingLobby from '../components/WaitingLobby/WaitingLobby';
import Game from '../components/Game/Game';
import { WebsocketContextProvider } from '../contexts/WebSocketContext/WebsocketContextProvider.tsx';

function App() {
  return (
    <WebSocketProvider>
      <Router>
        <Routes>
          <Route path="/" element={<Login />} />
          <Route
            path="/lobby"
            element={<WaitingLobby players={[]} username={null} />}
          />
          <Route
            path="/game"
            element={<Game /*username={""} players={[]} */ />}
          />
        </Routes>
      </Router>
    </WebSocketProvider>
  );
}

export default App;
