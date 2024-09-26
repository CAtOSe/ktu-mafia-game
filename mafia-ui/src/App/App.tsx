/*import './App.css'
import Login from "../Login/Login.tsx";
//import WaitingLobby from "../WaitingLobby/WaitingLobby.tsx";

function App() {

  return (
    <>
      <Login />
    </>
  )
}

export default App
*/
import './App.css'
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Login from '../Login/Login';
import WaitingLobby from '..//WaitingLobby/WaitingLobby';
import Game from '../Game/Game'; // Import the Game component

function App() {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<Login />} />
                <Route path="/lobby" element={<WaitingLobby players={[]} username={null} />} />
                <Route path="/game" element={<Game /*username={""} players={[]} *//>} />
            </Routes>
        </Router>
    );
}

export default App;
