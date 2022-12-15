import { Link } from "react-router-dom";

function Login() {
    return (
    <>
      <h1 className="text-3xl font-bold underline text-red-600">
        Login
      </h1>
      <Link to={`/tasks`}>login</Link>
    </>
    );  
  }
  
  export default Login;