import { NavLink, useNavigate } from "react-router-dom";
import deleteI from "../../images/delete.svg";
import { Btn } from "./btn";
import { Input } from "./input";


export const Register = () => {
  const navigate = useNavigate();
  const register = () => 
  {
    navigate("/");
  }
  return (
    <>
    <div className="w-full h-full absolute flex justify-center content-center bg-slate-50">
    <div className="w-1/3 m-auto pb-20">
    <h1 className="font-bold text-3xl">Sign up</h1>
      <Input label={"name"} type={"text"} />
      <Input label={"email"} type={"email"} />
      <Input label={"password"} type={"password"} />
      <div className="mt-10">
      <Btn label={"sign up"} onClick={register} />
      </div>
      <p>Or <NavLink to="/login" className="font-semibold underline">sign in</NavLink></p>
    </div>
    </div>

    </>
  );
};


  