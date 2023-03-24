import { useEffect, useState } from "react";
import deleteI from "../../images/delete.svg";
import { addDirection } from "../../services/actions/directionsAPI";
import { useAppDispatch, useAppSelector } from "../../utils/hooks";
import { AddBtn } from "../add-btn/add-btn";

type ModalPropsT = {
  close: Function;
};

export const AddDirectionModal = ({ close }: ModalPropsT) => {
  const [name, setName] = useState<string>("New direction");
  const loading = useAppSelector(
    (store) => store.directions.add_direction_loading
  );
  const error = useAppSelector((store) => store.directions.add_direction_error);
  const success = useAppSelector(
    (store) => store.directions.add_direction_success
  );
  const dispatch = useAppDispatch();
  const addNewDirection = () => {
    dispatch(addDirection(name));
  };
  useEffect(() => {
    if (name == "") return; //TODO
    if (!loading && success) {
      //console.log("new dir added!");
      close();
    }
  }, [loading, success]);
  return (
    <>
      <div className="w-full h-full absolute top-0 left-0 opacity-50 bg-black z-20">
        {" "}
      </div>
      <div className="w-full absolute flex justify-center top-1/4">
        <div className="w-2/3 p-3 pb-2 bg-slate-50 rounded-lg drop-shadow-lg z-40 opacity-100">
          <img
            src={deleteI}
            className="cursor-pointer mr-0 ml-auto"
            onClick={(e) => close()}
          ></img>
          {/* <h1 className="font-bold text-3xl">New Direction</h1> */}
          <Input
            label={"direction name"}
            value={name}
            onChange={setName}
          />
          <AddBtn label={"add a new direction"} onClick={addNewDirection} unactive={name===""}/>
        </div>
      </div>
    </>
  );
};

type InputPropsT = {
  label: string;
  value: string;
  onChange: Function;
};

const Input = ({ label, value, onChange }: InputPropsT) => {
  return (
    <>
      <input
        type="text"
        placeholder={label}
        onChange={(e) => onChange(e.target.value)}
        value={value}
        className="border w-full border-gray-300 rounded-lg pl-2 pr-2 p-2 mt-2"
      />
    </>
  );
};
