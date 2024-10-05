import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import App from './App/App.tsx';
import './index.css';
import AppV2 from './AppV2/AppV2.tsx';

const appVersion = localStorage.getItem('app-version') ?? '1';

createRoot(document.getElementById('root')!).render(
  <StrictMode>{appVersion === '2' ? <AppV2 /> : <App />}</StrictMode>,
);
