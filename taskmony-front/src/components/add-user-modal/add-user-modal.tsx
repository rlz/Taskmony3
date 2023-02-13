import { useEffect, useState } from "react";
import deleteI from "../../images/delete.svg";
import { addUser } from "../../services/actions/directionsAPI";
import { getUser } from "../../services/actions/userInfo";
import useIsFirstRender, {
  useAppDispatch,
  useAppSelector,
} from "../../utils/hooks";
import { AddBtn } from "../add-btn/add-btn";
import { useLocation } from "react-router-dom";

type ModalPropsT = {
  close: Function;
};

export const AddUserModal = ({ close }: ModalPropsT) => {
  const loading = useAppSelector((store) => store.directions.add_user_loading);
  const error = useAppSelector((store) => store.directions.add_user_error);
  const success = useAppSelector((store) => store.directions.add_user_success);
  const isFirst = useIsFirstRender();
  const [errorMessage, setErrorMessage] = useState("");
  useEffect(() => {
    if (success && !isFirst) close();
  }, [success]);
  useEffect(() => {
    if (error && !isFirst) setErrorMessage("cannot add this user");
  }, [error]);
  const location = useLocation();
  const directionId = location.pathname.split("/")[2];
  return (
    <>
      <div className="w-full h-full absolute top-0 left-0 opacity-50 bg-black z-30"></div>
      <div className="w-2/3 absolute p-3 pb-2 bg-slate-50 rounded-lg drop-shadow-lg z-40">
        <img
          src={deleteI}
          className="cursor-pointer mr-0 ml-auto"
          onClick={(e) => close()}
        ></img>
        {/* <h1 className="font-bold text-3xl mt-0 pt-0">New User</h1> */}
        <Input directionId={directionId} />
        <p>{errorMessage}</p>
        <AddBtn label={"add a new user"} onClick={() => {}} />
      </div>
    </>
  );
};

type InputPropsT = {
  directionId: string;
};

const Input = ({ directionId }: InputPropsT) => {
  const dispatch = useAppDispatch();
  const [value, setValue] = useState("");
  const foundUsers = useAppSelector((store) => store.userInfo.users);
  const addNewUser = (user) => {
    dispatch(addUser(directionId, user));
  };
  return (
    <>
      <input
        type="text"
        value={value}
        onChange={(e) => setValue(e.target.value)}
        placeholder={"user login"}
        className="border w-full border-gray-300 rounded pl-2 pr-2 p-2 mt-2"
        onKeyDown={(e) => {
          if (e.key !== "Enter") return;
          dispatch(getUser(value));
        }}
      />
      <div className="border w-full border-gray-200 rounded">
        {foundUsers.map((user) => (
          <SearchItem
            label={user.displayName}
            addUser={() => addNewUser(user)}
          />
        ))}
      </div>
    </>
  );
};

const SearchItem = ({ label, addUser }: InputPropsT) => {
  return (
    <div
      className="border w-full border-gray-200 bg-slate-100 pl-1 pr-2 p-1 cursor-pointer hover:font-semibold"
      onClick={addUser}
    >
      {label}
    </div>
  );
};
