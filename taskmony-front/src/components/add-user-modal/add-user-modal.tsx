import deleteI from "../../images/delete.svg";
import { AddBtn } from "../add-btn/add-btn";

type ModalPropsT = {
  close: Function;
};

export const AddUserModal = ({ close }: ModalPropsT) => {
  return (
    <>
    <div  className="w-full h-full absolute top-0 left-0 opacity-50 bg-black z-30"></div>
    <div className="w-2/3 absolute p-3 pb-2 bg-slate-50 rounded-lg drop-shadow-lg z-40">
      <img
        src={deleteI}
        className="cursor-pointer mr-0 ml-auto"
        onClick={(e) => close()}
      ></img>
      {/* <h1 className="font-bold text-3xl mt-0 pt-0">New User</h1> */}
      <Input label={"Al"} />
      <AddBtn label={"add a new user"} onClick={() => {}} />
    </div>
    </>
  );
};

type InputPropsT = {
    label:string
  };



const Input = ({ label } : InputPropsT) => {
    const searchValues = ['Alex Smith','Alex Lee','Alexander Ivanov']
    return (
      <>
      <input
        type="text"
        defaultValue={label}
        className="border w-full border-gray-300 rounded pl-2 pr-2 p-2 mt-2"
      />
      <div className="border w-full border-gray-200 rounded">
      {searchValues.map(name=>(
        <SearchItem label={name}/>
      ))}
      </div>
      </>
    );
  };
  
const SearchItem = ({ label } : InputPropsT) => {
    return(
<div className="border w-full border-gray-200 bg-slate-100 pl-1 pr-2 p-1 cursor-pointer hover:font-semibold">{label}</div>
    );
}