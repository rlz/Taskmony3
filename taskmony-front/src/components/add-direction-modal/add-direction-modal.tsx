import deleteI from "../../images/delete.svg";
import { AddBtn } from "../add-btn/add-btn";

type ModalPropsT = {
  close: Function;
};

export const AddDirectionModal = ({ close }: ModalPropsT) => {
  return (
    <>
    <div  className="w-full h-full absolute top-0 left-0 opacity-50 bg-black z-20"> </div>
    <div className="w-full absolute flex justify-center top-1/4">
    <div className="w-2/3 p-3 pb-2 bg-slate-50 rounded-lg drop-shadow-lg z-40 opacity-100">
      <img
        src={deleteI}
        className="cursor-pointer mr-0 ml-auto"
        onClick={(e) => close()}
      ></img>
      {/* <h1 className="font-bold text-3xl">New Direction</h1> */}
      <Input label={"direction name"} />
      <AddBtn label={"add a new direction"} onClick={() => {}} />
    </div>
    </div>

    </>
  );
};

type InputPropsT = {
    label:string
  };

const Input = ({ label } : InputPropsT) => {
    return (
      <>
      <input
        type="text"
        defaultValue={label}
        className="border w-full border-gray-300 rounded mt-2 pl-1 pr-2"
      />
      </>
    );
  };