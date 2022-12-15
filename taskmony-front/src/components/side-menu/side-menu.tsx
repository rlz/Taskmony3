import { Link } from "react-router-dom";

function SideMenu() {
    return (
        <div>
        <nav>
          <ul>
            <li>
              <Link to={`tasks`}>My Tasks</Link>
            </li>
            <li>
              <Link to={`ideas`}>My Ideas</Link>
            </li>
            <li>
              <Link to={`archive`}>Archive</Link>
            </li>
          </ul>
        </nav>
        </div>
    );  
  }
  
  export default SideMenu;