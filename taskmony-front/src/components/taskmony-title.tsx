import logo from "../images/logo.svg";

export const TaskmonyTitle = () => {
  return (
    <div className={"absolute z-40 m-4 flex"}>
      <img src={logo} className={"w-7 h-7 m-1"}/>
      <h1 className="font-bold text-3xl">TASKMONY</h1>
    </div>
  );
};
