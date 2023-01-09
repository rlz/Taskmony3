import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './components/app/app';

import {
  createBrowserRouter,
  RouterProvider,
} from "react-router-dom";
import reportWebVitals from './reportWebVitals';
import ErrorPage from './pages/error-page/error-page';
import MyIdeas from './pages/my-ideas/my-ideas';
import MyTasks from './pages/my-tasks/my-tasks';
import Archive from './pages/archive/archive';

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);

root.render(
  <App/>
);

reportWebVitals();
