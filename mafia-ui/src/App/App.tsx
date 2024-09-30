import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Login from '../components/Login/Login';
import WaitingLobby from '../components/WaitingLobby/WaitingLobby';
import Game from '../components/Game/Game'; // Import the Game component

function App() {
  return (
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
  );
}

export default App;
