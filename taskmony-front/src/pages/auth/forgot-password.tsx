import { NavLink, useNavigate } from "react-router-dom";
import deleteI from "../../images/delete.svg";
import { Btn } from "./btn";
import { Input } from "./input";


export const ForgotPassword = () => {
    const navigate = useNavigate();
  const resetPassword = () => 
  {
    navigate("/reset-password");
  }
  return (
    <>
    <div className="w-full h-full absolute flex justify-center content-center bg-slate-50">
    <div className="w-1/3 m-auto pb-20">
    <h1 className="font-bold text-3xl">Reset password</h1>
      <Input label={"email"} type={"email"} />
      <p className="mt-2">remembered your password? <NavLink to="/login" className="font-semibold underline">sign in</NavLink></p>
      <div className="mt-10">
      <Btn label={"recover password"} onClick={resetPassword} />
      </div>
    </div>
    </div>

    </>
  );
};


  