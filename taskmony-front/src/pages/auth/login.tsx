import { NavLink, useNavigate } from "react-router-dom";
import deleteI from "../../images/delete.svg";
import { Btn } from "./btn";
import { Input } from "./input";


export const Login = () => {
  const navigate = useNavigate();
  const login = () => 
  {
    navigate("/");
  }
  return (
    <>
    <div className="w-full h-full absolute flex justify-center content-center bg-slate-50">
    <div className="w-1/3 m-auto pb-20">
    <h1 className="font-bold text-3xl">Sign in</h1>
      <Input label={"email"} type={"email"} />
      <Input label={"password"} type={"password"} />
      <NavLink to="/forgot-password"><p className="mt-2">forgot your password?</p></NavLink>
      <div className="mt-10">
      <Btn label={"sign in"} onClick={login} />
      </div>
      <p className="mt-2">or <NavLink to="/register" className="font-semibold underline">sign up</NavLink></p>
    </div>
    </div>
    </>
  );
};


  