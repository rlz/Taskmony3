import { NavLink } from "react-router-dom";
import deleteI from "../../images/delete.svg";
import { Btn } from "./btn";
import { Input } from "./input";
import { useNavigate, } from "react-router-dom";
import { TaskmonyTitle } from "../../components/taskmony-title";


export const ResetPassword = () => {
  const navigate = useNavigate();
  const resetPassword = () => 
  {
    navigate("/login");
  }
  return (
    <>
    <TaskmonyTitle/>
    <div className="w-full h-full absolute flex justify-center content-center bg-slate-50">
    <div className="w-1/3 m-auto pb-20">
    <h1 className="font-bold text-3xl">Reset password</h1>
      <Input label={"new password"} type={"password"} />
      <Input label={"code from email"} type={"text"} />
      <p className="mt-2">remembered your password? <NavLink to="/login" className="font-semibold underline">sign in</NavLink></p>
      <div className="mt-10">
      <Btn label={"save changes"} onClick={resetPassword} />
      </div>
    </div>
    </div>

    </>
  );
};


  